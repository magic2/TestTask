using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestTask.DataAccess.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Parameter)]
    public class InputFileAttribute : Attribute
    {
    }
}
