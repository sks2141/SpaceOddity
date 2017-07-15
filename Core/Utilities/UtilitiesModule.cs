using Autofac;

namespace Utilities
{
    public class UtilitiesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileReader>().As<IFileReader>().SingleInstance();
        }
    }
}