using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core.ViewModels
{
    public class EmployeeBirthdayDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? Birthday { get; set; }
    }
}
