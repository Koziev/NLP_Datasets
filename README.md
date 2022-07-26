# Русскоязычные NLP датасеты

В этом репозитории выложены толко датасеты, которые я создавал (обычно автоматически, иногда с ручной правкой)
для решения разных задач с текстами на русском языке.


## Диалоги и обмены репликами

Диалоги с имиджборд - строго 18+, есть некоторое количество поломанных диалогов, так как отфильтровать их автоматически очень трудно:  
[часть 1](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues.zip)
[часть 2](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues.z01)
[часть 3](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues.z02)
[часть 4](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues.z03)
[часть 5](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues.z04)
[часть 6](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues.z05)

Разметка реплик в этих диалогах оценками релевантности и специфичности, файл в формате jsonl, чтобы отбирать самые качественные диалоги:  
[часть 1](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.zip)
[часть 2](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.z01)
[часть 3](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.z02)
[часть 4](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.z03)
[часть 5](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.z04)
[часть 6](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.z05)
[часть 7](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.z06)
[часть 8](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.z07)
[часть 9](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.z08)
[часть 10](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.z09)
[часть 11](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.z10)
[часть 12](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/chan_dialogues_scored.z11)


[Диалоги из анекдотов](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/extract_dialogues_from_anekdots.tar.xz) - около 90000 диалогов, собранных с разных развлекательных сайтов.

[Почищенные диалоги Cornell Movie Corpus](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/cornell_movie_corpus.tar.xz) - почищенные субтитры, много диалогов "с середины"

Диалоги из худлита (флибуста) - около 400 Мб после распаковки:  
[часть 1](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/extract_flibusta_dialogues.1.tar.xz)
[часть 2](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/extract_flibusta_dialogues.2.tar.xz)

[Еще русскоязычные диалоги из худлита](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/dialogues.zip) - более 130 Мб,
собранных из художественной литературы и подобных источников. В диалогах есть некоторое, относительное
небольшое, количество оставшегося после автоматической чистки мусора.

Пример кода для тренировки читчата на одном из вышеуказанных датасетов: [train_chitchat_rugpt.py](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/train_chitchat_rugpt.py).
В коде надо поправить пути к датасету и каталог, куда будет сохраняться модель, а также скорректировать batch_size.

