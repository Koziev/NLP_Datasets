using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;


namespace SetSimilarity2
{
    public class Tools
    {
        public static double CalcJackardSim(HashSet<int> s1, HashSet<int> s2)
        {
            return s1.Intersect(s2).Count() / (float)(s1.Union(s2).Count());
        }

        public static string NormalizeStr(string str)
        {
            System.Text.StringBuilder b = new StringBuilder(str.Length);

            foreach (char c in str.Replace("  ", " ").ToUpper())
            {
                char c2 = c;
/*
                switch (c)
                {
                    case 'A': c2 = 'А'; break;
                    case 'B': c2 = 'В'; break;
                    case 'C': c2 = 'С'; break;
                    case 'E': c2 = 'Е'; break;
                    case 'H': c2 = 'Н'; break;
                    case 'K': c2 = 'К'; break;
                    case 'M': c2 = 'М'; break;
                    case 'O': c2 = 'О'; break;
                    case 'P': c2 = 'Р'; break;
                    case 'T': c2 = 'Т'; break;
                    case 'X': c2 = 'Х'; break;
                    case 'Y': c2 = 'У'; break;
                }
*/
                b.Append(c2);
            }

            return b.ToString();
        }
    }

    public class MinHash
    {
        private const int m_numHashFunctions = 100; //Modify this parameter
        private delegate int Hash(int index);
        private Hash[] m_hashFunctions;

        List<Tuple<uint, uint, uint>> abc_hash = new List<Tuple<uint, uint, uint>>();

        public MinHash(int universeSize)
        {
            Debug.Assert(universeSize > 0);

            m_hashFunctions = new Hash[m_numHashFunctions];

            Random r = new Random(11);
            for (int i = 0; i < m_numHashFunctions; i++)
            {
                uint a = (uint)r.Next(universeSize);
                uint b = (uint)r.Next(universeSize);
                uint c = (uint)r.Next(universeSize);
                m_hashFunctions[i] = x => QHash((uint)x, a, b, c, (uint)universeSize);
                abc_hash.Add(new Tuple<uint, uint, uint>(a, b, c));
            }
        }

        public string GetABC()
        {
            System.Text.StringBuilder b = new StringBuilder();

            int i=1;
            foreach (var abc in abc_hash)
            {
                b.Append("m_hashFunctions_a.EXTEND();\n");
                b.Append("m_hashFunctions_b.EXTEND();\n");
                b.Append("m_hashFunctions_c.EXTEND();\n");

                b.AppendFormat("m_hashFunctions_a({0}) := MOD({1}, universeSize);\n", i, abc.Item1);
                b.AppendFormat("m_hashFunctions_b({0}) := MOD({1}, universeSize);\n", i, abc.Item2);
                b.AppendFormat("m_hashFunctions_c({0}) := MOD({1}, universeSize);\n", i, abc.Item3);

                i++;
            }

            return b.ToString();
        }


        private static int QHash(uint x, uint a, uint b, uint c, uint bound)
        {
            //            int hashValue = (int)((a * (x >> 4) + b * x + c) & 131071);
            int hashValue = (int)((a * (x >> 4) + b * x + c) % bound);
            return Math.Abs(hashValue);
        }

        public int[,] GetSignatureMatrix(List<HashSet<int>> sets)
        {
            int[,] SIG = new int[sets.Count, m_numHashFunctions];

            for (int iset = 0; iset < sets.Count; ++iset)
            {
                // Строим минхэши для документа iset
                HashSet<int> S = sets[iset];
                for (int ihash = 0; ihash < m_numHashFunctions; ++ihash)
                {
                    int not_null_pos = int.MaxValue;
                    foreach (int index in S)
                    {
                        int permutated_index = m_hashFunctions[ihash](index);
                        if (permutated_index < not_null_pos)
                        {
                            not_null_pos = permutated_index;
                        }
                    }

                    SIG[iset, ihash] = not_null_pos;
                }
            }

            return SIG;
        }

        public double ComputeSimilarity(int[,] SIG, int iset1, int iset2)
        {
            int eq = 0;
            for (int ihash = 0; ihash < m_numHashFunctions; ++ihash)
            {
                if (SIG[iset1, ihash] == SIG[iset2, ihash])
                {
                    eq++;
                }
            }


            return eq / (float)m_numHashFunctions;
        }
    }


    public class LSH
    {
        int m_numHashFunctions;
        int m_numBands;
        List<HashSet<int>> m_sets;
        int[,] m_minHashMatrix;
        const int ROWSINBAND = 5;
        //Dictionary<int, HashSet<int>> m_lshBuckets = new Dictionary<int, HashSet<int>>();
        Dictionary<int, HashSet<int>>[] m_lshBuckets;

        public LSH(int[,] minHashMatrix, List<HashSet<int>> sets)
        {
            m_numHashFunctions = minHashMatrix.Length / sets.Count;
            m_numBands = m_numHashFunctions / ROWSINBAND;
            m_sets = sets;
            m_minHashMatrix = minHashMatrix;

            m_lshBuckets = new Dictionary<int, HashSet<int>>[m_numBands];
            for (int iband = 0; iband < m_numBands; ++iband)
            {
                m_lshBuckets[iband] = new Dictionary<int, HashSet<int>>();
            }


            for (int s = 0; s < sets.Count; s++)
            {
                for (int b = 0; b < m_numBands; b++)
                {
                    //combine all 5 MH values and then hash get its hashcode
                    //need not be sum
                    int sum = 0;

                    for (int i = 0; i < ROWSINBAND; i++)
                    {
                        sum += minHashMatrix[s, b * ROWSINBAND + i];
                    }

                    if (m_lshBuckets[b].ContainsKey(sum))
                    {
                        m_lshBuckets[b][sum].Add(s);
                    }
                    else
                    {
                        var set = new HashSet<int>();
                        set.Add(s);
                        m_lshBuckets[b].Add(sum, set);
                    }
                }
            }
        }

        public List<int> FindClosest(int setIndex, MinHash minHasher, double treshold)
        {
            //First find potential "close" candidates
            HashSet<int> potentialSetIndexes = new HashSet<int>();

            for (int b = 0; b < m_numBands; b++)
            {
                //combine all 5 MH values and then hash get its hashcode
                int sum = 0;

                for (int i = 0; i < ROWSINBAND; i++)
                {
                    sum += m_minHashMatrix[setIndex, b * ROWSINBAND + i];
                }

                foreach (var i in m_lshBuckets[b][sum])
                {
                    potentialSetIndexes.Add(i);
                }
            }


            List<int> result_list = new List<int>();
            foreach (int candidateIndex in potentialSetIndexes.Where(i => i != setIndex))
            {
                double similarity = minHasher.ComputeSimilarity(m_minHashMatrix, setIndex, candidateIndex);
                if (similarity > treshold)
                {
                    result_list.Add(candidateIndex);
                }
            }

            return result_list;
        }

    }

}