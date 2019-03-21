"""
Парсинг датасетов из соревнования Rucoref-2015 (раскрытие анафоры и пр.)
Описание исходного датасета и задачи http://www.dialog-21.ru/evaluation/2014/anaphora/
"""
import io
import os
import pandas as pd
import csv


rucoref_folder = '../../../data/rucoref_2015/rucoref_29.10.2015'
output_file = '../../../tmp/ruanaphora_corpus.dat'

df_tokens = pd.read_csv(os.path.join(rucoref_folder, 'Tokens.txt'), encoding='utf-8', delimiter='\t', quoting=3)
df_groups = pd.read_csv(os.path.join(rucoref_folder, 'Groups.txt'), encoding='utf-8', delimiter='\t', quoting=3)

groupid2content = dict(zip(df_groups['group_id'].values, df_groups['content']))
groupid2link = dict(zip(df_groups['group_id'].values, df_groups['link']))

token2refcontent = dict()
for i, r in df_groups.iterrows():
    doc_id = r['doc_id']
    shift = r['shift']
    link = r['link']
    attr = r['attributes']
    if attr in ['ref:def|str:pron|type:anaph', 'ref:def|str:pron|type:coref']:
        token_id = (doc_id, shift)
        if link != 0:

            new_groupid = link
            njump = 0
            while njump < 5:
                link2 = groupid2link[new_groupid]
                if link2 != 0:
                    new_groupid = groupid2link[new_groupid]
                    njump += 1
                else:
                    break

            token2refcontent[token_id] = groupid2content[new_groupid]

df_res = pd.DataFrame(columns='doc_id shift token lemma gram refcontent'.split(), index=None)
n_discovered = 0
for i, r in df_tokens.iterrows():
    doc_id = r['doc_id']
    shift = r['shift']
    token_id = (doc_id, shift)
    token = r['token']
    lemma = r['lemma']
    gram = r['gram']
    refcontent = token2refcontent.get(token_id, '')
    n_discovered += refcontent != ''

    df_res = df_res.append({'doc_id': doc_id, 'shift': shift, 'token': token, 'lemma': lemma, 'gram': gram, 'refcontent': refcontent}, ignore_index=True)

df_res.to_csv(output_file, quoting=csv.QUOTE_MINIMAL, index=False, sep='\t')

print(u'раскрыто анафор={}'.format(n_discovered))

