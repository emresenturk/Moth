using System;
using System.Collections.Concurrent;
using Moth.Configuration;

namespace Moth.Database
{
    public partial class DatabaseContainer
    {
        private bool disposed;

        private static readonly ConcurrentDictionary<Type, Func<IDatabase>> DefaultConstructor = new ConcurrentDictionary<Type, Func<IDatabase>>();
        private static readonly ConcurrentDictionary<Type, Func<IDatabaseConfiguration, IDatabase>> ConfiguredConstructor = new ConcurrentDictionary<Type, Func<IDatabaseConfiguration, IDatabase>>();
        private static readonly ConcurrentDictionary<string, Type> DatabaseTypes = new ConcurrentDictionary<string, Type>();
        private static readonly ConcurrentDictionary<string, IDatabaseConfiguration> Configurations = new ConcurrentDictionary<string, IDatabaseConfiguration>();

        public static IDatabaseContainer DefaultContainer;

        static DatabaseContainer()
        {
            if (DefaultContainer == null)
            {
                DefaultContainer = new DatabaseContainer();
            }
        }

        ~DatabaseContainer()
        {
            Dispose(false);
        }

        public bool HasRegisteredItems
        {
            get { return DefaultConstructor.Count > 0 || ConfiguredConstructor.Count > 0; }
        }

        public static void Clear()
        {
            DefaultConstructor.Clear();
            ConfiguredConstructor.Clear();
            DatabaseTypes.Clear();
            Configurations.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static IDatabaseConfiguration GetConfiguration(string name)
        {
            IDatabaseConfiguration configuration;
            if (!Configurations.TryGetValue(name, out configuration))
            {
                throw new ArgumentException(
                    string.Format("No configuration or connection string set with name \"{0}\"", name), "name");
            }
            return configuration;
        }

        private static Type GetDatabaseType(string name)
        {
            Type type;
            if (!DatabaseTypes.TryGetValue(name, out type))
            {
                throw new ArgumentException(
                    string.Format("No database type registered with name \"{0}\"", name), "name");
            }

            return type;
        }

        private static void EnsureNameIsGiven(IDatabaseConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration.Name))
            {
                throw new ArgumentNullException("configuration",
                    string.Format(@"""Name"" property of configuration parameter cannot be null or empty."));
            }
        }

        private static Func<IDatabase> GetDefaultConstructor(Type type)
        {
            var defaultConstructor = type.GetConstructor(Type.EmptyTypes);
            if (defaultConstructor == null)
            {
                throw new MissingMethodException(string.Format("Parameterless constructor is not found for class {0}", type.Name));
            }

            return () => (IDatabase)defaultConstructor.Invoke(null);
        }

        private static Func<IDatabaseConfiguration, IDatabase> GetConfiguredConstuctor(Type type)
        {
            var configuredConstructor = type.GetConstructor(new[] { typeof(IDatabaseConfiguration) });
            if (configuredConstructor == null)
            {
                throw new MissingMethodException(string.Format("Constructor with IDatabaseConfiguration argument is not implemented in class {0}", type.Name));
            }

            return dconf => (IDatabase)configuredConstructor.Invoke(new object[] { dconf });
        }

        private static Func<IDatabaseConfiguration, T> GetConfiguredConstructor<T>()
        {
            var type = typeof(T);
            var configuredConstructor = type.GetConstructor(new[] { typeof(IDatabaseConfiguration) });
            if (configuredConstructor == null)
            {
                throw new MissingMethodException(string.Format("Constructor with IDatabaseConfiguration argument is not implemented in class {0}", type.Name));
            }

            return dconf => (T)configuredConstructor.Invoke(new object[] { dconf });
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // dispose disposables
            }

            disposed = true;
        }
    }
}