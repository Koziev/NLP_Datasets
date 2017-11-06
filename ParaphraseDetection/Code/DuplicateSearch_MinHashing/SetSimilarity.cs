using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;


namespace SetSimilarity
{
    class MinHash
    {
        private const int m_numHashFunctions = 100; //Modify this parameter
        private delegate int Hash(int index);
        private Hash[] m_hashFunctions;

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
            }
        }

        public double Similarity<T>(HashSet<T> set1, HashSet<T> set2)
        {
            Debug.Assert(set1.Count > 0 && set2.Count > 0);

            int numSets = 2;
            Dictionary<T, bool[]> bitMap = BuildBitMap(set1, set2);

            int[,] minHashValues = GetMinHashSlots(numSets, m_numHashFunctions);

            ComputeMinHashForSet(set1, 0, minHashValues, bitMap);
            ComputeMinHashForSet(set2, 1, minHashValues, bitMap);

            return ComputeSimilarityFromSignatures(minHashValues, m_numHashFunctions);
        }

        private void ComputeMinHashForSet<T>(HashSet<T> set, short setIndex, int[,] minHashValues, Dictionary<T, bool[]> bitArray)
        {
            int index = 0;
            foreach (T element in bitArray.Keys)
            {
                for (int i = 0; i < m_numHashFunctions; i++)
                {
                    if (set.Contains(element))
                    {
                        int hindex = m_hashFunctions[i](index);

                        if (hindex < minHashValues[setIndex, i])
                        {
                            minHashValues[setIndex, i] = hindex;
                        }
                    }
                }
                index++;
            }
        }

        private static int[,] GetMinHashSlots(int numSets, int numHashFunctions)
        {
            int[,] minHashValues = new int[numSets, numHashFunctions];

            for (int i = 0; i < numSets; i++)
            {
                for (int j = 0; j < numHashFunctions; j++)
                {
                    minHashValues[i, j] = Int32.MaxValue;
                }
            }
            return minHashValues;
        }

        private static int QHash(uint x, uint a, uint b, uint c, uint bound)
        {
            //Modify the hash family as per the size of possible elements in a Set
            int hashValue = (int)((a * (x >> 4) + b * x + c) & 131071);
            return Math.Abs(hashValue);
        }

        private static Dictionary<T, bool[]> BuildBitMap<T>(HashSet<T> set1, HashSet<T> set2)
        {
            Dictionary<T, bool[]> bitArray = new Dictionary<T, bool[]>();
            foreach (T item in set1)
            {
                bitArray.Add(item, new bool[2] { true, false });
            }

            foreach (T item in set2)
            {
                bool[] value;
                if (bitArray.TryGetValue(item, out value))
                {
                    //item is present in set1
                    bitArray[item] = new bool[2] { true, true };
                }
                else
                {
                    //item is not present in set1
                    bitArray.Add(item, new bool[2] { false, true });
                }
            }
            return bitArray;
        }

        private static double ComputeSimilarityFromSignatures(int[,] minHashValues, int numHashFunctions)
        {
            int identicalMinHashes = 0;
            for (int i = 0; i < numHashFunctions; i++)
            {
                if (minHashValues[0, i] == minHashValues[1, i])
                {
                    identicalMinHashes++;
                }
            }
            return (1.0 * identicalMinHashes) / numHashFunctions;
        }

        public static int[,] GetMinHashSlots(int numSets)
        {
            return GetMinHashSlots(numSets, m_numHashFunctions);
        }

    }



    class LSH<T>
    {
        int m_numHashFunctions;
        int m_numBands;
        Dictionary<int, HashSet<int>> m_lshBuckets = new Dictionary<int, HashSet<int>>();
        HashSet<T>[] m_sets;
        int[,] m_minHashMatrix;
        const int ROWSINBAND = 5;

        //first index is Set, second index contains hashValue (so index is hash function)
        public LSH(int[,] minHashMatrix, HashSet<T>[] sets)
        {
            m_numHashFunctions = minHashMatrix.Length / sets.Length;
            m_numBands = m_numHashFunctions / ROWSINBAND;
            m_sets = sets;
            m_minHashMatrix = minHashMatrix;

            for (int s = 0; s < sets.Length; s++)
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

                    if (m_lshBuckets.ContainsKey(sum))
                    {
                        m_lshBuckets[sum].Add(s);
                    }
                    else
                    {
                        var set = new HashSet<int>();
                        set.Add(s);
                        m_lshBuckets.Add(sum, set);
                    }
                }
            }
        }

/*
        public int FindClosest(int setIndex, MinHash minHasher)
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

                foreach (var i in m_lshBuckets[sum])
                {
                    potentialSetIndexes.Add(i);
                }
            }

            //From the candidates compute similarity using min-hash and find the index of the closet set
            int minIndex = -1;
            double similarityOfMinIndex = 0.0;
            foreach (int candidateIndex in potentialSetIndexes.Where(i => i != setIndex))
            {
                double similarity = minHasher.ComputeSimilarity(m_minHashMatrix, setIndex, candidateIndex);
                if (similarity > similarityOfMinIndex)
                {
                    similarityOfMinIndex = similarity;
                    minIndex = candidateIndex;
                }
            }

            return minIndex;
        }*/

    }
}