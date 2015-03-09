using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Moth.Configuration;
using NUnit.Framework;
using Moth.Extensions;
namespace Moth.Database.MsSql.Tests
{
    [TestFixture]
    public class QueryExtensionTests
    {
        private bool tablesAreCreated;
        private bool entityInserted;

        [TestFixtureSetUp]
        public void Setup()
        {
            Trace.WriteLine(string.Format("Read Configuration Started At :{0}", DateTime.Now));
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
            Trace.WriteLine(string.Format("Read Configuration Ended At :{0}", DateTime.Now));
        }

        //[Test]
        public void CreateTableTests()
        {
            if (tablesAreCreated)
            {
                return;
            }

            var query = new Query("CREATE TABLE TestTable(Id int, Name varchar(max))");
            var result = query.Execute().NonQuery();
            tablesAreCreated = true;
        }

        //[Test]
        public void InsertTests()
        {
            if (!tablesAreCreated)
            {
                CreateTableTests();
            }

            if (entityInserted)
            {
                return;
            }

            var result = new Query("INSERT INTO TestTable(Id, Name) VALUES (@Id, @Name)", new Parameter("@Id", 1), new Parameter("@Name", "Test")).Execute().NonQuery();
            Assert.AreEqual(1, result);
            entityInserted = true;
        }

        //[Test]
        public void EntityRetrievalTests()
        {
            if (!tablesAreCreated)
            {
                CreateTableTests();
            }

            if (!entityInserted)
            {
                InsertTests();
            }

            var count = new Query("SELECT COUNT(*) FROM TestTable").Execute().Scalar<int>();
            Assert.AreEqual(1, count);
            var entity = new Query("SELECT * FROM TestTable").Execute().Reader().FirstOrDefault();
            Assert.NotNull(entity);
            var sameEntity = new Query("SELECT * FROM TestTable").Execute().Retrieve()[0];
            Assert.NotNull(sameEntity);
        }

        [Test]
        public void ReadTests()
        {
            if (!tablesAreCreated)
            {
                try
                {
                    CreateTableTests();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            var insertQuery = new Query("INSERT INTO TestTable(Id, Name) VALUES (@Id, @Name)");
            for (int i = 0; i < 20; i++)
            {
                insertQuery.Parameters["@Id"] = i;
                insertQuery.Parameters["@Name"] = "Test" + i;
                insertQuery.Execute().NonQuery();
            }

            var entities = Query.Create("SELECT * FROM TestTable").Execute().Reader();
            foreach (var entity in entities)
            {
                Trace.WriteLine(string.Format("Id:{0}, Name:{1}", entity["Id"], entity["Name"]));
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            var query = new Query("DROP TABLE TestTable");
            query.Execute().NonQuery();
            DatabaseContainer.DefaultContainer.Dispose();
        }
        
    }
}