using System;
using System.Collections.Generic;
using System.Linq;
using IisLogArchiver.FileHandling;
using IisLogArchiver.Interfaces;
using Moq;
using NUnit.Framework;

namespace IisLogArchiverTests
{
    [TestFixture]
    public class FileBatchProviderTests
    {
        private FileBatchProvider cut;
        private Mock<IArchiveSettings> _mockArchiveSettings;

        [SetUp]
        public void SetUp()
        {
            _mockArchiveSettings = new Mock<IArchiveSettings>();

            _mockArchiveSettings.SetupGet(ass => ass.ArgumentLengthBeforePerCompress).Returns(50);

            cut = new FileBatchProvider(_mockArchiveSettings.Object);
        }

        [Test]
        public void Batch_CountOverSettingsBatchCountSplitsBatch_BatchesShouldBeTwo()
        {
            var imaginaryPaths = new List<string>();
            for (int i = 0; i < 10; i++)
                imaginaryPaths.Add(RandomString(10));

            var batches=  cut.Batch(imaginaryPaths);

            Assert.AreEqual(2, batches.Count());
        }

        [Test]
        public void Batch_CountUnderSettingsBatchCountSplitsBatch_BatchesShouldBeOne()
        {
            var imaginaryPaths = new List<string>();
            for (int i = 0; i < 4; i++)
                imaginaryPaths.Add(RandomString(10));

            var batches=  cut.Batch(imaginaryPaths);

            Assert.AreEqual(1, batches.Count());
        }

        private static readonly Random Random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
