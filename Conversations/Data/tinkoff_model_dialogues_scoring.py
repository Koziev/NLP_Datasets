"""
Разметка реплик в диалогах оценками релевантности и специфичности с помощью
модели tinkoff-ai/response-quality-classifier-base
"""

import io
import os
import collections
import json

import torch
from transformers import AutoTokenizer, AutoModelForSequenceClassification
import tqdm


def process_batch(dialogues):
    tx = []
    input_2_idialog = []
    idialog2result = collections.defaultdict(list)

    for idialog, lines in enumerate(dialogues):
        for i, reply in enumerate(lines):
            if i > 0:
                context = lines[:i]
                reply = lines[i]
                t = '[CLS]{}[RESPONSE_TOKEN]{}'.format('[SEP]'.join(context), reply)
                tx.append(t)
                input_2_idialog.append((idialog, i))
            else:
                idialog2result[idialog].append((reply, 1.0, 1.0))

    inputs = tokenizer(tx, max_length=512, truncation=True, padding=True, add_special_tokens=False, return_tensors='pt').to(device)

    with torch.no_grad():
        logits = model(**inputs).logits
        probas = torch.sigmoid(logits).cpu().detach().numpy()

    for msg_probas, (idialog, iline) in zip(probas, input_2_idialog):
        idialog2result[idialog].append((dialogues[idialog][iline], float(msg_probas[0]), float(msg_probas[1])))

    return [lines_probas for idialog, lines_probas in sorted(idialog2result.items(), key=lambda z: z[0])]


if __name__ == '__main__':
    proj_dir = os.path.expanduser('~/polygon/chatbot')

    # Сколько реплик диалогов максимально обрабатывать в рамках одного пакета.
    # ATT: выбирать с учетом того, что токенизация в bert не всегда очевидная, так что просто подобрать эмпирически под gpu
    BATCH_SIZE = 10

    device = 'cuda'
    tokenizer = AutoTokenizer.from_pretrained('tinkoff-ai/response-quality-classifier-base')
    model = AutoModelForSequenceClassification.from_pretrained('tinkoff-ai/response-quality-classifier-base')
    model.to(device)
    model.eval()

    dialogues_dataset = os.path.join(proj_dir, 'tmp/chan_dialogues.txt')

    # Определим, сколько всего диалогов в исходном датасете
    print('Counting source dialogues...', end='', flush=True)
    num_dialogues = 0
    with io.open(dialogues_dataset, 'r') as rdr:
        lines = []
        batch = []

        for line in rdr:
            s = line.strip()
            if s:
                lines.append(s)
            else:
                if lines:
                    num_dialogues += 1

                lines = []
    print('done, {} dialogues'.format(num_dialogues))

    with tqdm.tqdm(total=num_dialogues, desc='Filter "{}"'.format(os.path.basename(dialogues_dataset))) as pbar, \
         io.open(os.path.join(proj_dir, 'tmp/tinkoff_dialogues.filtered.txt'), 'w') as wrt_txt, \
         open(os.path.join(proj_dir, 'tmp/tinkoff_dialogues.filtered.jsonl'), 'w') as wrt_jsonl:

        with io.open(dialogues_dataset, 'r') as rdr:
            lines = []
            batch = []

            for line in rdr:
                s = line.strip()
                if s:
                    lines.append(s)
                else:
                    if lines:
                        batch.append(lines)
                        if sum(map(len, batch)) > BATCH_SIZE:
                            try:
                                results = process_batch(batch)
                                pbar.update(len(batch))
                            except RuntimeError:
                                print('DEBUG@96 CUDA OOM')
                                results = []
                                for dialog in batch:
                                    res1 = process_batch([dialog])
                                    results.append(res1[0])

                            batch = []

                            for lines_probas in results:
                                # Имеем список сообщений в диалоге с оценками релевантности и спефицичности.

                                # Сохраним в jsonl файле для последующей обработки
                                json.dump(lines_probas, wrt_jsonl, indent=None, ensure_ascii=False)
                                wrt_jsonl.write('\n')
                                wrt_jsonl.flush()

                                # Если все реплики в диалоге, кроме первой, оценены как релевантные и специфичные,
                                # то отложим такой диалог в чистый датасет.
                                min_relevance = min(r for _, r, _ in lines_probas)
                                min_specificity = min(s for _, _, s in lines_probas)
                                if min_relevance > 0.5 and min_specificity > 0.5:
                                    for line, _, _ in lines_probas:
                                        wrt_txt.write('{}\n'.format(line))
                                    wrt_txt.write('\n\n\n')
                                    wrt_txt.flush()

                    lines = []


    print('All done :)')
