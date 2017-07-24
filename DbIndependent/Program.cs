using Microsoft.Data.Sqlite;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace DbIndependent
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString;
            DbProviderFactory factory;

            var bedingung = 7 < 9;

            if (bedingung)
            {
                connectionString = "Data Source=.;Initial Catalog=NORTHWND;Integrated Security=True";
                factory = SqlClientFactory.Instance;
            }
            else
            {
                connectionString = "Connectionstring to Sqlite DB";
                factory = SqliteFactory.Instance;
            }

            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    Console.Write("Nach welchem Vornamen soll gesucht werden? ");
                    var eingabe = Console.ReadLine();

                    command.CommandText = $"SELECT * FROM Employees WHERE FirstName LIKE @search + '%'";

                    //var parameter = command.CreateParameter();
                    //parameter.ParameterName = "@search";
                    //parameter.Value = eingabe;

                    //command.Parameters.AddWithValue("@search", eingabe, command);
                    command.AddParameterWithValue("@search", eingabe);


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
            }

            Console.ReadKey();
        }
    }
}