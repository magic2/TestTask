using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestTask.Tests
{
    public class Generators
    {
        private static Random r = new Random();
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * r.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static int RandomInt()
        {
            return r.Next();
        }
    }
}
