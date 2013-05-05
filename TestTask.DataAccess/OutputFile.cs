using System;
using TestTask.DataAccess.Exceptions;
using System.Security;
using System.Security.Permissions;
using System.IO;
using TestTask.DataAccess.Interfaces;

namespace TestTask.DataAccess
{
    public class OutputFile : IFile
    {
        public void Validate(string path)
        {
            if (!Helpers.IsFilePathValid(path))
            {
                throw new FileException("<Output file path> isn't valid.");
            }

            try
            {
                new FileIOPermission(FileIOPermissionAccess.Write, path);
            }
            catch (SecurityException)
            {
                throw new FileException("<Output file> isn't accessible.");
            }
        }

        public Stream[] GetStreams(string path)
        {
            Validate(path);
            return new Stream[] { new FileStream(path, FileMode.Create, FileAccess.Write) };
        }
    }
}
