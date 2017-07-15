using Autofac;
using DataAccessLib;
using NumeralCalculator.Converter;
using NumeralCalculator.Interpreter;
using NumeralCalculator.Worker;
using NumeralCalculator.Validator;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utilities;

namespace NumeralCalculator
{
    public class NumeralCalculatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            this.RegisterConverters(builder);
            this.RegisterProductDependencies(builder);
            this.RegisterValidators(builder);
            this.RegisterInterpreters(builder);
            this.RegisterProcessors(builder);
        }

        private void RegisterConverters(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                INumeralRepository repository = c.Resolve<INumeralRepository>();

                return new ConverterStrategy(
                    repository.GetRomanToArabicMap(), 
                    repository.GetArabicToRomanMap());
            })
            .As<IConverterStrategy>()
            .SingleInstance();
        }

        private void RegisterProductDependencies(ContainerBuilder builder)
        {
            builder.Register(c => 
            {
                INumeralRepository repository = c.Resolve<INumeralRepository>();

                return new ProductCategorizer(
                    repository.GetIntergalacticProducts(), 
                    repository.GetEarthyProducts());
            })
            .As<IProductCategorizer>().SingleInstance();

            builder.RegisterType<ProductHelper>()
                   .As<IProductHelper>().SingleInstance();
        }

        private void RegisterValidators(ContainerBuilder builder)
        {
            // Shared between ProductDataValidation & QueryValidator in single instance scope, to help massage input in PreProcess step
            // This can be enhanced to persist to file and later bootstrap the contents.
            ICollection<string> productsList = new Collection<string>(); 

            builder.RegisterType<ProductDataValidator>()
                   .WithParameter("productsList", productsList)
                   .As<IProductDataValidator>().SingleInstance();

            builder.RegisterType<QueryValidator>()
                   .WithParameter("productsList", productsList)
                   .As<IQueryValidator>().SingleInstance();

            builder.RegisterType<CompoundValidator>()
                   .As<ICompoundValidator>().SingleInstance();
        }

        private void RegisterInterpreters(ContainerBuilder builder)
        {
            // Shared between ProductDataInterpreter & QueryInterpreter in single instance scope, to help store 
            // Intergalactic products by Roman Numeric Units & Earthy products by Arabic Numeric Units
            IDictionary<string, string> intergalacticProductsCache = new Dictionary<string, string>();
            IDictionary<string, decimal> earthyProductsCache = new Dictionary<string, decimal>();

            builder.RegisterType<ProductDataInterpreter>()
                   .WithParameters(new[]
                   {
                       new NamedParameter("intergalacticProductsCache", intergalacticProductsCache),
                       new NamedParameter("earthyProductsCache",earthyProductsCache)
                   })
                   .As<IInterpreter<int>>().SingleInstance();

            builder.RegisterType<QueryInterpreter>()
                .WithParameters(new[]
                {
                    new NamedParameter("intergalacticProductsCache", intergalacticProductsCache),
                    new NamedParameter("earthyProductsCache",earthyProductsCache)
                })
                .As<IInterpreter<string>>().SingleInstance();
        }

        private void RegisterProcessors(ContainerBuilder builder)
        {
            builder.RegisterType<PreProcessor>().As<IPreProcessor>().SingleInstance();
            builder.RegisterType<Analyzer>().As<IAnalyzer>().SingleInstance();
            builder.RegisterType<Processor>().As<IProcessor>().SingleInstance();
        }
    }
}