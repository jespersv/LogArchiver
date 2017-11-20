using IisLogArchiver;
using IisLogArchiver.Core;
using IisLogArchiver.Interfaces;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IisLogArchiverTests
{
    [TestFixture]
    public class ArchiverTests
    {
        private Mock<IArchiveSettings> _mockSettings;
        private Mock<ITimeProvider> _mockTimeprovider;
        private Mock<ICompressor> _mockCompressor;
        private Mock<IFileDeleter> _mockFileDeleter;
        private Mock<IArchiveNameProvider> _mockArchiverNameProvider;
        private Mock<IFileGatherer> _mockFileGatherer;
        private Mock<IDirectoryProvider> _mockDirectoryProvider;
        private Mock<IFileBatchProvider> _mockFileBathProvider;

        private Archiver cut;

        private Archive[] _testArchives;
        private DateTime _testDate;
        private Archive _testArchive1;

        [SetUp]
        public void SetUp()
        {
            _mockSettings = new Mock<IArchiveSettings>();
            _mockTimeprovider = new Mock<ITimeProvider>();
            _mockCompressor = new Mock<ICompressor>();
            _mockFileDeleter = new Mock<IFileDeleter>();
            _mockArchiverNameProvider = new Mock<IArchiveNameProvider>();
            _mockFileGatherer = new Mock<IFileGatherer>();
            _mockDirectoryProvider = new Mock<IDirectoryProvider>();
            _mockFileBathProvider = new Mock<IFileBatchProvider>();


            _testDate = new DateTime(2017, 6, 12);
            _testArchive1 = new Archive
            {
                ArchiveName = "archivename",
                ArchivePath = @"x:\logs",
                DeleteArchivedFiles = true,
                FilePath = @"x:\logs\archive"
            };
            _testArchives = new[]
            {
                _testArchive1
            };
            _mockTimeprovider.SetupGet(tp => tp.Now).Returns(_testDate);

            var testLogs = new List<string>() { "logs1", "log2" };//...
            _mockFileGatherer
                .Setup(fg => fg.FilesOlderThan(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(testLogs);

            _mockFileBathProvider
                .Setup(fbp => fbp.Batch(It.IsAny<IEnumerable<string>>()))
                .Returns(new List<List<string>>{ testLogs });

            cut = new Archiver(
                _mockSettings.Object,
                _mockTimeprovider.Object,
                _mockCompressor.Object,
                _mockFileDeleter.Object,
                _mockArchiverNameProvider.Object,
                _mockFileGatherer.Object,
                _mockDirectoryProvider.Object,
                _mockFileBathProvider.Object
                );
        }

        [Test]
        public void ProcessLogsToArchives_EmptyArchives_DoesNotCallAnyProviders()
        {
            //cut.ProcessLogsToArchives(_testArchives);
            cut.ProcessLogsToArchives(new Archive[0]);

            _mockFileGatherer.Verify(fg => fg.FilesOlderThan(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            _mockArchiverNameProvider.Verify(an => an.DecideArchiveName(It.IsAny<Archive>(), It.IsAny<DateTime>()), Times.Never);
            _mockDirectoryProvider.Verify(dp => dp.CreateDirectory(It.IsAny<string>()), Times.Never);
            _mockDirectoryProvider.Verify(dp => dp.Exists(It.IsAny<string>()), Times.Never);
            _mockFileDeleter.Verify(fd => fd.TryDeleteFile(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
            _mockCompressor.Verify(c => c.Compress(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ProcessLogsToArchives_NoLogFilesFoundInArchive_OnlyFileGatherIsCalled()
        {
            _mockFileGatherer
                .Setup(fg => fg.FilesOlderThan(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(new List<string>());

            cut.ProcessLogsToArchives(_testArchives);

            _mockFileGatherer.Verify(fg => fg.FilesOlderThan(It.Is<string>(s => s.Equals(_testArchive1.FilePath)), It.IsAny<DateTime>()), Times.Once);

            _mockArchiverNameProvider.Verify(an => an.DecideArchiveName(It.IsAny<Archive>(), It.IsAny<DateTime>()), Times.Never);
            _mockDirectoryProvider.Verify(dp => dp.CreateDirectory(It.IsAny<string>()), Times.Never);
            _mockDirectoryProvider.Verify(dp => dp.Exists(It.IsAny<string>()), Times.Never);
            _mockFileDeleter.Verify(fd => fd.TryDeleteFile(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
            _mockCompressor.Verify(c => c.Compress(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ProcessLogsToArchives_ArchiveIsSentToNameProvider_ReturnsAString()
        {
            cut.ProcessLogsToArchives(_testArchives);

            _mockArchiverNameProvider.Verify(an => an.DecideArchiveName(It.Is<Archive>(a => a.Equals(_testArchive1)), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void ProcessLogsToArchives_ArchiveFolderDoesNotExists_TryCreateIt()
        {
            _mockDirectoryProvider.Setup(dp => dp.Exists(It.IsAny<string>())).Returns(false);
            cut.ProcessLogsToArchives(_testArchives);

            _mockDirectoryProvider.Verify(
                dp => dp.CreateDirectory(It.Is<string>(s => s.Equals(_testArchive1.ArchivePath))), Times.Once);
        }

        [Test]
        public void ProcessLogsToArchives_ArchiveFolderExists_SkippTryingCreatingIt()
        {
            _mockDirectoryProvider.Setup(dp => dp.Exists(It.IsAny<string>())).Returns(true);

            cut.ProcessLogsToArchives(_testArchives);

            _mockDirectoryProvider.Verify(
                dp => dp.CreateDirectory(It.Is<string>(s => s.Equals(_testArchive1.ArchivePath))), Times.Never);
        }

        [Test]
        public void ProcessLogsToArchives_FilesFoundByGatherer_IsSentToCompressor()
        {
            var testLogs = new List<string>() { "logs1", "log2" };//...
            _mockFileGatherer
                .Setup(fg => fg.FilesOlderThan(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(testLogs);

            cut.ProcessLogsToArchives(_testArchives);

            _mockCompressor.Verify(c => c.Compress(It.IsAny<string>(), It.Is<string[]>(s => s[0] == testLogs[0] && s[1] == testLogs[1])), Times.Once);
        }

        [Test]
        public void ProcessLogsToArchives_NameGeneratedByArchiveNameProvider_IsSentToCompressor()
        {
            var fancyTestname = "AnyName";
            _mockArchiverNameProvider
                .Setup(anp => anp.DecideArchiveName(It.IsAny<Archive>(), It.IsAny<DateTime>()))
                .Returns(fancyTestname);

            cut.ProcessLogsToArchives(_testArchives);

            _mockCompressor.Verify(c => c.Compress(It.Is<string>(s => s.Equals(fancyTestname)), It.IsAny<string[]>()), Times.Once);
        }

        [Test]
        public void ProcessLogsToArchives_CompressorReturnsNoError_TriesToDeleteLogs()
        {
            var testLogs = new List<string>() { "logs1", "log2" };//...
            _mockFileGatherer
                .Setup(fg => fg.FilesOlderThan(It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(testLogs);

            _mockCompressor
                .Setup(c => c.Compress(It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(false);

            cut.ProcessLogsToArchives(_testArchives);

            _mockFileDeleter.Verify(fd => fd.TryDeleteFiles(It.Is<string[]>(es => es.SequenceEqual(testLogs)), It.IsAny<bool>()));
        }

        [Test]
        public void ProcessLogsToArchives_CompressorReturnsError_SkipTryingToDeleteLogs()
        {
            _mockCompressor
                .Setup(c => c.Compress(It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(true);

            cut.ProcessLogsToArchives(_testArchives);

            _mockFileDeleter.Verify(fd => fd.TryDeleteFiles(It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()),Times.Never);
        }

        [Test]
        public void ProcessLogsToArchives_BatcherReturnsBatchedArchiving_EverythingCalledTwiceInTheCompressPart()
        {
            _mockCompressor
                .Setup(c => c.Compress(It.IsAny<string>(), It.IsAny<string[]>()))
                .Returns(true);

            cut.ProcessLogsToArchives(_testArchives);

            _mockFileDeleter.Verify(fd => fd.TryDeleteFiles(It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()),Times.Never);
        }
    }
}
