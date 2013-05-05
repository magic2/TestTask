using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestTask.DataAccess.Exceptions
{
    public class FileException : ApplicationException
    {
        public FileException(string p) : base(p)
        {
        }
    }
}
