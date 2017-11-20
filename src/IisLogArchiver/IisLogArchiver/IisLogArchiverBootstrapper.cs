using Autofac;
using IisLogArchiver.Common;
using IisLogArchiver.Compression;
using IisLogArchiver.Core;
using IisLogArchiver.FileHandling;
using IisLogArchiver.Interfaces;

namespace IisLogArchiver
{
    public static class IisLogArchiverBootstrapper
    {
        public static IContainer BuildContainer()
        {
            var builder = GetContainerBuilder();
            return builder.Build();
        }

        public static ContainerBuilder GetContainerBuilder()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Archiver>().As<IArchiver>();
            builder.RegisterType<ArchiveSettings>().As<IArchiveSettings>();

            builder.RegisterType<Compressor>().As<ICompressor>();
            builder.RegisterType<CompressorProcessFactory>().As<ICompressProcessFactory>();
            builder.RegisterType<ProcessStarter>().As<IProcessStarter>();

            builder.RegisterType<TimeProvider>().As<ITimeProvider>();
            builder.RegisterType<ThreadProvider>().As<IThreadProvider>();
            builder.RegisterType<EnvironmentProvider>().As<IEnvironmentProvider>();

            builder.RegisterType<FileDeleter>().As<IFileDeleter>();
            builder.RegisterType<ArchiveNameProvider>().As<IArchiveNameProvider>();
            builder.RegisterType<FileGatherer>().As<IFileGatherer>();
            builder.RegisterType<DirectoryProvider>().As<IDirectoryProvider>();
            builder.RegisterType<FileNameParser>().As<IFileNameParser>();
            builder.RegisterType<FileProvider>().As<IFileProvider>();
            builder.RegisterType<FileBatchProvider>().As<IFileBatchProvider>();
            builder.RegisterType<CommandLineOptions>().As<ICommandLineOptions>();

            return builder;
        }
    }
}