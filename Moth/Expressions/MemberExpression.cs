using System;
using System.Linq;

namespace Moth.Expressions
{
    public class MemberExpression : IQueryExpression
    {
        public MemberExpression()
        {
        }

        internal MemberExpression(string memberName, Type objectType)
        {
            MemberName = memberName;
            ObjectType = objectType;
            ObjectName = objectType.Name;
        }

        public MemberExpression(string memberName, Type memberType, Type objectType)
        {
            MemberName = memberName;
            MemberType = memberType;
            ObjectType = objectType;
            ObjectName = objectType.Name;
            if (objectType.Namespace != null)
            {
                Namespace = objectType.Namespace.Split('.').Where(i => !string.IsNullOrEmpty(i)).ToArray();
            }
        }

        public string[] Namespace { get; set; }
        public string MemberName { get; set; }
        public Type ObjectType { get; set; }
        public Type MemberType { get; set; }
        public string ObjectName { get; set; }
    }
}