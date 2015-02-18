using System;
using System.Linq;
using Moth.Expressions;

namespace Moth.Linq
{
    public class One<T> where T : class 
    {
        private Guid uId;

        public T Value
        {
            get { return GetValue(); }
        }

        private T GetValue()
        {
            using (var executor = new ExpressionExecutor())
            {
                var query = new ExpressionQuery();
                query.AddFilter(new BinaryExpression(new MemberExpression("UId", typeof (Guid), typeof (T)),
                    BinaryOperator.Equal, new ParameterExpression(new Parameter(uId))));
                return executor.ExecuteRetrieve<T>(query).FirstOrDefault();
            }
        }

        public static implicit operator One<T>(Guid uId)
        {
            return new One<T> {uId = uId};
        }

        public static implicit operator Guid(One<T> one)
        {
            return one.uId;
        }

        public static implicit operator T(One<T> one)
        {
            return one.Value;
        }
    }
}