using System.Threading;
using NUnit.Framework;

namespace Moth.Linq.Tests
{
    public partial class RecordTests
    {
        [Test]
        public void CheckIfDateUpdatedChanges()
        {
            var employee = new Employee {FirstName = "John", LastName = "Anderson"};
            employee.Create();

            Assert.IsNull(employee.DateUpdated);

            employee.FirstName = "George";
            employee.Update();
            Assert.IsNotNull(employee.DateUpdated);
            Thread.Sleep(5000);
            var oldDateUpdated = employee.DateUpdated.Value;
            employee.FirstName = "Kurt";
            employee.Update();
            Assert.AreNotEqual(oldDateUpdated, employee.DateUpdated.Value);
        }
    }
}