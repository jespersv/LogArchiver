using IisLogArchiver.FileHandling;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace IisLogArchiverTests
{
    [TestFixture]
    public class FileNameParserTests
    {
        private FileNameParser cut;

        [SetUp]
        public void SetUp()
        {
            cut = new FileNameParser();
        }

        [Test]
        public void TryParseDateFromString_ValidFormats_ReturnsTrue()
        {
            var testString = new[]
            {
                "text170606.log",
                "text20170606.log",
                "111111.log",
                "20111111.log",
                "170606_123.log-123",

                "text170606.txt",
                "text20170606.txt",
                "111111.txt",
                "20111111.txt",
                "170606_123.txt-123",


                "text170606654.log",
                "text20170606654.log",
                "111111654.log",
                "20111111654.log",
            };

            foreach (var str in testString)
            {
                var valid = cut.TryParseDateFromString(str, out DateTime d);
                Debug.Print(str);

                Assert.IsTrue(valid);
                Assert.AreNotEqual(DateTime.MinValue, d);
            }
        }

        [Test]
        public void TryParseDateFromString_InValidFormats_ReturnsFalse()
        {

            var testString = new[]
            {
                "",
                "asdf",
                "asdf.log",
                "asdf.txt",
                "101310.log",
                "101232.log"
            };

            foreach (var s in testString)
            {
                var notValid = cut.TryParseDateFromString("", out DateTime d);
                Assert.IsFalse(notValid);
                Assert.AreEqual(DateTime.MinValue, d);
            }
        }
    }
}
