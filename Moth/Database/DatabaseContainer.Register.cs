using System;
using Moth.Configuration;

namespace Moth.Database
{
    public partial class DatabaseContainer
    {
        public void Register<T>() where T : class, IDatabase, new()
        {
            DefaultConstructor.TryAdd(typeof(T), () => new T());
            ConfiguredConstructor.TryAdd(typeof(T), GetConfiguredConstructor<T>());
        }

        public void Register(Type databaseType)
        {
            DefaultConstructor.TryAdd(databaseType, GetDefaultConstructor(databaseType));
            ConfiguredConstructor.TryAdd(databaseType, GetConfiguredConstuctor(databaseType));
        }

        public void Register<T>(IDatabaseConfiguration configuration) where T : class, IDatabase, new()
        {
            EnsureNameIsGiven(configuration);
            DefaultConstructor.TryAdd(typeof(T), () => new T());
            ConfiguredConstructor.TryAdd(typeof(T), GetConfiguredConstructor<T>());
            var name = configuration.Name;
            DatabaseTypes.TryAdd(name, typeof(T));
            Configurations.TryAdd(name, configuration);
        }

        public void Register(Type databaseType, IDatabaseConfiguration configuration)
        {
            EnsureNameIsGiven(configuration);
            DefaultConstructor.TryAdd(databaseType, GetDefaultConstructor(databaseType));
            ConfiguredConstructor.TryAdd(databaseType, GetConfiguredConstuctor(databaseType));
            var name = configuration.Name;
            DatabaseTypes.TryAdd(name, databaseType);
            Configurations.TryAdd(name, configuration);
        }
    }
}