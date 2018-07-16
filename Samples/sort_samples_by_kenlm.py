# -*- coding: utf-8 -*-
'''
Постобработка списков предложения для репозитория https://github.com/Koziev/NLP_Datasets
Сортируем предложения в порядке убывания вероятности, получаемой с помощью
предварительно обученной языковой модели KenLM.
'''

from __future__ import print_function

import sys
import os
import codecs
import itertools
import unicodedata
import kenlm
import nltk
import glob


def tokenize(s):
    for word in nltk.word_tokenize(s):
        yield word


def is_punct(word):
    return len(word)>0 and unicodedata.category(word[0]) == 'Po'


def is_num(s):
    if len(s)>0:
        if s[0] in ('-', '+'):
            return s[1:].isdigit()
        return s.isdigit()
    else:
        return False


def prepare_word(w):
    if is_num(w):
        return u'_num_'
    else:
        return w.lower()


def prepare4lm(tokens):
    s2 = unicode.join(u' ', [prepare_word(t) for t in tokens if not is_punct(t) and len(t)>0 ])
    return s2

# -------------------------------------------------------------------

model_filepath = '/home/eek/polygon/kenlm/ru.text.arpa'
print('Loading the language model {}...'.format(model_filepath) )
model = kenlm.Model( model_filepath )

for filename in glob.glob('./*.txt'):
    print(u'Processing {}'.format(filename))

    sent_set = set()
    sent_list = []
    with codecs.open(filename, 'r', 'utf-8') as rdr:
        for line in rdr:
            line2 = prepare4lm( tokenize(line.strip()) )

            if line2 not in sent_set:
                sent_set.add(line2)
                score = model.score(line2, bos=True, eos=True)
                sent_list.append((line2,score))

    new_filename = filename.replace(u'.txt', u'.sorted')
    print(u'Storing {} lines to {}'.format(len(sent_list), new_filename))
    with codecs.open(new_filename, 'w', 'utf-8') as wrt:
        for (sent, _) in sorted(sent_list, key=lambda z:-z[1]):
            wrt.write(sent)
            wrt.write('\n')
