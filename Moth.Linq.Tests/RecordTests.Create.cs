using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Moth.Linq.Tests
{
    public partial class RecordTests
    {
        [Test]
        public void CreateOne()
        {
            var employee = new Employee { FirstName = "Emre", LastName = "Þentürk" };
            employee.Create();
            Assert.AreNotEqual(employee.UId, Guid.Empty);
            Assert.AreNotEqual(employee.Id, 0);
        }

        [Test]
        public void CreateOneWithRelation()
        {
            var employee = new Employee {FirstName = "Random", LastName = "Employee"};
            var department = new Department {Name = "Research & Development"};
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
        public void CreateMultiple()
        {
            var names = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("Candie", "Huneke"),
                    new Tuple<string, string>("Fernando", "Vida"),
                    new Tuple<string, string>("Nubia", "Kovach"),
                    new Tuple<string, string>("Natacha", "Schnabel"),
                    new Tuple<string, string>("Nicol", "Kenny"),
                    new Tuple<string, string>("Augustina", "Nestor"),
                    new Tuple<string, string>("Sherlyn", "Wiste"),
                    new Tuple<string, string>("Daine", "Pickles"),
                    new Tuple<string, string>("Adah", "Hagedorn"),
                    new Tuple<string, string>("Sandy", "Mattera"),
                    new Tuple<string, string>("Sherilyn", "Sica"),
                    new Tuple<string, string>("Dwana", "Willson"),
                    new Tuple<string, string>("Magdalena", "Cushenberry"),
                    new Tuple<string, string>("Margarete", "Nehls"),
                    new Tuple<string, string>("Hailey", "Samuel"),
                    new Tuple<string, string>("Tamra", "Artist"),
                    new Tuple<string, string>("Gudrun", "Clayborn"),
                    new Tuple<string, string>("Claris", "Nowack"),
                    new Tuple<string, string>("Arvilla", "Cifuentes"),
                    new Tuple<string, string>("Nathaniel", "Cerda"),
                };

            names.Select(fullName => new Employee {FirstName = fullName.Item1, LastName = fullName.Item2}).ToList().ForEach(e => e.Create());

        }
    }
}