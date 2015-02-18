using System;
using System.Linq;
using Moth.Configuration;

namespace Moth.Database
{
    public partial class DatabaseContainer : IDatabaseContainer
    {
        public IDatabase GetInstance()
        {
            if (Configurations.Any())
            {
                var configuration = Configurations.First();
                var type = DatabaseTypes[configuration.Key];
                return ConfiguredConstructor[type](configuration.Value);
            }

            var ctor = DefaultConstructor.First().Value;
            return ctor();
        }

        public T GetInstance<T>() where T : class, IDatabase, new()
        {
            return (T)GetInstance(typeof(T));
        }

        public IDatabase GetInstance(Type databaseType)
        {
            var instance = DefaultConstructor[databaseType]();
            return instance;
        }

        public IDatabase GetInstance(string name)
        {
            var configuration = GetConfiguration(name);
            var type = GetDatabaseType(name);
            return ConfiguredConstructor[type](configuration);
        }

        public T GetInstance<T>(string name) where T : class, IDatabase, new()
        {
            return GetInstance(typeof(T), name) as T;
        }

        public IDatabase GetInstance(Type databaseType, string name)
        {
            var configuration = GetConfiguration(name);
            return ConfiguredConstructor[databaseType](configuration);
        }

        public T GetInstance<T>(IDatabaseConfiguration configuration) where T : class, IDatabase, new()
        {
            var databaseType = typeof(T);
            return GetInstance(databaseType, configuration) as T;
        }

        public IDatabase GetInstance(Type databaseType, IDatabaseConfiguration configuration)
        {
            return ConfiguredConstructor[databaseType](configuration);
        }
    }
}