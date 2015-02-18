using System;
using System.Collections.Generic;
using Moth.Configuration;
using Moth.Data;
using Moth.Expressions;

namespace Moth.Database
{
    public abstract class Database : IDatabase
    {
        public IDatabaseConfiguration Configuration { get; protected set; }

        public Database()
        {
        }

        public Database(IDatabaseConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual IList<Entity> Retrieve(IQuery query)
        {
            return query is Query ? RetrieveByText(query as Query) : RetrieveByExpression(query as ExpressionQuery);
        }

        public virtual IEnumerable<Entity> Read(IQuery query)
        {
            return query is Query ? ReadByText(query as Query) : ReadByExpression(query as ExpressionQuery);
        }

        public virtual Entity Create(Entity entity, TypeExpression entityType)
        {
            throw new NotImplementedException();
        }

        public virtual Entity Update(Entity entity, TypeExpression entityType)
        {
            throw new NotImplementedException();
        }

        public virtual Entity Delete(Entity entity, TypeExpression entityType)
        {
            throw new NotImplementedException();
        }

        public virtual int NonQuery(IQuery query)
        {
            return query is Query ? NonQueryByText(query as Query) : NonQueryByExpression(query as ExpressionQuery);
        }

       

        protected virtual IList<Entity> RetrieveByText(Query query)
        {
            throw new NotImplementedException();
        }

        protected virtual IList<Entity> RetrieveByExpression(ExpressionQuery query)
        {
            throw new NotImplementedException();
        }

        protected virtual IEnumerable<Entity> ReadByText(Query query)
        {
            throw new NotImplementedException();
        }

        protected virtual IEnumerable<Entity> ReadByExpression(ExpressionQuery query)
        {
            throw new NotImplementedException();
        }

        protected virtual int NonQueryByText(Query query)
        {
            throw new NotImplementedException();
        }

        protected virtual int NonQueryByExpression(ExpressionQuery query)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }        
    }
}