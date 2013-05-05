using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TestTask.Configuration
{
    public static class Config
    {
        private static Encoding defaultEncoding;
        static Config()
        {
            defaultEncoding = Encoding.GetEncoding(int.Parse(ConfigurationManager.AppSettings["DefaultEncoding"]));
        }

        public static Encoding DefaultEncoding
        {
            get
            {
                return defaultEncoding;
            }
        }
    }
}
