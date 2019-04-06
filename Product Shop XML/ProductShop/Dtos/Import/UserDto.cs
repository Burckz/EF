using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.Dtos.Import
{
    public class UserDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? Age { get; set; }
    }
}
