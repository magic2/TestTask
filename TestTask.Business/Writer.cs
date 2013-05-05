using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestTask.Business.Interfaces;
using System.IO;
using TestTask.Configuration;

namespace TestTask.Business
{
    internal class Writer : IWriter, IDisposable
    {
        private StreamWriter streamWriter;

        public void Write(Stream stream, IEnumerable<KeyValuePair<string, long>> dictionary)
        {
            streamWriter = new StreamWriter(stream, Config.DefaultEncoding);
            
            foreach (var pair in dictionary)
            {
                streamWriter.WriteLine("{0},{1}", pair.Key, pair.Value);
            }
            streamWriter.Flush();
        }

        public void Dispose()
        {
            if (streamWriter != null)
            {
                streamWriter.Dispose();
            }
        }
    }
}
