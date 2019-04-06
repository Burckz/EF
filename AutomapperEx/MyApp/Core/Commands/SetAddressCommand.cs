using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class SetAddressCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public SetAddressCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] args)
        {
            int employeeId = int.Parse(args[0]);
            string address = args[1];

            var employee = this.context.Employees.FirstOrDefault(e => e.Id == employeeId);

            if(employee == null)
            {
                throw new ArgumentException("Employee does not exist!");
            }

            employee.Address = address;
            context.SaveChanges();

            var employeeAddressDto = mapper.CreateMappedObject<EmployeeAddressDto>(employee);

            return $"Address of employee {employeeAddressDto.FirstName} {employeeAddressDto.LastName} successfully updated!";
        }
    }
}
