using System;
using System.Linq;

namespace HalloLinqToSql
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new NorthwindClassesDataContext())
            {
                var employees = context.Employees
                    //.Where(e => e.BirthDate.Value.Year > 1950)
                    .OrderBy(e => e.FirstName)
                    .Select(e => new { e.EmployeeID, e.FirstName, e.LastName})
                    .ToList();

                foreach (var e in employees)
                    Console.WriteLine($"Id: {e.EmployeeID} - {e.FirstName, 10} {e.LastName, 10}");

                //var employee = context.Employees.First();
                //employee.FirstName = "Stanislaus";
                //context.SubmitChanges();                    // in EntityFramework: context.SaveChanges()

                //var newEmployee = new Employee
                //{
                //    FirstName = "theNewOne",
                //    LastName = "Maier"
                //};
                //context.Employees.InsertOnSubmit(newEmployee); // in EntityFramework: cotnext.Employees.Add(employee)
                //context.SubmitChanges();

                //var employeeToDelete = context.Employees.SingleOrDefault(e => e.FirstName == "theNewOne" && e.LastName == "Maier");
                //context.Employees.DeleteOnSubmit(employeeToDelete);       // in EntityFramework: context.Employees.Remove(employee)
                //context.SubmitChanges();
            }

            Console.ReadLine();
        }
    }
}
