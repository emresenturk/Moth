using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Moth.Configuration;
using Moth.Database;
using Moth.Database.MsSql;
using Moth.Extensions;
using Moth.Linq.Tests.TestClasses;
using NUnit.Framework;

namespace Moth.Linq.Tests
{
    [TestFixture]
    public partial class RecordTests
    {
        private readonly Random rand = new Random();
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
            var employee = new Employee { FirstName = "EmpOne", LastName = "Employeeson" };
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
            //Query.Create(@"EXEC sp_MSforeachtable @command1 = ""DROP TABLE ?""").Execute().NonQuery();
            DatabaseContainer.DefaultContainer.Dispose();
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

        private Tuple<string, string> GetRandomName()
        {
            var firstNames = new[] { "Randall", "Eleanor", "Jasmine", "Mattie", "Billy", "Tasha", "Joan", "Willie", "Lucille", "Saul", "Gordon", "Lloyd", "Vernon", "Emanuel", "Sergio", "Nathan", "Celia", "Laurence", "Jose", "Danny", "Arturo", "Alton", "Darrel", "Elsa", "Lynette", "Devin", "Belinda", "Evan", "Horace", "Conrad", "Jody", "Christopher", "Leland", "Lance", "Candice", "Rex", "Natasha", "Guadalupe", "Lynn", "Janice", "Pam", "Timmy", "Byron", "John", "Brad", "Stephanie", "Marvin", "Tricia", "Cindy", "Flora", "Kathryn", "Donna", "Walter", "Evelyn", "Jermaine", "Woodrow", "Joanne", "Preston", "Renee", "Lynne", "Michael", "Clyde", "Maria", "Myron", "Eva", "Dorothy", "Ebony", "Alexis", "Karl", "Alberto", "Thelma", "Edwin", "Charlie", "Verna", "Betty", "Jeremiah", "Marlon", "Raul", "Tony", "Pablo", "Adam", "Kirk", "Rosalie", "Dolores", "Laurie", "Clarence", "Nicolas", "Noel", "Vincent", "Marsha", "Donnie", "Melody", "Becky", "Joshua", "Mack", "Nora", "Candace", "Dennis", "Jack", "Sadie" };

            var lastNames = new[] {"Douglas", "Meyer", "Malone", "Logan", "Edwards", "Hunt", "Reeves", "Chavez", "Banks", "Gross", "Simmons", "Bridges", "Yates", "Webster", "Leonard", "Washington", "Abbott", "Vega", "Castro", "Murray", "Johnston", "Porter", "Gill", "Poole", "Ortiz", "Hogan", "Pierce", "Mitchell", "Kelley", "Mclaughlin", "Martin", "Bates", "Powers", "Crawford", "Santiago", "Stewart", "Baker", "Schneider", "Brewer", "Tyler", "Bailey", "Cain", "Welch", "Barrett", "King", "Jennings", "Houston", "Luna", "George", "Nichols", "Clayton", "Wells", "Reyes", "Keller", "Burton", "Benson", "Diaz", "Vasquez", "Henderson", "Sanchez", "Barber", "Hoffman", "Cox", "Mccarthy", "Ramos", "Sullivan", "Marsh", "Gibbs", "Wise", "Elliott", "Burns", "Munoz", "Salazar", "Fernandez", "Allen", "Phelps", "Lynch", "Mckinney", "Oliver", "Beck", "Wilkerson", "Morgan", "Hayes", "Hawkins", "Alvarez", "Ferguson", "Robinson", "Olson", "Carson", "Gonzales", "Dixon", "Newman", "Wheeler", "Hernandez", "Rodgers", "Parsons", "Richards", "Herrera", "Holloway", "Thornton"};

            var firstName = firstNames[rand.Next(0, 99)];
            var lastName = lastNames[rand.Next(0, 99)];
            return new Tuple<string, string>(firstName, lastName);
        }
    }
}