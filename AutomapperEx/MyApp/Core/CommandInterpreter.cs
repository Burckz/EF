using Microsoft.Extensions.DependencyInjection;
using MyApp.Core.Commands.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyApp.Core
{
    public class CommandInterpreter : ICommandInterpreter
    {
        private const string Suffix = "Command";
        private readonly IServiceProvider serviceProvider;

        public CommandInterpreter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public string Read(string[] args)
        {
            string command = args[0] + Suffix;

            string[] commandParams = args.Skip(1).ToArray();



            var type = Assembly.GetCallingAssembly()
                .GetTypes()
                .FirstOrDefault(x => x.Name == command);

            if(type == null)
            {
                throw new ArgumentException("Invalid command!");
            }


            var constructor = type.GetConstructors().FirstOrDefault();

            var constructorParams = constructor
                .GetParameters()
                .Select(x => x.ParameterType)
                .ToArray();

            var services = constructorParams
                .Select(this.serviceProvider.GetService)
                .ToArray();

            

            var invokeConstructor = (ICommand)constructor.Invoke(services);

            return invokeConstructor.Execute(commandParams);
            
        }
    }
}
