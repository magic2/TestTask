using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestTask.DataAccess.Interfaces
{
    public interface IFile
    {
        Stream[] GetStreams(string filePath);
    }
}
