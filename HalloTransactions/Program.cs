using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace HalloTransactions
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Data Source=.;Initial Catalog=NORTHWND;Integrated Security=True";

            var employees = new List<Employee>();
            var employeesTuple = new List<(int id, string name, DateTime birthdate)>();

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
     , BirthDate
    FROM Employees";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                Id = (int)reader["EmployeeId"],
                                Name = (string)reader["Name"],
                                Birthdate = (DateTime)reader["Birthdate"]
                            });

                            employeesTuple.Add(
                            (
                                (int)reader["EmployeeId"],
                                (string)reader["Name"],
                                (DateTime)reader["Birthdate"]
                            ));
                        }
                    }
                }
            }

            foreach (var e in employees)
            {
                Console.WriteLine($"{e.Id,2} - {e.Name,-20} - {e.Birthdate.ToString("dd.MM.yyyy")}");
            }
            Console.WriteLine();
            foreach (var e in employeesTuple)
            {
                Console.WriteLine($"{e.id,2} - {e.name,-20} - {e.birthdate.ToString("dd.MM.yyyy")}");
            }
            Console.WriteLine();


            // Alle Employees 1 Jahr jünger machen
            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        var affectedEmployees = 0;
                        foreach (var e in employees)
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                command.CommandText = "UPDATE Employees SET Birthdate = @date WHERE EmployeeId = @id";
                                command.Parameters.AddWithValue("@date", e.Birthdate.AddYears(1));
                                command.Parameters.AddWithValue("@id", e.Id);

                                affectedEmployees += command.ExecuteNonQuery();
                            }

                            if (e.Id == 5)
                                throw new Exception("Eine Id hatte den Wert 5! :O");
                        }

                        if (affectedEmployees != employees.Count)
                            throw new Exception("There was a problem updating the Employees.");

                        Console.WriteLine($"{affectedEmployees} rows affected.");
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            Console.ReadKey();
        }
    }
}