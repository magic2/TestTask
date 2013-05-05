using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestTask.DataAccess.Exceptions;

namespace TestTask.Business.Exceptions
{
    public class BusinessException : ApplicationException
    {
        public BusinessException(FileException ex)
            : base(ex.Message, ex)
        {
        }
    }
}
