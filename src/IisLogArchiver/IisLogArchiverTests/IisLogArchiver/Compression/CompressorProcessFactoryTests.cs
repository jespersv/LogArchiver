using IisLogArchiver.Compression;
using NUnit.Framework;

namespace IisLogArchiverTests
{
    [TestFixture]
    public class CompressorProcessFactoryTests
    {

        private CompressorProcessFactory cut;
        [SetUp]
        public void Setup()
        {
            cut = new CompressorProcessFactory();
        }

        [Test]
        public void CreateCompressProcessInfo_AnyValue_ReturnsCustomProcess()
        {
            var customerProcessStartInfo = cut.CreateCompressProcessInfo("", "");

            Assert.IsNotNull(customerProcessStartInfo);
        }
    }
}
