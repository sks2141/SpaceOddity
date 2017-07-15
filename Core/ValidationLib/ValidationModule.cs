using Autofac;
using DataAccessLib;
using System.Collections.Generic;

namespace ValidationLib
{
    public class ValidationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ValidationMessageManager>().As<IValidationMessageManager>().SingleInstance();
            builder.RegisterType<ArabicNumeralValidator>().As<IValidator<int>>().SingleInstance();


            builder.Register(c =>
            {
                INumeralRepository repository = c.Resolve<INumeralRepository>();
                IEnumerable<char> romanNumeralSymbols = repository.GetRomanToArabicMap().Keys;
                return new RomanNumeralValidator(romanNumeralSymbols);
            })
            .As<IValidator<string>>()
            .SingleInstance();

            builder.RegisterType<RuleValidationEngine>().As<IRuleValidationEngine>().SingleInstance();
        }
    }
}