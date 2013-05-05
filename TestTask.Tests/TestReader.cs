using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TestTask.Configuration;
using TestTask.Business;

namespace TestTask.Tests
{
    [TestClass]
    public class TestReader
    {
        private IDictionary<string, long> FillStream(StreamWriter writer, int numberOfWords = 100)
        {
            string[] input = new[] {
                Generators.RandomString(10),
                Generators.RandomString(20),
                Generators.RandomString(30),
                Generators.RandomString(5),
            };

            char[] separators = new[] { '-', ' ', '\n', '\r' };

            var r = new Random();

            var expected = new Dictionary<string, long>(StringComparer.InvariantCultureIgnoreCase);
        
            var word = string.Empty;
            var i = 0;
            do 
            {
                var selectedWord = input[r.Next(3)];
                var separator = separators[r.Next(3)];

                word += selectedWord;

                if (separator == ' ' || separator == '\n' || separator == '\r')
                {
                    if (expected.ContainsKey(word))
                    {
                        expected[word]++;
                    }
                    else
                    {
                        expected.Add(word, 1);
                    }
                    word = string.Empty;
                }
                else 
                {
                    word += separator;
                }

                writer.Write(selectedWord);
                writer.Write(separator);
                i++;
            }
            while (i < numberOfWords);

            if (word.Length > 0)
            {
                if (expected.ContainsKey(word))
                {
                    expected[word]++;
                }
                else
                {
                    expected.Add(word, 1);
                }
            }
            
            return expected;
        }

        [TestMethod]
        public void Read()
        {
            IDictionary<string, long> expected;
            IDictionary<string, long> actual;

            using (var ms = new MemoryStream()) 
            {
                using (var sw = new StreamWriter(ms, Config.DefaultEncoding))
                {
                    expected = FillStream(sw);
                    sw.Flush();
                    ms.Position = 0;

                    actual = new Reader().Read(new Stream[] { ms });
                }
            }

            Assert.AreEqual(expected.Count, actual.Count);
            foreach (var expectedPair in expected)
            {
                Assert.IsTrue(actual.ContainsKey(expectedPair.Key), string.Format("Can't find key: {0}", expectedPair.Key));
                Assert.AreEqual(expectedPair.Value, actual[expectedPair.Key]);
            }
        }

        [TestMethod]
        public void ReadMultipleStreams()
        {
            IDictionary<string, long> expected;
            IDictionary<string, long> actual;

            byte[] sourceData = new byte[10000];

            var ms = new MemoryStream(sourceData);
            var sw = new StreamWriter(ms, Config.DefaultEncoding);
            expected = FillStream(sw);
            sw.Flush();
            
            ms.Position = 0;

            var ms2 = new MemoryStream(sourceData);
            ms2.Position = 0;

            var ms3 = new MemoryStream(sourceData);
            ms3.Position = 0;

            actual = new Reader().Read(new Stream[] { ms, ms2, ms3 });

            Console.WriteLine(System.Text.Encoding.ASCII.GetString(sourceData));

            Assert.AreEqual(expected.Count, actual.Count);
            foreach (var expectedPair in expected)
            {
                Assert.IsTrue(actual.ContainsKey(expectedPair.Key), string.Format("Can't find key: {0}", expectedPair.Key));
                Assert.AreEqual(expectedPair.Value, actual[expectedPair.Key]);
            }

            sw.Dispose();
            ms.Dispose();
            ms2.Dispose();
            ms3.Dispose();
        }

        
        [TestMethod]
        public void ReadWithEmptyValues()
        {
            IDictionary<string, long> actual;
            IDictionary<string, long> expected;

            using (var ms = new MemoryStream())
            {
                var s1 = Generators.RandomString(10);
                var s2 = Generators.RandomString(10);

                using (var sw = new StreamWriter(ms, Config.DefaultEncoding))
                {
                    sw.WriteLine("{0}  {1}\n {0} {0}", s1, s2);
                    sw.Flush();
                    ms.Position = 0;

                    actual = new Reader().Read(new Stream[] { ms });
                }
                expected = new Dictionary<string, long>() {
                    {s1, 3},
                    {s2, 1},
                };
            }

            Assert.AreEqual(expected.Count, actual.Count);
            foreach (var expectedPair in expected)
            {
                Assert.IsTrue(actual.ContainsKey(expectedPair.Key), string.Format("Can't find key: {0}", expectedPair.Key));
                Assert.AreEqual(expectedPair.Value, actual[expectedPair.Key]);
            }
        }

        [TestMethod]
        public void ReadCaseInsensitive()
        {
            IDictionary<string, long> actual;
            IDictionary<string, long> expected;

            using (var ms = new MemoryStream())
            {
                using (var sw = new StreamWriter(ms, Config.DefaultEncoding))
                {
                    sw.Write("AAA  BBB\n AAA aAa AaA aaa");
                    sw.Flush();
                    ms.Position = 0;

                    actual = new Reader().Read(new Stream[] { ms });
                }
                expected = new Dictionary<string, long>(StringComparer.InvariantCultureIgnoreCase) {
                    {"AAA", 5},
                    {"BBB", 1},
                };
            }

            Assert.AreEqual(expected.Count, actual.Count);
            foreach (var actualPair in actual)
            {
                Assert.IsTrue(expected.ContainsKey(actualPair.Key), string.Format("Can't find key: {0} here: {1}", actualPair.Key, string.Join<string>(", ", expected.Keys)));
                Assert.AreEqual(expected[actualPair.Key], actualPair.Value);
            }
        }
    }
}
