import codecs
import os

for infile in ['mutual_info_2_ru.dat', 'mutual_info_3_ru.dat', 'mutual_info_4_ru.dat']:
    with codecs.open(infile, 'r', 'utf-8') as rdr:
        with codecs.open(os.path.basename(infile)+'-head.tsv', 'w', 'utf-8') as wrt:
            for cnt, line in enumerate(rdr):
                wrt.write(line)
                if cnt>100000:
                    break
