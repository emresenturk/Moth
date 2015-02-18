using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Moth.Configuration;
using Moth.Expressions;
using NUnit.Framework;
using Handy.Statistics;
namespace Moth.Database.MsSql.Tests
{
    [TestFixture]
    public class QueryTests
    {
        private Executor executor;
        private List<Tuple<int, string, DateTime>> matchList;
            
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
            Trace.WriteLine(string.Format("Executor CTOR Run At :{0}", DateTime.Now));
            executor = new Executor("Main");
            Trace.WriteLine(string.Format("Executor CTOR Ended At :{0}", DateTime.Now));
        }

        [Test]
        public void a_ReadFiles()
        {
            Trace.WriteLine("Read Files Started");
            const string MatchPath = "C:\\Users\\mesh\\Desktop\\helehele\\";
            matchList = new List<Tuple<int, string, DateTime>>();
            for (var i = 906; i < 50000 /*650000*/; i++)
            {
                if (!File.Exists(MatchPath + i))
                {
                    continue;
                }

                var fileContents = File.ReadAllText(MatchPath + i);
                matchList.Add(new Tuple<int, string, DateTime>(i, fileContents.Substring(0, fileContents.Length > 500 ? 500 : fileContents.Length), DateTime.Now));
            }
            Trace.WriteLine("Read Files Ended");
        }


        [Test]
        public void b_SingleHighLoadTest()
        {
            CreateTable();
            Inserts();
        }
        
        public void CreateTable()
        {
            var queryOne = new Query("CREATE TABLE PlayerJson (Id int, Json varchar(MAX))");
            var queryTwo = new Query("CREATE TABLE MatchJson(Id int, Json varchar(MAX), DummyDate datetime)");
            executor.ExecuteNonQuery(queryOne);
            executor.ExecuteNonQuery(queryTwo);
        }

        public void Inserts()
        {
            var matchInsertQuery = new Query("INSERT INTO MatchJson(Id, Json, DummyDate) VALUES (@Id, @Json, @DummyDate)");
            var perRecordDurations = new List<long>();
            var executionDurations = new List<long>();
            var parameterSetDurations = new List<long>();
            var perRecord = new Stopwatch();
            var parameterSetStopWatch = new Stopwatch();
            var executeQueryStopWatch = new Stopwatch();
            foreach (var match in matchList)
            {
                if (string.IsNullOrEmpty(match.Item2))
                {
                    continue;
                }

                perRecord.Reset();
                parameterSetStopWatch.Reset();
                executeQueryStopWatch.Reset();

                perRecord.Start();
                parameterSetStopWatch.Start();
                
                matchInsertQuery.Parameters["@Id"] = match.Item1;
                matchInsertQuery.Parameters["@Json"] = match.Item2;
                matchInsertQuery.Parameters["@DummyDate"] = match.Item3;
                
                parameterSetStopWatch.Stop();
                parameterSetDurations.Add(parameterSetStopWatch.ElapsedTicks);
                
                executeQueryStopWatch.Start();
                
                executor.ExecuteNonQuery(matchInsertQuery);
                
                executeQueryStopWatch.Stop();
                executionDurations.Add(executeQueryStopWatch.ElapsedTicks);

                perRecord.Stop();
                perRecordDurations.Add(perRecord.ElapsedTicks);
            }
            Trace.WriteLine(string.Format("Stopwatch Freqency\t{0}", Stopwatch.Frequency));
            Trace.WriteLine(string.Format("Total Ticks\t{0}", perRecordDurations.Sum()));
            Trace.WriteLine(string.Format("Average Ticks\t{0}", perRecordDurations.Average()));
            Trace.WriteLine(string.Format("Max ticks\t{0}", perRecordDurations.Max()));
            Trace.WriteLine(string.Format("Min ticks\t{0}", perRecordDurations.Min()));
            Trace.WriteLine(string.Format("Average parameter set duration\t{0}", parameterSetDurations.Average()));
            Trace.WriteLine(string.Format("Min parameter set duration\t{0}", parameterSetDurations.Min()));
            Trace.WriteLine(string.Format("Max parameter set duration\t{0}", parameterSetDurations.Max()));
            Trace.WriteLine(string.Format("Average query execution duration\t{0}", executionDurations.Average()));
            Trace.WriteLine(string.Format("Min query execution duration\t{0}", executionDurations.Min()));
            Trace.WriteLine(string.Format("Max query execution duration\t{0}", executionDurations.Max()));
            Trace.WriteLine(string.Format("Variance of execution duration\t{0}", executionDurations.Variance()));
            Trace.WriteLine(string.Format("Std. Dev. of execution duration\t{0}", executionDurations.StdDev()));

            //const string PlayerMatchPath = "D:\\hele2";
            //var playerMatchQuery = new Query("INSERT INTO PlayerJson(Id, Json) VALUES (@Id, @Json)");
            //var folderPath = string.Format("{0}\\{1}\\", PlayerMatchPath, 1);
            //for (var i = 1; i < 820000; i++)
            //{
            //    if (i % 10000 == 0)
            //    {
            //        folderPath = string.Format("{0}\\{1}\\", PlayerMatchPath, i);
            //    }

            //    if (!File.Exists(folderPath + i))
            //    {
            //        continue;
            //    }

            //    var fileContents = File.ReadAllText(folderPath + i);
            //    playerMatchQuery.Parameters["@Id"] = i;
            //    playerMatchQuery.Parameters["@Json"] = fileContents;
            //    executor.ExecuteNonQuery(playerMatchQuery);
            //}
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            var query = new Query(@"EXEC sp_MSforeachtable @command1 = ""DROP TABLE ?""");
            executor.ExecuteNonQuery(query);
            executor.Dispose();
            DatabaseContainer.DefaultContainer.Dispose();
        }
    }
}
