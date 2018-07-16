# -*- coding: utf-8 -*-
'''
Сортировка списка предложений через последовательное применение LSA и t-SNE (встраивание
векторов LSA в 1d)
'''

from __future__ import division  # for python2 compatability
from __future__ import print_function

import codecs
import numpy as np

from sklearn.manifold import TSNE
from sklearn.decomposition import TruncatedSVD
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.pipeline import Pipeline

input_path = r'e:\polygon\paraphrasing\data\facts4_1s.txt'
output_path = '../tmp/facts4_1s.txt'

LSA_DIMS = 60

def v_cosine(a, b):
    return np.dot(a,b)/(np.linalg.norm(a)*np.linalg.norm(b))

print('Buidling tf-idf corpus...')
tfidf_corpus = set()

with codecs.open(input_path, 'r', 'utf-8') as rdr:
    for line in rdr:
        phrase = line.strip()
        if len(phrase) > 0:
            tfidf_corpus.add(phrase)

tfidf_corpus = list(tfidf_corpus)
print('{} phrases in tfidf corpus'.format(len(tfidf_corpus)))

print('Fitting LSA...')
vectorizer = TfidfVectorizer(max_features=None, ngram_range=(3, 5), min_df=1, analyzer='char')
svd_model = TruncatedSVD(n_components=LSA_DIMS, algorithm='randomized', n_iter=20, random_state=42)
svd_transformer = Pipeline([('tfidf', vectorizer), ('svd', svd_model)])
svd_transformer.fit(tfidf_corpus)

print('Calculating LSA vectors for query phrases...')
phrase_ls = svd_transformer.transform(tfidf_corpus)

print('Running t-SNE')
tsne = TSNE(n_components=1)
phrases_1d = tsne.fit_transform(phrase_ls)

print('Printing results')
with codecs.open(output_path, 'w', 'utf-8') as wrt:
    phrases = [(tfidf_corpus[i], phrases_1d[i]) for i in range(len(tfidf_corpus))]
    phrases = sorted(phrases, key=lambda z: z[1])
    for phrase, _ in phrases:
        wrt.write(u'{}\n'.format(phrase))
