using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestTask.DataAccess;
using TestTask.Business.Interfaces;
using Ninject;
using System.IO;
using TestTask.DataAccess.Interfaces;
using TestTask.DataAccess.Attributes;

namespace TestTask.Business
{
    public class Processor : IProcessor
    {
        internal IReader Reader { get; private set; }
        internal IWriter Writer { get; private set; }

        internal IFile InputFile { get; private set; }
        internal IFile OutputFile { get; private set; }

        private Processor()
        {
        }

        public Processor(IReader reader, IWriter writer, [InputFile] IFile inputFile, [OutputFile] IFile outputFile)
        {
            Reader = reader;
            Writer = writer;
            InputFile = inputFile;
            OutputFile = outputFile;
        }

        public Result Process(string inputFilePath, string outputFilePath)
        {
            try
            {
                var inputStreams = InputFile.GetStreams(inputFilePath);
                var outputStreams = OutputFile.GetStreams(outputFilePath);

                var dictionary = Reader.Read(inputStreams);

                var orderedDictionary = dictionary.OrderByDescending(pair => pair.Value);
                Writer.Write(outputStreams[0], orderedDictionary);

                return new Result { WordsCount = dictionary.Count, MaximumNumber = dictionary.Count == 0 ? 0 : orderedDictionary.First().Value };
            }
            catch (DataAccess.Exceptions.FileException ex)
            {
                throw new Business.Exceptions.BusinessException(ex);
            }
        }
    }
}
