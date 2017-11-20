using System;
using IisLogArchiver.FileHandling;
using IisLogArchiver.Interfaces;
using Moq;
using NUnit.Framework;

namespace IisLogArchiverTests
{
    [TestFixture]
    public class FileGathererTests
    {
        private FileGatherer cut;
        private Mock<IDirectoryProvider> _mockDirectoryProvider;
        private Mock<IFileNameParser> _mockFileNameParser;

        private DateTime _testDate;
        private string _testFolderPath;
        private string[] _testLogFilePaths;


        [SetUp]
        public void SetUp()
        {
            _mockDirectoryProvider = new Mock<IDirectoryProvider>();
            _mockFileNameParser = new Mock<IFileNameParser>();

            _testDate = new DateTime(2017, 6, 9);
            _testFolderPath = @"m:\u_ex170608.log";
            _testLogFilePaths = new[]
            {
                @"m:\u_ex170608.log",
                @"m:\u_ex170608.txt",
                @"m:\u_ex20170608.log"
            };

            _mockDirectoryProvider
                .Setup(dp => dp.GetFiles(It.Is<string>(s => s == _testFolderPath)))
                .Returns(_testLogFilePaths);

            cut = new FileGatherer(_mockDirectoryProvider.Object, _mockFileNameParser.Object);
        }

        [Test]
        public void FilesOlderThan_DirectoryProviderIsCalledForFiles_CalledOnce()
        {

            cut.FilesOlderThan(_testFolderPath, _testDate);

            _mockDirectoryProvider.Verify(dp => dp.GetFiles(It.Is<string>(s => s == _testFolderPath)), Times.Once);
        }

        [Test]
        public void FilesOlderThan_CannotParseAnyDateFromAnyLogFile_NoneToBeReturned()
        {
            var fileDate = _testDate;
            _mockFileNameParser
                .Setup(fnp => fnp.TryParseDateFromString(It.IsAny<string>(), out fileDate))
                .Returns(false);

            var validLogfiles = cut.FilesOlderThan(_testFolderPath, _testDate);

            Assert.AreEqual(0, validLogfiles.Count);
        }

        [Test]
        public void FilesOlderThan_AllLogsFilesAreFromToday_NoneToBeReturned()
        {
            var fileDate = _testDate.AddDays(1);
            _mockFileNameParser
                .Setup(fnp => fnp.TryParseDateFromString(It.IsAny<string>(), out fileDate))
                .Returns(true);

            var validLogfiles = cut.FilesOlderThan(_testFolderPath, _testDate);

            Assert.AreEqual(0, validLogfiles.Count);
        }

        [Test]
        public void FilesOlderThan_AllLogsFilesAreFromYesterday_AllToBeReturned()
        {
            var fileDate = _testDate.AddDays(-1);
            _mockFileNameParser
                .Setup(fnp => fnp.TryParseDateFromString(It.IsAny<string>(), out fileDate))
                .Returns(true);

            var validLogfiles = cut.FilesOlderThan(_testFolderPath, _testDate);

            Assert.AreEqual(_testLogFilePaths.Length, validLogfiles.Count);
        }
    }
}
