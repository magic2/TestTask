using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestTask.Business.Interfaces
{
    public interface IProcessor
    {
        Result Process(string inputFilePath, string outputFilePath);
    }
}
