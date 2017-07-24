using System;
using System.Data.SqlClient;

namespace HalloDatenbank
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Data Source=.;Initial Catalog=NORTHWND;Integrated Security=True";

            using (var connection = new SqlConnection(connectionString))
            {
                //connection.ConnectionString = connectionString;
                connection.Open();

                // Skalare Werte laden
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "SELECT COUNT(*) FROM Employees";

                    var count = (int)command.ExecuteScalar();
                    Console.WriteLine($"{count} Emplyoees in DB");
                    Console.WriteLine();
                }

                // Datensätze laden
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "SELECT * FROM Employees";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var firstname = reader.GetString(2);
                            var lastname = (string)reader["LastName"];

                            var bithdate = reader.GetDateTime(5);
                            var hiredate = (DateTime)reader["HireDate"];

                            Console.WriteLine($"{firstname,-10} {lastname,-10} | {bithdate.ToString("dd.MM.yyyy")} | {hiredate.ToString("dd.MM.yyyy")}");
                        }
                    }
                }

                // No Query
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "UPDATE Employees SET Firstname = 'Luise' WHERE EmployeeId = 1";

                    var affectedRows = command.ExecuteNonQuery();
                    Console.WriteLine($"\n{affectedRows} Rows affected.\n");
                }

                // Übung Query
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    Console.Write("Nach welchem Vornamen soll gesucht werden? ");
                    var eingabe = Console.ReadLine();

                    command.CommandText = $"SELECT * FROM Employees WHERE FirstName LIKE @search + '%'";
                    command.Parameters.AddWithValue("@search", eingabe);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var firstname = reader.GetString(2);
                            var lastname = (string)reader["LastName"];

                            var bithdate = reader.GetDateTime(5);
                            var hiredate = (DateTime)reader["HireDate"];

                            Console.WriteLine($"{firstname,-10} {lastname,-10} | {bithdate.ToString("dd.MM.yyyy")} | {hiredate.ToString("dd.MM.yyyy")}");
                        }
                    }
                }
                Console.WriteLine();

                // Stored Procedure
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    //command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = "CustOrderHist @custId";
                    var customerId = "ANTON";
                    command.Parameters.AddWithValue("@custId", customerId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var productName = (string)reader["ProductName"];
                            var total = (int)reader["Total"];

                            Console.WriteLine($"{productName, -30} - {total, 2}");
                        }
                    }
                }
            }

            Console.ReadKey();
        }
    }
}