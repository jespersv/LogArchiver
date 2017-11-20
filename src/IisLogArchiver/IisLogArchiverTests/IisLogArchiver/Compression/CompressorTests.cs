using IisLogArchiver.Compression;
using IisLogArchiver.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace IisLogArchiverTests
{
    [TestFixture]
    public class CompressorTests
    {
        private Mock<ICompressProcessFactory> _mockCompressProcessFactory;
        private Mock<IFileDeleter> _mockFileDeleter;
        private Mock<IProcessStarter> _mockProcessStarter;

        private string _testArchivePath;
        private string[] _testLogFilePaths;
        private Compressor cut;
        private Mock<IArchiveSettings> _mockArchiverSettings;

        [SetUp]
        public void SetUp()
        {
            _mockCompressProcessFactory = new Mock<ICompressProcessFactory>();
            _mockFileDeleter = new Mock<IFileDeleter>();
            _mockProcessStarter = new Mock<IProcessStarter>();
            _mockArchiverSettings = new Mock<IArchiveSettings>();

            _testArchivePath = @"m:\archive\archive.7z";
            _testLogFilePaths = new[]
            {
                @"m:\f1.log",
                @"m:\f2.log",
                @"m:\f3.log"
            };

            cut = new Compressor(_mockCompressProcessFactory.Object, _mockFileDeleter.Object, _mockProcessStarter.Object, _mockArchiverSettings.Object);
        }

        [Test]
        public void Compress_InvalidCompressToFileInput_Throws()
        {

            Assert.Throws<ArgumentException>(() => cut.Compress("", ""));
            Assert.Throws<ArgumentException>(() => cut.Compress("valid", null));
            Assert.Throws<ArgumentException>(() => cut.Compress("valid", new string[0]));
        }

        [Test]
        public void Compress_ArchiveCompressProcessIsCreated_ArchivePathSentToFactory()
        {
            cut.Compress(_testArchivePath, _testLogFilePaths);

            _mockCompressProcessFactory.Verify(
                cpf => cpf.CreateCompressProcessInfo(It.Is<string>(s => s == _testArchivePath), It.IsAny<string>()));
        }

        [Test]
        public void Compress_ProcessInfoFromFactory_IsUsedInTheProcessStarter()
        {
            var mockStartInfo = new Mock<ICustomProcessStartInfo>();
            _mockCompressProcessFactory
                .Setup(cpf => cpf.CreateCompressProcessInfo(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(mockStartInfo.Object);

            cut.Compress(_testArchivePath, _testLogFilePaths);

            _mockProcessStarter.Verify(
                cpf => cpf.Start(It.Is<ICustomProcessStartInfo>(s => s == mockStartInfo.Object), It.IsAny<ProcessPriorityClass>()));
        }

        [Test]
        public void Compress_ProcessStarterReturnsProcess_ProcessIsWaitedFor()
        {
            var mockProcesss = new Mock<ICustomProcess>();

            _mockProcessStarter
                .Setup(cpf => cpf.Start(It.IsAny<ICustomProcessStartInfo>(), It.IsAny<ProcessPriorityClass>()))
                .Returns(mockProcesss.Object);

            cut.Compress(_testArchivePath, _testLogFilePaths);

            mockProcesss.Verify(cpf => cpf.WaitForExit(), Times.Once);
        }

        [Test]
        public void Compress_ProcessThrowsException_ReturnsFalse()
        {
            _mockProcessStarter
                .Setup(cpf => cpf.Start(It.IsAny<ICustomProcessStartInfo>(), It.IsAny<ProcessPriorityClass>()))
                .Throws<Exception>();

            var exceptionOccurred = cut.Compress(_testArchivePath, _testLogFilePaths);

            Assert.IsTrue(exceptionOccurred);
        }

        [Test]
        public void Compress_ProcessThrowsException_NewArchiveFileIsDeleted()
        {
            _mockProcessStarter
                .Setup(cpf => cpf.Start(It.IsAny<ICustomProcessStartInfo>(), It.IsAny<ProcessPriorityClass>()))
                .Throws<Exception>();

            cut.Compress(_testArchivePath, _testLogFilePaths);

            _mockFileDeleter.Verify(fd => fd.TryDeleteFile(It.Is<string>(s => s == _testArchivePath), true));
        }
    }
}
