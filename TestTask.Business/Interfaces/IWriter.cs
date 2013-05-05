using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestTask.Business.Interfaces
{
    public interface IWriter
    {
        void Write(Stream stream, IEnumerable<KeyValuePair<string, long>> dictionary);
    }
}
