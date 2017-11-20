using IisLogArchiver.Core;
using IisLogArchiver.FileHandling;
using IisLogArchiver.Interfaces;
using Moq;
using NUnit.Framework;
using System;

namespace IisLogArchiverTests
{
    [TestFixture]
    public class ArchiveNameProviderTests
    {
        private Mock<IEnvironmentProvider> _mockEnvProvider;
        private Mock<IFileProvider> _mockFileProvider;

        private ArchiveNameProvider cut;

        private readonly DateTime _testADate = new DateTime(2017, 06, 12);
        private readonly Archive _testArchive = new Archive()
        {
            ArchiveName = "name",
            ArchivePath = @"x:\path\archive",
            DeleteArchivedFiles = true,
            FilePath = @"x:\path"
        };

        [SetUp]
        public void SetUp()
        {
            _mockEnvProvider = new Mock<IEnvironmentProvider>();
            _mockFileProvider = new Mock<IFileProvider>();

            cut = new ArchiveNameProvider(_mockFileProvider.Object, _mockEnvProvider.Object);

        }

        [Test]
        public void DecideArchiveName_NameFromEnvironmentProvider_IsPartOfBaseName()
        {
            var machinename = "machinename";
            _mockEnvProvider
                .SetupGet(ep=>ep.MachineName)
                .Returns(machinename);

            var generatedName = cut.DecideArchiveName(_testArchive, _testADate);

            Assert.IsTrue(generatedName.Contains(machinename));
        }

        [Test]
        public void DecideArchiveName_NameFromEnvironmentProvider_IsFirstValueFromGeneratedName()
        {
            var machinename = "machinename";
            _mockEnvProvider
                .SetupGet(ep=>ep.MachineName)
                .Returns(machinename);
            _testArchive.ArchiveName = _testArchive.ArchivePath = "";

            var generatedName = cut.DecideArchiveName(_testArchive, _testADate);

            Assert.IsTrue(generatedName.StartsWith(machinename));
        }

        [Test]
        public void DecideArchiveName_GeneratedNameExists_ContainsIterationNumber()
        {
            var machinename = "machinename";
            _mockEnvProvider
                .SetupGet(ep=>ep.MachineName)
                .Returns(machinename);
            _testArchive.ArchiveName = _testArchive.ArchivePath = "";
            var archiveName = "machinename__170612.7z";

            _mockFileProvider
                .Setup(fp => fp.Exists(It.Is<string>(s => s.Equals(archiveName))))
                .Returns(true);


            var generatedName = cut.DecideArchiveName(_testArchive, _testADate);

            Assert.IsTrue(generatedName.Contains("_1"));
        }

        [Test]
        public void DecideArchiveName_AllGeneratedNameExists_ThrowsError()
        {

            _mockFileProvider
                .Setup(fp => fp.Exists(It.IsAny<string>()))
                .Returns(true);

            Assert.Throws<Exception>(() => cut.DecideArchiveName(_testArchive, _testADate));
        }
    }
}
