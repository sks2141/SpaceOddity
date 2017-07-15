using Autofac;
using DataAccessLib;
using NumeralCalculator;
using NumeralCalculator.Worker;
using Utilities;
using ValidationLib;

namespace ClientConsole
{
    public class DependencyBuilder
    {
        private readonly ContainerBuilder builder;

        public DependencyBuilder()
        {
            this.builder = new ContainerBuilder();
        }

        public void LoadDependencyGraph()
        {
            builder.RegisterModule<DataAccessModule>();
            builder.RegisterModule<UtilitiesModule>();
            builder.RegisterModule<ValidationModule>();
            builder.RegisterModule<NumeralCalculatorModule>();
            
            builder.Register(c =>
            {
                INumeralRepository repository = c.Resolve<INumeralRepository>();

                return new Runner(c.Resolve<IProcessor>(),
                    repository.GetInputFileContents());
            })
            .As<IStartable>()
            .SingleInstance();
        }

        public void Build()
        {
            this.builder.Build();
        }
    }
}