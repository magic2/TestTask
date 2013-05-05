using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TestTask.Business;
using TestTask.Configuration;

namespace TestTask.Tests
{
    [TestClass]
    public class TestWriter
    {
        [TestMethod]
        public void TestWrite()
        {
            using (var ms = new MemoryStream())
            {
                var dictionary = new List<KeyValuePair<string, long>>();
                dictionary.Add(new KeyValuePair<string, long>(Generators.RandomString(10), Generators.RandomInt()));
                dictionary.Add(new KeyValuePair<string, long>(Generators.RandomString(10), Generators.RandomInt()));
                dictionary.Add(new KeyValuePair<string, long>(Generators.RandomString(10), Generators.RandomInt()));

                new Writer().Write(ms, dictionary);

                ms.Position = 0;

                var lineNum = 0;
                using (var tr = new StreamReader(ms, Config.DefaultEncoding))
                {
                    while (!tr.EndOfStream)
                    {
                        var expected = string.Format("{0},{1}", dictionary[lineNum].Key, dictionary[lineNum].Value);
                        var actual = tr.ReadLine();
                        Assert.AreEqual(expected, actual);
                        lineNum++;
                    }
                }
            }
        }

        [TestMethod]
        public void TestEmptyWrite()
        {
            using (var ms = new MemoryStream())
            {
                var dictionary = new List<KeyValuePair<string, long>>();
                new Writer().Write(ms, dictionary);

                ms.Position = 0;

                using (var tr = new StreamReader(ms, Config.DefaultEncoding))
                {
                    Assert.IsTrue(tr.EndOfStream);
                }
            }
        }
    }
}
