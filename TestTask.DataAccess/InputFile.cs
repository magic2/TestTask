using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Permissions;
using System.Security;
using TestTask.DataAccess.Exceptions;
using TestTask.DataAccess.Interfaces;

namespace TestTask.DataAccess
{
    public class InputFile : IFile
    {
        private void Validate(string path)
        {
            if (!Helpers.IsFilePathValid(path))
            {
                throw new FileException("<Input file path> isn't valid.");
            }

            if (!File.Exists(path))
            {
                throw new FileException("<Input file> doesn't exist.");
            }

            try
            {
                new FileIOPermission(FileIOPermissionAccess.Read, path);
            }
            catch (SecurityException)
            {
                throw new FileException("<Input file> isn't accessible.");
            }
        }

        public Stream[] GetStreams(string path)
        {
            Validate(path);
            var fileStream = GetStream(path);
            if (Environment.ProcessorCount > 1)
            {
                var numOfStreams = Environment.ProcessorCount;
                var streams = new Stream[numOfStreams];
                streams[0] = fileStream;
                for(var i = 1; i < numOfStreams; i++) 
                {
                    streams[i] = GetStream(path);
                }
                return streams;
            }
            return new Stream[] { fileStream };
        }

        private Stream GetStream(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
