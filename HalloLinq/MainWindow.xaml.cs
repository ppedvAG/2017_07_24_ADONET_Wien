using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace HalloLinq
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private IEnumerable<Employee> GetEmployees()
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
        private IEnumerable<Order> GetOrders()
        {
            var connectionString = "Data Source=.;Initial Catalog=NORTHWND;Integrated Security=True";

            var orders = new List<Order>();

            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                // Alle Employees laden und in einer lokalen Liste Speichern und ausgeben
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
SELECT OrderId
     , EmployeeId
     , ShipName
    FROM Orders";

                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            orders.Add(new Order
                            {
                                Id = (int)reader["OrderId"],
                                EmployeeId = (int)reader["EmployeeId"],
                                ShipName = (string)reader["ShipName"]
                            });
                }
            }

            return orders;
        }

        private void AlleButton_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = GetEmployees();
        }
        private void RestrictionButton_Click(object sender, RoutedEventArgs eargs)
        {
            // Linq Syntax
            //var query = from e in GetData()
            //            where e.YearOfBirth < 1960
            //            select e;

            // Fluid API
            var query = GetEmployees().Where(e => e.YearOfBirth < 1960);

            dataGrid.ItemsSource = query;
        }
        private void OrderingButton_Click(object sender, RoutedEventArgs eargs)
        {
            // Linq Syntax
            //var query = from e in GetData()
            //            orderby e.YearOfBirth descending, e.Name
            //            select e;

            // Fluid API
            var query = GetEmployees().OrderByDescending(e => e.YearOfBirth)
                                 .ThenBy(e => e.Name);

            dataGrid.ItemsSource = query;
        }
        private void ProjectionButton_Click(object sender, RoutedEventArgs eargs)
        {
            // Linq Syntax
            //var query = from e in GetData()
            //            select new { e.Name, Geburtsjahr = e.YearOfBirth };

            // Fluid API
            var query = GetEmployees().Select(e => new { e.Name, Geburtsjahr = e.YearOfBirth });
            
            dataGrid.ItemsSource = query;
        }
        private void GroupingButton_Click(object sender, RoutedEventArgs eargs)
        {
            // Linq Syntax
            //var query = from e in GetData()
            //            group e by e.Name[0] into g
            //            orderby g.Key
            //            select new { AnfangsBuchstabe = g.Key, Employees = g};

            // Fluid API
            var query = GetEmployees().GroupBy(e => e.Name[0])
                                 .Select(g => new { AnfangsBuchstabe = g.Key, Employees = g })
                                 .OrderBy(g => g.AnfangsBuchstabe);

            dataGrid.ItemsSource = query;

            foreach (var item in query)
            {
                Debug.WriteLine($"----- {item.AnfangsBuchstabe} -----");

                foreach (var e in item.Employees)
                    Debug.WriteLine($"\t{e.Name}");
            }
        }
        private void PartitioningButton_Click(object sender, RoutedEventArgs eargs)
        {
            var query = GetEmployees().OrderBy(e => e.Id).Skip(3).Take(3);

            dataGrid.ItemsSource = query;
        }
        private void ElementOperatorsButton_Click(object sender, RoutedEventArgs eargs)
        {
            var employees = GetEmployees();

            //var employee = employees.First();
            //var employee = employees.FirstOrDefault();
            //var employee = employees.First(e => e.Name.StartsWith("M"));
            //var employee = employees.FirstOrDefault(e => e.Name.StartsWith("M"));

            //var employee = employees.Single();
            //var employee = employees.SingleOrDefault();
            //var employee = employees.Single(e => e.Name.StartsWith("L"));
            var employee = employees.SingleOrDefault(e => e.Name.StartsWith("R"));

            // Wird von SQL Server nicht unterstützt!
            //var employee = employees.Last();
            //var employee = employees.LastOrDefault();
            //var employee = employees.Last(e => e.Name.StartsWith("M"));
            //var employee = employees.LastOrDefault(e => e.Name.StartsWith("M"));

            elementOperatorsTextBlock.Text = employee?.Name;
        }
        private void QuanitfyingButton_Click(object sender, RoutedEventArgs eargs)
        {
            var employees = GetEmployees();

            var allBornIn1950 = employees.All(e => e.YearOfBirth == 1950);
            var anyBornIn1950 = employees.Any(e => e.YearOfBirth == 1950);

            quantifyingAllTextBlock.Text = $"{(allBornIn1950 ? "Alle" : "Nicht alle")} Employees sind im Jahr 1950 geboren.";
            quantifyingAnyTextBlock.Text = $"{(anyBornIn1950 ? "Mindestens einer" : "Keiner")} der Employees ist im Jahr 1950 geboren.";
        }
        private void AggregatingButton_Click(object sender, RoutedEventArgs eargs)
        {
            var employees = GetEmployees();

            countTextBlock.Text = $"{employees.Count()} employees in DB.";

            minTextBlock.Text = $"Der älsteste Employee ist geboren im Jahr {employees.Min(e => e.YearOfBirth)}";
            maxTextBlock.Text = $"Der jünste Employee ist geboren im Jahr {employees.Max(e => e.YearOfBirth)}";
            averageTextBlock.Text = $"Im Durchschnitt sind die Employees im Jahr {employees.Average(e => e.YearOfBirth): 00.0} geboren.";
            sumTextBlock.Text = $"Die Summe der Geburtsjahre ist {employees.Sum(e => e.YearOfBirth)}.";
        }
        private void InnerJoinButton_Click(object sender, RoutedEventArgs eargs)
        {
            var employees = GetEmployees();
            var orders = GetOrders();

            // Linq Syntax
            //var query = from e in employees
            //            join o in orders on e.Id equals o.EmployeeId
            //            select new { e.Name, o.ShipName };

            // Fluid API
            var query = employees.Join(
                inner: orders,
                outerKeySelector: e => e.Id,
                innerKeySelector: o => o.EmployeeId,
                resultSelector: (e, o) => new { e.Name, o.ShipName });

            dataGrid.ItemsSource = query;
        }
        private void GroupJoinButton_Click(object sender, RoutedEventArgs eargs)
        {
            var employees = GetEmployees();
            var orders = GetOrders();

            // Linq Syntax
            //var query = from e in employees
            //            join o in orders on e.Id equals o.EmployeeId into g
            //            select new { e.Name, Count = g.Count() };

            // Fluid API
            var query = employees.GroupJoin(
                inner: orders,
                outerKeySelector: e => e.Id,
                innerKeySelector: o => o.EmployeeId,
                resultSelector: (e, o) => new { e.Name, Count = o.Count() });

            dataGrid.ItemsSource = query;
        }
        private void CrossJoinButton_Click(object sender, RoutedEventArgs eargs)
        {
            var employees = GetEmployees();
            var orders = GetOrders();

            // LinqSyntax
            //var whatever = from e in employees
            //               from o in orders
            //               select new { e.Name, o.ShipName };

            // Fluid API
            //var whatever = employees.SelectMany(e => orders.Select(o => new { e.Name, o.ShipName }));
            var whatever = employees.CrossJoin(orders).Select(x => new { x.Item1.Name, x.Item2.ShipName });

            dataGrid.ItemsSource = whatever;
            countTextBlock.Text = whatever.Count().ToString();
        }
    }

    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T1, T2)> CrossJoin<T1, T2>(this IEnumerable<T1> outer, IEnumerable<T2> inner)
        {
            return outer.SelectMany(o => inner.Select(i => (o, i)));
        }
    }

    //public class Mitarbeiter
    //{
    //    public string Name { get; set; }
    //    public int Geburtsjahr { get; set; }
    //}
}
