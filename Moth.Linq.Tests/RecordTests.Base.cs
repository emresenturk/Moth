using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Moth.Configuration;
using Moth.Database;
using Moth.Database.MsSql;
using Moth.Extensions;
using NUnit.Framework;

namespace Moth.Linq.Tests
{
    [TestFixture]
    public partial class RecordTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                var databaseConfig = new DatabaseConfiguration
                {
                    ConnectionString = connectionString.ConnectionString,
                    Name = connectionString.Name,
                    Provider = connectionString.ProviderName
                };

                DatabaseContainer.DefaultContainer.Register<MsSqlDatabase>(databaseConfig);
            }

            CreateTable();
        }

        [Test]
        public void UpdateTest()
        {
            var employee = new Employee {FirstName = "EmpOne", LastName = "Employeeson"};
            employee.Create();
            employee.LastName = "Employeeberg";
            employee.Update();
            var employeeRecord = Employee.Records.First(e => e.Id == employee.Id);
            Assert.AreEqual(employee.LastName, employeeRecord.LastName);
        }

        [Test]
        public void TestOne()
        {
            var employees = from employee in Employee.Records where employee.Id > 5 || employee.FirstName == "Claris" orderby employee.FirstName, employee.Id descending select employee;
            Trace.WriteLine("Id\tFirst Name\tLastName");
            foreach (var employeeRecord in employees)
            {
                Trace.WriteLine(string.Format("{0}\t{1}\t{2}", employeeRecord.Id, employeeRecord.FirstName, employeeRecord.LastName));
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Query.Create(@"EXEC sp_MSforeachtable @command1 = ""DROP TABLE ?""").Execute().NonQuery();
            DatabaseContainer.DefaultContainer.Dispose();
        }

        private void CreateTable()
        {
            TearDown();
            Query.Create(
                "CREATE TABLE [Moth.Linq.Tests.Employee] (Id int NOT NULL PRIMARY KEY IDENTITY(1,1), UId uniqueidentifier, DateCreated DateTime NOT NULL, DateUpdated DateTime NULL,FirstName varchar(max), LastName varchar(MAX), Department uniqueidentifier)")
                .Execute()
                .NonQuery();
            Query.Create(
                "CREATE TABLE [Moth.Linq.Tests.Department](Id int NOT NULL PRIMARY KEY IDENTITY(1,1), UId uniqueidentifier, DateCreated DateTime NOT NULL, DateUpdated DateTime NULL, Name varchar(max))").Execute().NonQuery();
        }
    }
}