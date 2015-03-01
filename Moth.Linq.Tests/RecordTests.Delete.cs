using System;
using System.Linq;
using Moth.Linq.Tests.TestClasses;
using NUnit.Framework;

namespace Moth.Linq.Tests
{
    public partial class RecordTests
    {
        [Test]
        public void DeleteSingle()
        {
            var employee = new Employee {FirstName = "Employee", LastName = "Dude"};
            employee.Create();

            employee.Delete();

            Assert.Throws<InvalidOperationException>(delegate
            {
// ReSharper disable UnusedVariable
                var emp = Employee.Records.First(e => e.FirstName == "Employee");
// ReSharper restore UnusedVariable
            });

            Assert.IsNull(Employee.Records.FirstOrDefault(e => e.FirstName == "Employee"));
        }
    }
}