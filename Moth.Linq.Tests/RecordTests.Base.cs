using System;
using System.Collections.Generic;
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
    public class Employee : RecordBase<Employee>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        protected override void OnCreated(Employee employee)
        {
            Trace.WriteLine(string.Format("My Id is :{0}", employee.Id));
            Trace.WriteLine(string.Format("My UniqueId is :{0}", employee.UId));
            Trace.WriteLine(string.Format("I am created on {0}", employee.DateCreated));
        }

        protected override void OnDeleted(Employee employee)
        {
            Trace.WriteLine("I'm deleted");
        }

        protected override void OnUpdated(Employee employee)
        {
            Trace.WriteLine(string.Format("I was created on {0}", employee.DateCreated));
            Trace.WriteLine(string.Format("I am updated on {0}", employee.DateUpdated.Value));
            Trace.WriteLine(string.Format("My full name is {0} {1}", employee.FirstName ,employee.LastName));
        }
    }

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
                "CREATE TABLE [Moth.Linq.Tests.Employee] (Id int NOT NULL PRIMARY KEY IDENTITY(1,1), UId uniqueidentifier, DateCreated DateTime NOT NULL, DateUpdated DateTime NULL,FirstName varchar(max), LastName varchar(MAX))")
                .Execute()
                .NonQuery();
        }
    }
}