using System;

namespace Moth.Linq.Attributes
{
    public class OneToManyAttribute : Attribute
    {
        public string RelationName { get; set; }
        public OneToManyAttribute(string relationName)
        {
            RelationName = relationName;
        }
    }
}