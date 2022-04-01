using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeDirectory
{
    class Employee
    {
        // Constructors
        public Employee(string LastName, string FirstName, string Phone, string City, string State, string Title, string Department, string Mobile, string Email)
        {
            this.LastName = LastName;
            this.FirstName = FirstName;
            this.DirectPhone = Phone;
            this.City = City;
            this.State = State;
            this.JobTitle = Title;
            this.Department = Department;
            this.MobilePhone = Mobile;
            this.Email = Email;
        }

        // getters and setters
        public string LastName { get; private set; } = null;
        public string FirstName { get; private set; } = null;
        public string DirectPhone { get; private set; } = null;
        public string City { get; private set; } = null;
        public string State { get; private set; } = null;
        public string JobTitle { get; private set; } = null;
        public string Department { get; private set; } = null;
        public string MobilePhone { get; private set; } = null;
        public string Email { get; private set; } = null;
    }
}
