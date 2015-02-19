using System.Diagnostics;

namespace Moth.Linq.Tests
{
    public class Employee : RecordBase<Employee>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public One<Department> Department { get; set; }

        protected override void OnCreated(Employee employee)
        {
            Trace.WriteLine(string.Format("My Id is :{0}", employee.Id));
            Trace.WriteLine(string.Format("My UniqueId is :{0}", employee.UId));
            Trace.WriteLine(string.Format("I am created on {0}", employee.DateCreated));
        }

        protected override void OnDeleted(Employee employee)
        {
            Trace.WriteLine("I'm deleted");
        }

        protected override void OnUpdated(Employee employee)
        {
            Trace.WriteLine(string.Format("I was created on {0}", employee.DateCreated));
            Trace.WriteLine(string.Format("I am updated on {0}", employee.DateUpdated.Value));
            Trace.WriteLine(string.Format("My full name is {0} {1}", employee.FirstName ,employee.LastName));
        }
    }

    public class Department : RecordBase<Department>
    {
        public string Name { get; set; }
    }
}