using Moth.Linq.Attributes;

namespace Moth.Linq.Tests.TestClasses
{
    public class Department : RecordBase<Department>
    {
        public string Name { get; set; }

        [OneToMany("Department")]
        public Many<Employee> Employees { get; set; }
    }
}