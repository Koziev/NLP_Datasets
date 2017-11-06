/*
 * В корпусе текстов ищем перестановочные пары предложений.
 * Группы таких перестановочных перефразировок сохраняем в текстовом файле
 * для последующего использовании при тренировки сеточного детектора перефразировок.
 * Формат корпуса - utf-8, одна строка содержит одно предложение.
 * */

using System;
using System.Collections.Generic;
using System.Security.Cryptography;


class Program
{
    static List<string> StringToWords(string str)
    {
        StringParser.StringTokenizer tokenizer = new StringParser.StringTokenizer(str);

        List<string> res = new List<string>();

        while (tokenizer.eof() == false)
        {
            string w = tokenizer.read().ToLower();

            if (w == "не" || w == "ни")
            {
                string w2 = w + " " + tokenizer.read().ToLower();
                res.Add(w2);
            }
            else
            {
                res.Add(w);
            }
        }

        return res;
    }


    static bool SetsAreEqual(List<string> set1, List<string> set2)
    {
        if (set1.Count != set2.Count)
        {
            return false;
        }

        foreach (string w1 in set1)
        {
            if (!set2.Contains(w1))
            {
                return false;
            }
        }

        foreach (string w2 in set2)
        {
            if (!set1.Contains(w2))
            {
                return false;
            }
        }


        return true;
    }

    static string NormalizeSent(string s)
    {
        string res = s.ToLower();

        while (char.IsPunctuation(res[res.Length - 1]))
        {
            res = res.Substring(0, res.Length - 1);
        }

        return res;
    }

    static void Main(string[] args)
    {
        // Путь к исходному тексту
        string SENTx_path = args[0];

        // Путь к файлу, куда будем записывать дубликаты
        string result_path = args[1];

        // Пороги похожести, зависят от длины предложений.
        double threshold2 = double.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture);
        double threshold1 = threshold2 * 0.8;

        Console.WriteLine("threshold1={0} threshold2={1}", threshold1, threshold2);

        // Макс. число предложений
        int max_sent = int.Parse(args[3]);

        HashSet<long> sample_hashes = new HashSet<long>(); // для устранения повторов в результатах
        MD5 md5 = MD5.Create();


        DateTime started = DateTime.Now;

        int n_groups = 0;

        using (System.IO.StreamWriter wrt = new System.IO.StreamWriter(result_path))
        {
            // Загружаем предложения в vals
            Console.WriteLine("Processing {0}...", SENTx_path);

            List<string> vals = new List<string>();

            using (System.IO.StreamReader rdr = new System.IO.StreamReader(SENTx_path))
            {
                while (!rdr.EndOfStream && vals.Count <= max_sent)
                {
                    string line = rdr.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    line = NormalizeSent( line.Trim().Replace("  ", " ").Replace("  ", " ") );
                    vals.Add(line);
                }
            }

            Console.WriteLine("{0} lines in {1}", vals.Count, SENTx_path);

            // -----------------------------------------------------------------------

            Dictionary<string, int> shingle2id = new Dictionary<string, int>();
            List<HashSet<int>> val_sets = new List<HashSet<int>>();
            foreach (string v in vals)
            {
                string uv = SetSimilarity2.Tools.NormalizeStr(v);

                HashSet<int> v_set = new HashSet<int>();

                for (int i0 = 0; i0 < uv.Length - 3; ++i0)
                {
                    string shingle = uv.Substring(i0, 3);

                    int id = -1;
                    if (!shingle2id.TryGetValue(shingle, out id))
                    {
                        id = shingle2id.Count;
                        shingle2id.Add(shingle, id);
                    }

                    v_set.Add(id);
                }

                val_sets.Add(v_set);
            }

            /*
                            // --- отладка ---
                            {
                                int iset1 = vals.IndexOf("Ты меня слышишь?");
                                int iset2 = vals.IndexOf("Ты слышишь меня?");
                                double sim0 = SetSimilarity2.Tools.CalcJackardSim(val_sets[iset1], val_sets[iset2]);
                                Console.WriteLine("iset1={0} iset2={1} sim={2}", iset1, iset2, sim0);
                            }
                            // ---------------
            */

            Console.WriteLine("Hashing...");

            // ------------------------------------------------------------------
            SetSimilarity2.MinHash minhash = new SetSimilarity2.MinHash(shingle2id.Count);

            // --- для отладки
            //string abc = minhash.GetABC();
            // ---------------

            int[,] SIG = minhash.GetSignatureMatrix(val_sets);

            double sim12 = minhash.ComputeSimilarity(SIG, 0, 1);

            // Решение с использованием Local Sensitiviy Hash
            SetSimilarity2.LSH lsh = new SetSimilarity2.LSH(SIG, val_sets);

            Console.WriteLine("Searching duplicates...");

            List<Tuple<int, int>> sim_pairs = new List<Tuple<int, int>>();
            for (int iset1 = 0; iset1 < val_sets.Count; ++iset1)
            {
                List<int> sets2 = lsh.FindClosest(iset1, minhash, threshold1);

                List<int> isets_bucket = new List<int>();
                isets_bucket.Add(iset1);

                List<string> toks1 = StringToWords(vals[iset1]);

                foreach (int iset2 in sets2)
                {
                    if (iset2 > iset1)
                    {
                        double sim0 = SetSimilarity2.Tools.CalcJackardSim(val_sets[iset1], val_sets[iset2]);

                        if (sim0 > threshold2)
                        {
                            // Дополнительная проверка строк, в частности надо убедиться,
                            // что частица НЕ/НИ осталась перед тем же словом.

                            List<string> toks2 = StringToWords(vals[iset2]);

                            if (SetsAreEqual(toks1, toks2))
                            {
                                isets_bucket.Add(iset2);
                                //Console.WriteLine("\nistr1={0}\nistr2={1}\nstr1={2}\nstr2={3}\nsim={4:N5}", iset1, iset2, vals[iset1], vals[iset2], sim0);
                            }
                        }
                    }
                }

                if (isets_bucket.Count > 1)
                {
                    List<string> printed = new List<string>();

                    foreach (int isetx in isets_bucket)
                    {
                        string line = NormalizeSent(vals[isetx]);
                        if (!printed.Contains(line))
                        {
                            byte[] hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(line));
                            Int64 ihash1 = BitConverter.ToInt64(hash, 0);
                            Int64 ihash2 = BitConverter.ToInt64(hash, 8);
                            Int64 ihash = ihash1 ^ ihash2;

                            if (!sample_hashes.Contains(ihash))
                            {
                                sample_hashes.Add(ihash);
                                printed.Add(line);
                            }
                        }
                    }

                    if (printed.Count > 1)
                    {
                        foreach (string l in printed)
                        {
                            wrt.WriteLine("{0}", l);
                        }

                        wrt.WriteLine("\n");
                        n_groups++;
                    }
                }


                if ((iset1 % 1000) == 0)
                {
                    Console.Write("iset1={0}/{1} n_groups={2}\r", iset1, vals.Count, n_groups);
                }
            }

        }


        DateTime finished = DateTime.Now;
        Console.WriteLine("Done via LSH, elapsed time={0} sec", (finished - started).TotalSeconds);

        return;
    }
}
