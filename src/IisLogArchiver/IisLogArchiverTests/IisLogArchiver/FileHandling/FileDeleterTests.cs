using IisLogArchiver.FileHandling;
using IisLogArchiver.Interfaces;
using Moq;
using NUnit.Framework;

namespace IisLogArchiverTests
{
    [TestFixture]
    public class FileDeleterTests
    {
        private FileDeleter cut;
        private Mock<IFileProvider> _mockFileProvider;
        private Mock<IThreadProvider> _mockThreadProvider;
        private string _testFilePath;

        [SetUp]
        public void Setup()
        {
            _mockFileProvider = new Mock<IFileProvider>();
            _mockThreadProvider = new Mock<IThreadProvider>();

            cut = new FileDeleter(_mockFileProvider.Object, _mockThreadProvider.Object);
        }

        [Test]
        public void TryDeleteFile_ExistsNotLocked_FileProviderCalledWithParameters()
        {
            _testFilePath = @"x:\logs\file.f";

            _mockFileProvider
                .Setup(fp => fp.Exists(It.IsAny<string>()))
                .Returns(true);
            _mockFileProvider
                .Setup(fp => fp.IsFileLocked(It.IsAny<string>()))
                .Returns(false);

            cut.TryDeleteFile(_testFilePath, true);

            _mockFileProvider
                .Verify(fp => fp.DeleteFile(It.Is<string>(s => s.Equals(_testFilePath)), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public void TryDeleteFile_DoesNotExist_FileProviderNeverCalled()
        {
            _testFilePath = @"x:\logs\file.f";

            _mockFileProvider
                .Setup(fp => fp.Exists(It.IsAny<string>()))
                .Returns(false);

            cut.TryDeleteFile(_testFilePath, true);

            _mockFileProvider
                .Verify(fp => fp.DeleteFile(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public void TryDeleteFile_FileIsLocked_FileProviderNeverCalled()
        {
            _testFilePath = @"x:\logs\file.f";

            _mockFileProvider
                .Setup(fp => fp.Exists(It.IsAny<string>()))
                .Returns(true);
            _mockFileProvider
                .Setup(fp => fp.IsFileLocked(It.IsAny<string>()))
                .Returns(true);

            cut.TryDeleteFile(_testFilePath, true);

            _mockFileProvider
                .Verify(fp => fp.DeleteFile(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public void TryDeleteFiles_state_expect()
        {
            //todo
        }
    }
}
