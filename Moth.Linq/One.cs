using System;
using System.Linq;
using Moth.Expressions;

namespace Moth.Linq
{
    public class One<T> where T : class, IModel
    {
        public Guid UId { get; set; }

        public One()
        {
        }

        public One(Guid uId)
        {
            UId = uId;
        }

        public T Entity
        {
            get { return GetEntity(); }
        }

        private T GetEntity()
        {  
            using (var executor = new ExpressionExecutor())
            {
                var query = new ExpressionQuery();
                query.AddType(typeof(T));
                query.AddFilter(new BinaryExpression(new MemberExpression("UId", typeof (Guid), typeof (T)),
                    BinaryOperator.Equal, new ParameterExpression(new Parameter(UId))));
                return executor.ExecuteRetrieve<T>(query).FirstOrDefault();
            }
        }

        public static implicit operator One<T>(Guid uId)
        {
            return new One<T> {UId = uId};
        }

        public static implicit operator Guid(One<T> one)
        {
            return one.UId;
        }

        public static implicit operator T(One<T> one)
        {
            return one.Entity;
        }

        public static implicit operator One<T>(T entity)
        {
            return new One<T> {UId = entity.UId};
        }

        public static bool operator ==(One<T> one, Guid uId)
        {
            return one != null && one.UId == uId;
        }

        public static bool operator !=(One<T> one, Guid uId)
        {
            return !(one == uId);
        }
    }
}