using System.Configuration;
using Moth.Configuration;
using Moth.Database;
using Moth.Database.MsSql;
using Moth.Extensions;
using NUnit.Framework;

namespace Moth.Linq.Tests
{
    [TestFixture]
    public partial class VisitorTests
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

        [TestFixtureTearDown]
        public void TearDown()
        {
            Query.Create(@"EXEC sp_MSforeachtable @command1 = ""DROP TABLE ?""").Execute().NonQuery();
        }

        private void CreateTable()
        {
            TearDown();
            Query.Create(
                "CREATE TABLE [Moth.Linq.Tests.TestClasses.Employee] (Id int NOT NULL PRIMARY KEY IDENTITY(1,1), UId uniqueidentifier, DateCreated DateTime NOT NULL, DateUpdated DateTime NULL,FirstName varchar(max), LastName varchar(MAX), Department uniqueidentifier)")
                .Execute()
                .NonQuery();
            Query.Create(
                "CREATE TABLE [Moth.Linq.Tests.TestClasses.Department](Id int NOT NULL PRIMARY KEY IDENTITY(1,1), UId uniqueidentifier, DateCreated DateTime NOT NULL, DateUpdated DateTime NULL, Name varchar(max))").Execute().NonQuery();
        }
    }
}