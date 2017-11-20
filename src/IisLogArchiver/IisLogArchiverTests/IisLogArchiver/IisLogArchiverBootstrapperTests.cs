using Autofac;
using IisLogArchiver;
using IisLogArchiver.Interfaces;
using NUnit.Framework;

namespace IisLogArchiverTests
{
    [TestFixture]
    public class IisLogArchiverBootstrapperTests
    {
        [Test]
        public void BuildContainer_BuildContainer_ResolveArchiverWithOutError()
        {
            var container = IisLogArchiverBootstrapper.BuildContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                var archiver = scope.Resolve<IArchiver>();
            }
        }
    }
}