Проверить натренированный читчат можно с помощью кода [run_chitchat_query.py](https://github.com/Koziev/NLP_Datasets/blob/master/Conversations/Data/run_chitchat_query.py).
Например, натренированный на "анекдотах" читчат для запроса "Дай денег в долг" выдаст примерно такие варианты ответа:

```
[1] -  Откуда у меня деньги?!
[2] -  А ты мне что, должен?
[3] -  А зачем?
[4] -  Что, опять?
[5] -  На себя и детей?
[6] -  У меня денег нет.
[7] -  Откуда у меня деньги?
[8] -  Нет.
[9] -  Не дам!
[10] -  Не дам!
```







## Короткие предложения и словосочетания.

[Датасеты](https://github.com/Koziev/NLP_Datasets/tree/master/Samples) используются для тренировки чат-бота.
Они содержат короткие предложения, извлеченные из большого текстового корпуса,
а также некоторые паттерны и словосочетания.


### Шаблоны предложений с открытыми именными группами

В архиве [templates.clause_with_np.100000.zip](https://github.com/Koziev/NLP_Datasets/tree/master/Samples/templates.clause_with_np.100000.zip)
находится часть датасета с сэмплами следующего вида:

```
52669	есть#NP,Nom,Sing#.
25839	есть#NP,Nom,Plur#.
18371	NP,Masc,Nom,Sing#пожал#NP,Ins#.
17709	NP,Masc,Nom,Sing#покачал#NP,Ins#.
```

Первый столбец - частота. Всего было собрано примерно 21 миллион предложений.

Второй столбец содержит результат shallow parsing'а, в котором именные группы заменены
подстановочными масками вида NP,тэги. Задается падеж, а также число и грамматический род в случаях,
когда это необходимо для правильного согласования с глаголом. Например, запись NP,Nom,Sing
описывает группу существительного в именительном падеже и единственном числе. Символ '#' используется как
разделитель слов и чанков.




### Словосочетания и неполные предложения

Архив [PRN+PreposAdj+V.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/PRN%2BPreposAdj%2BV.zip)
содержит сэмплы вида:
```
Я на автобус опоздаю
Я из автобуса пришел
Мы из автобуса вышли
Я из автобуса вышла
Я из автобуса видел
Я на автобусах езжу
Они на автобусах приезжают
Мы на автобусах объездили
```

Архив [adv+verb.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/adv%2Bverb.zip) содержит
пары наречие+глагол в личной форме:
```
ПРЯМО АРЕСТОВАЛИ
ЛИЧНО атаковал
Немо атаковал
Ровно атаковала
Сегодня АТАКУЕТ
Ближе аттестует
Юрко ахнул
```

Архив [adj+noun.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/adj%2Bnoun.zip) содержит сэмплы типа:
```
ПОЧЕТНЫМ АБОНЕНТОМ
Вашим абонентом
Калининским абонентом
Калининградских аборигенов
Тунисских аборигенов
Байкальских аборигенов
Марсианских аборигенов
Голландские аборигены
```

Более новая и расширенная версия этого набора, собранная другим способом, находится
в архиве [patterns.adj_noun.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/patterns.adj_noun.zip).
Этот датасет имеет выглядит так:
```
8	смутное	предчувствие
8	городская	полиция
8	среднеазиатские	государства
8	чудесное	средство
8	<<<null>>>	претендентка
8	испанский	король
```

Токен <<<null>>> вместо прилагательного означает, что существительное употреблено без
атрибутирующего прилагательного. Такие записи нужны для правильной маргинализации
частот употребления словосочетаний.



Архив [prep+noun.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/prep%2Bnoun.zip) содержит такие паттерны:
```
У аборигенных народов
У аборигенных кобыл
Из аборигенных пород
С помощью аборигенов
На аборигенов
Для аборигенов
От аборигенов
У аборигенов
```


Архив [patterns.noun_gen.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/patterns.noun_gen.zip) содержит 
паттерны из двух существительных, из которых второе в родительном падеже:
```
4	французские	<<<null>>>
4	дворец	фестивалей
4	названье	мест
4	классы	вагонов
4	доступность	магазина
```

Обратите внимание, что если в исходном предложении у генитива были подчиненные прилагательные или PP,
то они в этом датасете будут удалены. Токен <<<null>>> в столбце генитива обозначает 
ситуацию, когда первое существительное употреблено без генитива. Эти записи упрощают
маргинализацию частот.


Архив [patterns.noun_np_gen.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/patterns.noun_np_gen.zip) содержит 
паттерны из существительного и полного правого генитива:
```
окно браузера
течение дня
укус медведки
изюминка такой процедуры
суть декларации
рецепт вкусного молочного коктейля
музыка самого высокого уровня
```


Архив [S+V.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/S%2BV.zip) содержит образцы такого вида:
```
Мы абсолютно не отказали.
Мужчина абсолютно не пострадал.
Они абсолютно совпадают.
Михаил абсолютно не рисковал.
Я абсолютно не выспалась.
Они абсолютно не сочетаются.
Я абсолютно не обижусь...
```

В архиве [S+V+INF.zip](https://github.com/Koziev/NLP_Datasets/blob/master/S%2BV%2BINF.zip) находятся такие образцы:
```
Заславский бахвалился превратить
Ленка бегает поспать
Она бегает умываться
Альбина бегает мерить
Вы бегаете жаловаться
Димка бегал фотографироваться
```

Архив [S+V+INDOBJ.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/S%2BV%2BINDOBJ.zip)
содержит автоматически собранные паттерны подлежащее+глагол+предлог+существительное:
```
Встревоженный аббат пошел навстречу мэру.
Бывший аббат превратился в настоятеля.
Старый Абдуррахман прохаживался возле дома.
Лопоухий абориген по-прежнему был в прострации.
Высокий абориген вернулся с граблями;
Сморщенный абориген сидел за столиком.
```


В архиве [S+V+ACCUS.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/S%2BV%2BACCUS.zip) находятся сэмплы такого вида:
```
Мой агент кинул меня.
Ричард аккуратно поднял Диану.
Леха аккуратно снял Аленку...
Они активируют новые мины!
Адмирал активно поддержал нас.
```

Архив [S+V+INSTR.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/S%2BV%2BINSTR.zip) содержит сэмплы:
```
Я вертел ими
Они вертели ими
Вы вертели мной
Он вертит нами
Она вертит тобой
Она вертит мной
Он вертит ими
Она вертит ими
```

Архив [S+INSTR+V.zip](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/S%2BINSTR%2BV.zip) содержит такие сэмплы:
```
Я тобой брезгую
Они ими бреются
Они ими вдохновляются
Мы ими вертим
Она тобой вертит
Он мной вертит
Он ими вертит
```

Остальные сэмплы - законченные предложения. Для удобства тренировки
диалоговых моделей эти данные разбиты на 3 группы:

### Предложения с глаголом в 1-м лице единственного числа

```
Я только продаю!
Я не курю.
Я НЕ ОТПРАВЛЯЮ!
Я заклеил моментом.
Ездил только я.
```

### Предложения с глаголом в 2-м лице единственного числа

```
Как ты поступишь?
Ты это читаешь?
Где ты живешь?
Док ты есть.
Ты видишь меня.
```

### Предложения с подлежащим-существительным и глаголом в 3-м лице

```
Фонарь имел металлическую скобу.
Щенок ищет добрых хозяев.
Массажные головки имеют встроенный нагрев
Бусины переливаются очень красиво!
```

Предложения в датасетах facts4_1s.txt, facts5_1s.txt, facts5_2s.txt, facts4.txt,
facts6_1s.txt, facts6_2s.txt отсортированы с помощью кода [sort_facts_by_LSA_tSNE.py](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/sort_facts_by_LSA_tSNE.py).
Идея сортировки следующая. Для предложений в файле сначала выполняем [LSA](http://scikit-learn.org/stable/modules/generated/sklearn.decomposition.TruncatedSVD.html),
получая векторы длиной 60 (см. константу LSA_DIMS в коде). Затем эти векторы встраиваются
в одномерное пространство с помощью [t-SNE](http://scikit-learn.org/stable/modules/generated/sklearn.manifold.TSNE.html),
так что в итоге для каждого предложения получается действительное число, такое, что
декартово-близкие в LSA-пространстве предложения имеют небольшую разность этих tsne-чисел. Далее
сортируем предложения согласно t-SNE значения и сохраняем получающийся список.

Предложения в остальных файлах отсортированы программой [sort_samples_by_kenlm.py](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/sort_samples_by_kenlm.py)
в порядке убывания вероятности. Вероятность предложения получается с помощью предварительно
обученной 3-грамной языковой модели [KenLM](https://github.com/kpu/kenlm).

Отдельно выложен файл [questions_2s.txt](https://github.com/Koziev/NLP_Datasets/blob/master/Samples/questions_2s.txt) с вопросами, содержащими финитный глагол в форме 2 лица
единственного числа. Эти вопросы собраны из большого корпуса с текстами, наскрапленными с форумов, субтитрами
и так далее. Для удобства сэмплы отсортированы по финитному глаголу:

```

Берёшь 15 долларов ?
Берёшь денёк на отгул?
Берёшь отпуск за свой счёт?
Берёшь с собой что-нибудь на букву «К»?


Беспокоишься за меня?
Беспокоишься из-за Питера?
Беспокоишься из-за чего?

```

Вопросы отобраны автоматически с помощью [POS Tagger'а](https://github.com/Koziev/rupostagger)
и могут содержать небольшое количество ошибочных сэмплов.



## Разрешение анафоры (Rucoref-2015)

Задача и датасет описаны на [официальной странице соревнования](http://www.dialog-21.ru/evaluation/2014/anaphora/).
Исходный датасет, предоставленный организаторами, [доступен по ссылке](https://github.com/Koziev/NLP_Datasets/blob/master/Anacoref/data/rucoref_29.10.2015.zip).
С помощью скрипта [extract_anaphora.py](https://github.com/Koziev/NLP_Datasets/blob/master/Anacoref/py/extract_anaphora.py) были раскрыты анафоры, в результате
чего получился более простой для тренировки чатбота [датасет](https://github.com/Koziev/NLP_Datasets/blob/master/Anacoref/data/ruanaphora_corpus.dat).
Например, фрагмент данных:

```
1	159	Кругом	кругом	R  
1	166	она	она	P-3fsnn	одинокую дачу  
1	170	была	быть	Vmis-sfa-e  
1	175	обнесена	обнесена	Vmps-sfpsp  
1	184	высоким	высокий	Afpmsif  
1	192	забором	забор	Ncmsin  
```

Видно, что местоимение "она" раскрывается в словосочетание "одинокая дача". Приведение раскрытого
словосочетания к правильной грамматической форме оставлено для следующего этапа.


## Ударения

[Упакованный tsv файл](https://github.com/Koziev/NLP_Datasets/blob/master/Stress/all_accents.zip).

Данные собраны для решения задачи конкурса [ClassicAI](https://classic.sberbank.ai/description).
Использованы открытые данные - Википедия и Викисловарь. В случаях, когда ударение
известно только для одной нормальной формы слова (леммы), я использовал таблицы словоизменения
в [грамматическом словаре](https://github.com/Koziev/GrammarEngine) и генерировал записи с отметкой ударности.
При этом подразумевается, что позиция ударения в слове не меняется при его склонении или спряжении. Для
некоторого количества слов в русском языке это не так, например:

*р^еки* (именительный падеж множественное число)  
*рек^и* (родительный падеж единственное число)  

В таких случаях в датасете будет один из вариантов ударения.


## Статистика употребляемости слов в группах по 2, 3 и 4 слова

Датасеты содержат числовые оценки того, насколько слова чаще употребляются вместе, чем порознь.
Подробности о содержимом и способе получения датасетов см. на [отдельной странице](MutualInfo/README.md).


## Сэмплы со сменой грамматического лица

Пары предложений [в этих сэмплах](https://github.com/Koziev/NLP_Datasets/tree/master/ChangePerson) могут быть полезны для тренировки моделей в составе
чат-бота. Данные выглядят так:

```
Я часто захожу !	ты часто заходишь !
Я сам перезвоню .	ты сам перезвонишь .
Я Вам перезвоню !	ты Вам перезвонишь !
Я не пью .	ты не пьешь .
```

В каждой строке находятся два предложения, отделенные символом табуляции.



## Вопросы и ответы для чат-ботов

Датасеты сгенерированы автоматически из большого корпуса предложений.

[Триады "предпосылка-вопрос-ответ" для предложений длиной 3 слова](https://github.com/Koziev/NLP_Datasets/blob/master/QA/premise_question_answer4.txt)  
[Триады "предпосылка-вопрос-ответ" для предложений длиной 4 слова](https://github.com/Koziev/NLP_Datasets/blob/master/QA/premise_question_answer5.txt)  

Пример данных в вышеуказанных файлах:

```
T: Собственник заключает договор аренды
Q: собственник заключает что?
A: договор аренды

T: Спереди стоит защитное бронестекло
Q: где защитное бронестекло стоит?
A: спереди
```

Каждая группа предпосылка-вопрос-ответ отделена пустыми строками. Перед предпосылкой стоит
метка T:, перед вопросом метка Q:, перед ответом метка A:

## Леммы

[Датасет с леммами](https://github.com/Koziev/NLP_Datasets/blob/master/Lemmas/Data/word2lemma.7z)

В архиве - список словоформ и их лемм, взятый из [Грамматического Словаря Русского Языка](https://github.com/Koziev/GrammarEngine).
Некоторое количество (несколько процентов) слов имеют неоднозначную лемматизацию,
например РОЙ - глагол РЫТЬ или существительное РОЙ. В таких случаях
нужно учитывать контекст слова. К примеру, так работает библиотека
для лемматизации [rulemma](https://github.com/Koziev/rulemma).


## NP chunking

[Датасет с разметкой](https://github.com/Koziev/NLP_Datasets/blob/master/NP_Chunker/chunker_train_NP.dat)

Датасет содержит предложения, в которых выделены NP-чанки. Первое поле в каждой записи
содержит метку принадлежности слова:  

0 - не принадлежит NP-чанку  
1 - начало NP-чанка  
2 - продолжение NP-чанка  

Разметка получена автоматической конвертацией из dependencies и может содержать
некоторые артефакты.


## Прочее

[Перестановочные перефразировки](https://github.com/Koziev/NLP_Datasets/tree/master/ParaphraseDetection)

[Частоты слов с учетом частей речи](https://github.com/Koziev/NLP_Datasets/tree/master/WordformFrequencies)

[Приведение слов к нейтральной форме "штучка-штука"](https://github.com/Koziev/NLP_Datasets/blob/master/Lemmas/Data/lemma2normal.dat)

[Корни слов](https://github.com/Koziev/NLP_Datasets/tree/master/Morph/wiki_roots.txt)
