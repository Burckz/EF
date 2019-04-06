using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class ManagerInfoCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public ManagerInfoCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] args)
        {
            int managerId = int.Parse(args[0]);

            var manager = this.context.Employees
                .Include(e => e.ManagedEmployees)
                .FirstOrDefault(e => e.Id == managerId);

            if(manager == null)
            {
                throw new ArgumentException($"Employee does not exist!");
            }

            var managerDto = this.mapper.CreateMappedObject<ManagerDto>(manager);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{managerDto.FirstName} {managerDto.LastName} | Employees: {managerDto.ManagedEmployees.Count}");

            foreach (var employeeDto in managerDto.ManagedEmployees)
            {
                sb.AppendLine($"{Convert.ToChar(9)}- {employeeDto.FirstName} {employeeDto.LastName} - ${employeeDto.Salary:f2}");
            }

            return sb.ToString().Trim();
        }
    }
}
