using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DelegatesInPraxis
{
    public delegate bool MyDelegate(Employee employee);
    // Action       -> void
    // Predicate    -> bool
    // Func

    class Program
    {
        static void Main(string[] args)
        {
            var employees = GetData();

            //MyDelegate del = new MyDelegate(Bedingung);
            //Func<Employee, bool> del = new Func<Employee, bool>(Bedingung);
            //var del = new Func<Employee, bool>(Bedingung);
            //Func<Employee, bool> del = Bedingung;
            //var query = Abfrage(employees, del);

            //var query = Abfrage(employees, Bedingung);

            //var query = Abfrage(employees, delegate(Employee e)
            //{
            //    return e.YearOfBirth >= 1960;
            //});

            //var query = Abfrage(employees, (Employee e) =>
            //{
            //    return e.YearOfBirth >= 1960;
            //});

            //var query = Abfrage(employees, (e) => e.YearOfBirth >= 1960);
            var query =  Abfrage(employees, e => e.YearOfBirth >= 1960);
            var linqQuery = employees.Where(Bedingung);

            foreach (var e in query)
            {
                Console.WriteLine($"Id: {e.Id, 2} | {e.Name, 20} | {e.YearOfBirth }");
            }
            Console.ReadKey();
        }

        private static bool Bedingung(Employee e)
        {
            return e.Name.StartsWith("M");
        }

        private static IEnumerable<Employee> Abfrage(
            IEnumerable<Employee> employees, 
            Func<Employee, bool> predicate)
        {
            var query = new List<Employee>();

            foreach (var e in employees)
                if (predicate(e))
                    query.Add(e);

            return query;
        }

        private static IEnumerable<Employee> GetData()
        {
            var connectionString = "Data Source=.;Initial Catalog=NORTHWND;Integrated Security=True";

            var employees = new List<Employee>();

            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                // Alle Employees laden und in einer lokalen Liste Speichern und ausgeben
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
SELECT EmployeeId
     , Name = FirstName + ' ' + LastName
     , YearOfBirth = YEAR(BirthDate)
    FROM Employees";

                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            employees.Add(new Employee
                            {
                                Id = (int)reader["EmployeeId"],
                                Name = (string)reader["Name"],
                                YearOfBirth = (int)reader["YearOfBirth"]
                            });
                }
            }

            return employees;
        }
    }
}