using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denomica.Text.Json.Tests
{

    internal class Employee
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Email { get; set; }

        public string? MobilePhone { get; set; }

        public string EmployeeNumber { get; set; } = null!;

    }

    internal class Endpoint
    {
        public string Uri { get; set; } = null!;
    }

    internal class EndpointResponse
    {
        public Endpoint Endpoint { get; set; } = null!;

        public Dictionary<string, object?> Meta { get; set; } = new Dictionary<string, object?>();
    }

    internal class RecordSet
    {
        public List<object> Records { get; set; } = new List<object>();
    }
}
