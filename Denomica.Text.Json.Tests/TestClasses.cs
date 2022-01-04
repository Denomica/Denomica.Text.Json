using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denomica.Text.Json.Tests
{

    internal class Employee
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? MobilePhone { get; set; }

        public string EmployeeNumber { get; set; } = string.Empty;

    }

}
