using Autofac;
using System.Configuration;
using Utilities;

namespace DataAccessLib
{
    public class DataAccessModule : Module
    {
        private static string EarthyProductsPathKey = "EarthyProductsPath";
        private static string IntergalacticProductsPathKey = "IntergalacticProductsPath";
        private static string InputFilePathKey = "InputFilePath";

        protected override void Load(ContainerBuilder builder)
        {
            string intergalacticProductsPath = ConfigurationManager.AppSettings[IntergalacticProductsPathKey];
            string earthyProductsPath = ConfigurationManager.AppSettings[EarthyProductsPathKey];
            string inputFilePath = ConfigurationManager.AppSettings[InputFilePathKey];

            builder.Register(c => 
            {
                IFileReader reader = c.Resolve<IFileReader>();
                return new NumeralRepository(reader, intergalacticProductsPath, earthyProductsPath, inputFilePath);
            })
            .As<INumeralRepository>()
            .SingleInstance();
        }
    }
}