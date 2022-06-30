"""
Проверка модели читчата, натренированной с помощью train_chitchat_rugpt.py
"""

import math
import os.path

import torch
import transformers
from transformers import AutoTokenizer, AutoModelForCausalLM
from transformers import TextDataset, DataCollatorForLanguageModeling
from transformers import Trainer, TrainingArguments
from transformers import EarlyStoppingCallback
import sklearn.model_selection


class GptDialogueDataset(torch.utils.data.Dataset):
    def __init__(self, tokenizer, file_path, block_size):
        self.examples = []
        self.transform = None
        with open(file_path, encoding="utf-8") as f:
            chunks = f.read().split('\n\n')  # пустая строка (или две) - разделитель диалогов
            for chunk in chunks:
                if len(chunk) > 0:
                    tokenized_chunk = tokenizer.convert_tokens_to_ids(tokenizer.tokenize(chunk.strip()))
                    l = len(tokenized_chunk)
                    if l < block_size:
                        self.examples.append(tokenizer.build_inputs_with_special_tokens(tokenized_chunk))
                    else:
                        while tokenized_chunk:
                            self.examples.append(tokenizer.build_inputs_with_special_tokens(tokenized_chunk[:block_size]))
                            tokenized_chunk = tokenized_chunk[block_size:]

    def __len__(self):
        return len(self.examples)

    def __getitem__(self, idx):
        if torch.is_tensor(idx):
            idx = idx.tolist()

        sample = self.examples[idx]

        if self.transform:
            sample = self.transform(sample)

        return sample


if __name__ == '__main__':
    device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    model_dir = os.path.expanduser('~/polygon/chatbot/tmp/rugpt_chitchat')

    tokenizer = AutoTokenizer.from_pretrained(model_dir)
    tokenizer.add_special_tokens({'bos_token': '<s>', 'eos_token': '</s>', 'pad_token': '<pad>'})

    model = AutoModelForCausalLM.from_pretrained(model_dir)
    model.to(device)
    model.eval()

    prompt = "- Дай денег в долг!\n-"

    encoded_prompt = tokenizer.encode(prompt, return_tensors="pt").to(device)
    out = model.generate(encoded_prompt, max_length=200, do_sample=True, top_k=35, top_p=0.85, temperature=1.0,
                         num_return_sequences=10, eos_token_id=2, pad_token_id=0)

    # Выведем все 10 вариантов ответной реплики.
    for i, tokens in enumerate(out.cpu().tolist(), start=1):
        tokens = tokens[encoded_prompt.shape[1]:]
        text = tokenizer.decode(tokens)
        reply = text[:text.index('\n')]
        print('[{}] - {}'.format(i, reply))
