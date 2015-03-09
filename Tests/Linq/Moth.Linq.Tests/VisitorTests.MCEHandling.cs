using System;
using System.Linq;
using Moth.Linq.Tests.TestClasses;
using NUnit.Framework;

namespace Moth.Linq.Tests
{
    public partial class VisitorTests
    {
        [Test]
        public void GetMethodNames()
        {
            var employee =
                Employee.Records.Where(emp => emp.FirstName == "Deneme" && emp.DateCreated != DateTime.Today).ToList();
        }
    }
}