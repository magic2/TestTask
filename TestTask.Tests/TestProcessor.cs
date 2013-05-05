using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestTask.Business;
using Ninject;
using Ninject.Modules;
using TestTask.DataAccess.Interfaces;
using TestTask.DataAccess.Attributes;
using TestTask.Business.Interfaces;
using System.IO;
using Moq;
using Ninject.MockingKernel.Moq;
using TestTask.DataAccess;
using TestTask.Configuration;
using TestTask.DataAccess.Exceptions;

namespace TestTask.Tests
{
    [TestClass]
    public class TestProcessor
    {
        private class TestFile : IFile
        {
            private string path;
            public string GetPath()
            {
                return path;
            }
            public Stream[] GetStreams(string filePath)
            {
                path = filePath;
                return new Stream[] { new MemoryStream() };
            }
        }

        private readonly MoqMockingKernel kernel;

        public TestProcessor()
        {
            this.kernel = new MoqMockingKernel();
            this.kernel.Bind<IProcessor>().To<Processor>();
        }

        [TestInitialize]
        public void SetUp()
        {
            this.kernel.Reset(); 
        }

        [TestMethod]
        public void TestProcess()
        {
            var inputPath = Generators.RandomString(255);
            var outputPath = Generators.RandomString(255);

            var readerStreams = new Stream[] { new MemoryStream() };
            var readerMock = this.kernel.GetMock<IReader>();

            var line = new KeyValuePair<string, long>(Generators.RandomString(10), Generators.RandomInt());

            var writerStreams = new Stream[] { new MemoryStream() } ;
            var dictionary = new List<KeyValuePair<string, long>>() { line };
            var writerMock = this.kernel.GetMock<IWriter>();

            readerMock.Setup(mock => mock.Read(readerStreams)).Returns(new Dictionary<string, long>() { { line.Key, line.Value} });
            writerMock.Setup(mock => mock.Write(writerStreams[0], dictionary));

            var inputFileMock = this.kernel.GetMock<IFile>();
            inputFileMock.Setup(mock => mock.GetStreams(inputPath)).Returns(readerStreams);

            var outputFileMock = this.kernel.GetMock<IFile>();
            outputFileMock.Setup(mock => mock.GetStreams(outputPath)).Returns(writerStreams);

            this.kernel.Bind<IFile>().ToMethod(x => inputFileMock.Object).WhenTargetHas<InputFileAttribute>();
            this.kernel.Bind<IFile>().ToMethod(x => outputFileMock.Object).WhenTargetHas<OutputFileAttribute>();

            var processor = kernel.Get<IProcessor>();

            var result = processor.Process(inputPath, outputPath);

            readerMock.VerifyAll();
            writerMock.VerifyAll();
            inputFileMock.VerifyAll();
            outputFileMock.VerifyAll();

            Assert.AreEqual(1, result.WordsCount);
            Assert.AreEqual(line.Value, result.MaximumNumber);
        }

        [TestMethod]
        [ExpectedException(typeof(Business.Exceptions.BusinessException))]
        public void TestProcessThrowException()
        {
            var inputPath = Generators.RandomString(255);
            var outputPath = Generators.RandomString(255);

            var readerStreams = new Stream[] { new MemoryStream() };
            var readerMock = this.kernel.GetMock<IReader>();

            var line = new KeyValuePair<string, long>(Generators.RandomString(10), Generators.RandomInt());

            var writerStreams = new Stream[] { new MemoryStream() };
            var dictionary = new List<KeyValuePair<string, long>>() { line };
            var writerMock = this.kernel.GetMock<IWriter>();

            readerMock.Setup(mock => mock.Read(readerStreams)).Returns(new Dictionary<string, long>() { { line.Key, line.Value } });
            writerMock.Setup(mock => mock.Write(writerStreams[0], dictionary));

            var inputFileMock = this.kernel.GetMock<IFile>();
            var message = Generators.RandomString(255);
            inputFileMock.Setup(mock => mock.GetStreams(inputPath)).Throws(new FileException(message));

            var outputFileMock = this.kernel.GetMock<IFile>();
            outputFileMock.Setup(mock => mock.GetStreams(outputPath)).Returns(writerStreams);

            this.kernel.Bind<IFile>().ToMethod(x => inputFileMock.Object).WhenTargetHas<InputFileAttribute>();
            this.kernel.Bind<IFile>().ToMethod(x => outputFileMock.Object).WhenTargetHas<OutputFileAttribute>();

            var processor = kernel.Get<IProcessor>();

            var result = processor.Process(inputPath, outputPath);
        }
    }
}
