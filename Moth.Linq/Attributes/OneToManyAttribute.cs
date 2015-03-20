using System;

namespace Moth.Linq.Attributes
{
    public class OneToManyAttribute : Attribute
    {
        public string Relation { get; set; }
        public OneToManyAttribute(string relation)
        {
            Relation = relation;
        }
    }
}