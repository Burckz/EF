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
    public class ListEmployeesOlderThanCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public ListEmployeesOlderThanCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] args)
        {
            int age = int.Parse(args[0]);

            var employeesOlderThan = this.context.Employees.Where(e => DateTime.Now.Year - e.Birthday.Value.Year > age)
                .OrderByDescending(e => e.Salary)
                .ToList();


            List<EmployeeDto> employeesOlderDtos = new List<EmployeeDto>();

            foreach (var employee in employeesOlderThan)
            {
                var employeeOlderThanDto = this.mapper.CreateMappedObject<EmployeeDto>(employee);
                employeesOlderDtos.Add(employeeOlderThanDto);
            }

            StringBuilder sb = new StringBuilder();

            foreach (var employeeDto in employeesOlderDtos)
            {
                foreach (var emp in employeesOlderThan)
                {
                    if (emp.Id == employeeDto.Id)
                    {
                        sb.AppendLine($"{employeeDto.FirstName} {employeeDto.LastName} - ${employeeDto.Salary:f2} - Manager: {(emp.Manager == null ? "[no manager]" : emp.Manager.LastName)}");
                    }
                   
                }
            }

            return sb.ToString().Trim();
        }
    }
}
