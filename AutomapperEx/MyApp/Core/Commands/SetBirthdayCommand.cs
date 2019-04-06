using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class SetBirthdayCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public SetBirthdayCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] args)
        {
            int employeeId = int.Parse(args[0]);
            DateTime date = DateTime.ParseExact(args[1], "dd-MM-yyyy", CultureInfo.InstalledUICulture);

            var employee = this.context.Employees.FirstOrDefault(e => e.Id == employeeId);

            if(employee == null)
            {
                throw new ArgumentException("Employee does not exist!");
            }

            employee.Birthday = date;

            context.SaveChanges();

            var employeeBirthdayDto = this.mapper.CreateMappedObject<EmployeeBirthdayDto>(employee);

            return $"Birthday of employee {employeeBirthdayDto.FirstName} {employeeBirthdayDto.LastName} successfully updated!";
        }
    }
}
