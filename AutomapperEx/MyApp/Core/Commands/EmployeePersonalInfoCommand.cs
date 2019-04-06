using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class EmployeePersonalInfoCommand : ICommand
    {
        private readonly MyAppContext context;

        public EmployeePersonalInfoCommand(MyAppContext context)
        {
            this.context = context;
        }

        public string Execute(string[] args)
        {
            int employeeId = int.Parse(args[0]);

            var employee = this.context.Employees.FirstOrDefault(e => e.Id == employeeId);

            if(employee == null)
            {
                throw new ArgumentException("Employee does not exist!");
            }

            return $"ID: {employee.Id} - {employee.FirstName} {employee.LastName} - ${employee.Salary:f2}" +
                $"{Environment.NewLine}Birthday: {employee.Birthday.Value.ToShortDateString()}" +
                $"{Environment.NewLine}Address: {employee.Address}";
        }
    }
}
