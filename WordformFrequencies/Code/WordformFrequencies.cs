using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WordformFrequencies
{
    class WordformFrequencies
    {
        static void Main(string[] args)
        {
            Dictionary<string, int> term2freq = new Dictionary<string, int>();

            string solarix_folder = args[0];
            foreach (string xml_path in System.IO.Directory.GetFiles(solarix_folder, "*.parsing.txt"))
            {
                Console.WriteLine($"Processing {xml_path}");
                System.Text.StringBuilder b = new StringBuilder();

                using (System.IO.StreamReader rdr = new System.IO.StreamReader(xml_path))
                {
                    while (!rdr.EndOfStream)
                    {
                        string line = rdr.ReadLine();

                        b.AppendFormat("{0}\r\n", line);

                        if (line.StartsWith("<sentence"))
                        {
                            b.Length = 0;
                            b.Append(line);
                        }
                        else if (line == "</sentence>")
                        {
                            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                            doc.LoadXml(b.ToString());

                            foreach (XmlNode n in doc.DocumentElement.SelectNodes("tokens/token"))
                            {
                                string word = n.SelectSingleNode("word").InnerText.ToLower();
                                string lemma = n.SelectSingleNode("lemma").InnerText;
                                string part_of_speech = n.SelectSingleNode("part_of_speech").InnerText;

                                string key = word + "|" + part_of_speech;
                                int freq;
                                if (term2freq.TryGetValue(key, out freq))
                                {
                                    term2freq[key] = freq + 1;
                                }
                                else
                                {
                                    term2freq[key] = 1;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"Done, {term2freq.Count} records in frequency table");
            }

            using (System.IO.StreamWriter wrt = new System.IO.StreamWriter(args[1]))
            {
                foreach (var q in term2freq.OrderByDescending(z => z.Value))
                {
                    var t = q.Key.Split('|');
                    wrt.WriteLine("{0}\t{1}\t{2}", t[0], t[1], q.Value);
                }
            }

            return;
        }
    }
}
