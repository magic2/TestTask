using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Concurrent;
using TestTask.Configuration;
using TestTask.Business.Interfaces;
using System.Globalization;
using System.Threading.Tasks;

namespace TestTask.Business
{
    internal class Reader : IReader
    {
        private class ChunkData
        {
            public long StartOffset { get; set; }
            public long Count { get; set; }
            public Stream Stream { get; set; }
            public StreamReader StreamReader { get; set; }

            public long HaveRead { get; set; }
        }

        public IDictionary<string, long> Read(Stream[] streams)
        {
            var dictionary = new ConcurrentDictionary<string, long>(StringComparer.Create(CultureInfo.InvariantCulture, true)); 

            var chunks = new ChunkData[streams.Length];
            var chunkLength = streams[0].Length / streams.Length;

            for(var i = 0; i < streams.Length; i++)
            {
                chunks[i] = new ChunkData
                {
                    Stream = streams[i],
                    StartOffset = i * chunkLength,
                    Count = chunkLength,
                    StreamReader = new StreamReader(streams[i], Config.DefaultEncoding),
                };
            }
            chunks.Last().Count = chunkLength + (streams[0].Length - (chunkLength * chunks.Length));

            PrepareStartOffsets(chunks);
            ReadWords(dictionary, chunks);
            
            return dictionary;
        }

        private void PrepareStartOffsets(ChunkData[] chunks)
        {
            Parallel.ForEach(chunks, (value, loopState, index) =>
                {
                    var stream = value.Stream;
                    stream.Position = value.StartOffset;

                    if (index > 0)
                    {
                        int offset = 0;
                        while (!value.StreamReader.EndOfStream)
                        {
                            var ch = (char)value.StreamReader.Read();
                            offset++;

                            if (IsSeparator(ch))
                            {
                                break;
                            }
                        }
                        chunks[index - 1].Count += offset;
                    }
                }
                );
        }

        private void ReadWords(ConcurrentDictionary<string, long> dictionary, ChunkData[] chunks)
        {
            Parallel.ForEach(chunks, (value, loopState, index) =>
                {
                    var list = new List<char>();
                    while (value.HaveRead < value.Count && !value.StreamReader.EndOfStream)
                    {
                        var ch = (char)value.StreamReader.Read();
                        value.HaveRead++;

                        if (!IsSeparator(ch))
                        {
                            list.Add(ch);
                            continue;
                        }
                        else
                        {
                            AddWord(list, dictionary);
                        }
                    }
                    AddWord(list, dictionary);
                }
                );
        }

        public IDictionary<string, long> Read(Stream stream)
        {
            var streamReader = new StreamReader(stream, Config.DefaultEncoding);
            var dictionary = new ConcurrentDictionary<string, long>(StringComparer.Create(CultureInfo.InvariantCulture, true));

            var list = new List<char>();
            while (!streamReader.EndOfStream)
            {
                var ch = (char)streamReader.Read();
                if (!IsSeparator(ch))
                {
                    list.Add(ch);
                    continue;
                }
                else
                {
                    AddWord(list, dictionary);
                }
            }
            AddWord(list, dictionary);
            return dictionary;
        }

        private void AddWord(List<char> list, ConcurrentDictionary<string, long> dictionary)
        {
            if (list.Count > 0)
            {
                var word = string.Join("", list).Trim().Replace("\0", "");
                if (word.Length > 0)
                {
                    dictionary.AddOrUpdate(word, 1, (key, count) => count + 1);
                }
                list.Clear();
            }   
        }

        private bool IsSeparator(char ch)
        {
            return ch == ' ' || ch == '\n' || ch == '\r';
        }
    }
}
