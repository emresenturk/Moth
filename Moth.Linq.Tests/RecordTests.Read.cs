using System;
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
            var employee = new Employee {FirstName = "Some", LastName = "Guy"};
            var anotherEmployee = new Employee {FirstName = "Some", LastName = "Lady"};

            employee.Create();
            Assert.AreNotEqual(0, employee.Id);
            Assert.AreNotEqual(Guid.Empty, employee.UId);
            anotherEmployee.Create();

            var employeeRetrieved = Employee.Records.Single(e => e.LastName == "Guy");
            var ladyRetrieved = Employee.Records.Single(l => l.LastName == "Lady");

            Assert.AreEqual(employeeRetrieved.LastName, employee.LastName);
            Assert.AreEqual(ladyRetrieved.LastName, anotherEmployee.LastName);
        }
    }
}