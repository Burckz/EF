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
    public class SetManagerCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public SetManagerCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] args)
        {
            int employeeId = int.Parse(args[0]);
            int managerId = int.Parse(args[1]);

            if(employeeId == managerId)
            {
                throw new ArgumentException("Lol?");
            }

            var employee = this.context.Employees.FirstOrDefault(e => e.Id == employeeId);
            var manager = this.context.Employees.FirstOrDefault(m => m.Id == managerId);

            if(employee == null || manager == null)
            {
                throw new ArgumentException("Employee does not exist!");
            }

            employee.ManagerId = manager.Id;
            context.SaveChanges();

            var employeeDto = this.mapper.CreateMappedObject<EmployeeDto>(employee);
            var managerDto = this.mapper.CreateMappedObject<ManagerDto>(manager);

            return $"Employee {employeeDto.FirstName} {employeeDto.LastName} is managed by {managerDto.FirstName} {managerDto.LastName}";
        }
    }
}
