using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Moth.Linq.Tests.TestClasses;
using NUnit.Framework;

namespace Moth.Linq.Tests
{
    public partial class RecordTests
    {
        [Test]
        public void RetrieveSingle()
        {
            var employee = new Employee { FirstName = "Some", LastName = "Guy" };
            var anotherEmployee = new Employee { FirstName = "Some", LastName = "Lady" };

            employee.Create();
            Assert.AreNotEqual(0, employee.Id);
            Assert.AreNotEqual(Guid.Empty, employee.UId);
            anotherEmployee.Create();

            var employeeRetrieved = Employee.Records.Single(e => e.LastName == "Guy");
            var ladyRetrieved = Employee.Records.Single(l => l.LastName == "Lady");

            Assert.AreEqual(employeeRetrieved.LastName, employee.LastName);
            Assert.AreEqual(ladyRetrieved.LastName, anotherEmployee.LastName);
        }

        [Test]
        public void RetrieveMultiple()
        {
            var employeeList = new List<Employee>();
            for (var i = 0; i < 20; i++)
            {
                var employeeName = GetRandomName();
                var employee = new Employee { FirstName = employeeName.Item1, LastName = employeeName.Item2 };
                employee.Create();
                employeeList.Add(employee);
            }

            Trace.WriteLine("Just fetch all employees");
            var retrievedEmployees = from employee in Employee.Records select employee;
            Assert.True(retrievedEmployees.ToList().All(re => employeeList.Any(el => el.UId == re.UId)), "Retrieve all employees has error");
            TabulateEmployees(retrievedEmployees, employeeList);
            Trace.WriteLine("");
            Trace.WriteLine("");
            Trace.WriteLine("");
            Trace.WriteLine("Order By First Name");
            var employeesSortedByFirstName = Employee.Records.OrderBy(e => e.FirstName).ToList();
            var controlList = employeeList.OrderBy(e => e.FirstName).ToList();
            var sortedForSinglePropertyCorrectly = true;
            for (var i = 0; i < employeesSortedByFirstName.Count; i++)
            {
                sortedForSinglePropertyCorrectly = sortedForSinglePropertyCorrectly && employeesSortedByFirstName[i].UId == controlList[i].UId;
            }

            TabulateEmployees(employeesSortedByFirstName, controlList);
            Assert.True(sortedForSinglePropertyCorrectly, "Sorting for single property has problems");
            Trace.WriteLine("");
            Trace.WriteLine("");
            Trace.WriteLine("");
            Trace.WriteLine("Order By first name and then last name");
            var employeesSortedByFirstNameAndLastName = Employee.Records.OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList();
            var controlListForMultipleSort = employeeList.OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList();
            var sortedForMultiplePropertyCorrectly = true;
            for (var i = 0; i < employeesSortedByFirstNameAndLastName.Count; i++)
            {
                sortedForMultiplePropertyCorrectly = sortedForMultiplePropertyCorrectly && employeesSortedByFirstNameAndLastName[i].UId == controlListForMultipleSort[i].UId;
            }

            TabulateEmployees(employeesSortedByFirstNameAndLastName, controlListForMultipleSort);
            Assert.True(sortedForMultiplePropertyCorrectly, "Sorting for multiple properties has problems");
        }

        private void TabulateEmployees(IEnumerable<Employee> employees, List<Employee> controlList)
        {
            var empList = employees.ToList();
            Trace.WriteLine("Test               \t\t\t\tControl");
            Trace.WriteLine("Id\tFirstName\tLastName\t\tId\tFirstName\tLastName");
            Trace.WriteLine("------------------------------------------------------------------------------");
            for (var i = 0; i < empList.Count; i++)
            {
                var employee = empList[i];
                var controlEmployee = controlList[i];
                Trace.WriteLine(
                    string.Format("{0}\t{1}\t{2}\t\t{3}\t{4}\t{5}",
                    employee.Id.ToString(CultureInfo.InvariantCulture).PadRight(2),
                    employee.FirstName.PadRight(10),
                    employee.LastName.PadRight(10), controlEmployee.Id.ToString(CultureInfo.InvariantCulture).PadRight(2), controlEmployee.FirstName.PadRight(10), controlEmployee.LastName));
            }
        }
    }
}