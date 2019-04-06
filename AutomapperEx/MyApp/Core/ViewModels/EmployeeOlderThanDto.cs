using MyApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core.ViewModels
{
    public class EmployeeOlderThanDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public decimal Salary { get; set; }

    }
}
