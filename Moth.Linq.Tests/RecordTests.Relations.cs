using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Moth.Linq.Tests
{
    public partial class RecordTests
    {
        [Test]
        public void AddChild()
        {
            var employee = new Employee { FirstName = "Random", LastName = "Employee" };
            var department = new Department { Name = "Research & Development" };
            department.Create();
            employee.Create();
            department.Employees.Add(employee);
            var randomEmployee = department.Employees.First(emp => emp.FirstName == "Random");
            Assert.NotNull(randomEmployee);
            Trace.WriteLine(randomEmployee.FirstName + " " + randomEmployee.LastName);
            var employeeRetrieved = Employee.Records.First(emp => emp.UId == employee.UId);
            Trace.WriteLine(employeeRetrieved.Department.UId);
            Department employeeDepartment = employeeRetrieved.Department;
            Trace.WriteLine(employeeDepartment.Name);
        }

        [Test]
        public void RemoveChild()
        {
            var employee = new Employee {FirstName = "Dennis", LastName = "Reynolds"};
            var department = new Department {Name = "Management"};
            // I'm watching the jersey shore episode now.
            department.Create();
            department.Employees.Add(employee);
            employee.Create();
            department.Employees.Remove(employee);
            Assert.IsNull(department.Employees.Where(e => e.Department == department.UId).FirstOrDefault());
        }
    }
}