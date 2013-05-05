using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestTask.Business.Interfaces
{
    public interface IReader
    {
        IDictionary<string, long> Read(Stream[] streams);
        IDictionary<string, long> Read(Stream stream);
    }
}
