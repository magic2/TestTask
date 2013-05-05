using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestTask.DataAccess
{
    public static class Helpers
    {
        public static bool IsFilePathValid(string path)
        {
            if (path.Trim() == string.Empty)
            {
                return false;
            }

            string pathname;
            string filename;
            try
            {
                pathname = Path.GetPathRoot(path);
                filename = Path.GetFileName(path);
            }
            catch (ArgumentException)
            {
                return false;
            }
            if (filename.Trim() == string.Empty)
            {
                return false;
            }
            if (pathname.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return false;
            }
            if (filename.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return false;
            }

            return true;
        }
    }
}
