using System;
using System.Collections.Generic;
using Moth.Data;
using Moth.Database;

namespace Moth
{
    public class Executor : IDisposable
    {
        private bool disposed;
        protected IDatabase Database;
        internal Query ExtensionQuery;

        public Executor()
        {
            Database = GetDatabase(null);
        }

        public Executor(string databaseName)
        {
            Database = GetDatabase(databaseName);
        }

        ~Executor()
        {
            Dispose(false);
        }

        public IEnumerable<Entity> ExecuteReader(IQuery query)
        {
            return Database.Read(query);
        }

        public IList<Entity> ExecuteRetrieve(IQuery query)
        {
            return Database.Retrieve(query);
        }

        public object ExecuteScalar(IQuery query)
        {
            return Database.Retrieve(query)[0][0];
        }

        public T ExecuteScalar<T>(IQuery query) where T : struct
        {
            return (T)ExecuteScalar(query);
        }

        public int ExecuteNonQuery(IQuery query)
        {
            return Database.NonQuery(query);
        }

        private IDatabase GetDatabase(string databaseName)
        {
            return string.IsNullOrEmpty(databaseName)
                ? DatabaseContainer.DefaultContainer.GetInstance()
                : DatabaseContainer.DefaultContainer.GetInstance(databaseName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal void SetDatabase(string name)
        {
            Database = GetDatabase(name);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (Database != null)
                {
                    Database.Dispose();
                }
            }

            disposed = true;
        }
    }
}
