using SQLServerDeneme.Models;
using System.Data;
using System.Data.SqlClient;

namespace SQLServerDeneme.DataAccessLayer
{
    public class Employee_DAL
    {
        SqlConnection _connection = null;
        SqlCommand _command = null;

        public static IConfiguration Configuration { get; set; }

        private string GetConnectionString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            return Configuration.GetConnectionString("BloggingDatabase");
        }
        public List<Employee> GetAll()
        {
            List<Employee> employeelist = new List<Employee>();
            using( _connection =new SqlConnection(GetConnectionString()))
            {
                _command = _connection.CreateCommand();
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = "[DBO].[usp_Get_Employees]";
                _connection.Open();
                SqlDataReader datareader = _command.ExecuteReader();

                while (datareader.Read()) 
                {
                    Employee employee = new Employee();
                    employee.Id = Convert.ToInt32(datareader["Id"]);
                    employee.FirstName = datareader["FirstName"].ToString();
                    employee.LastName = datareader["LastName"].ToString();
                    employee.DateOfBirth = Convert.ToDateTime(datareader["DateOfBirth"]);
                    employee.Email = datareader["Email"].ToString();
                    employee.Salary = (float)Convert.ToDouble(datareader["Salary"]);
                    employeelist.Add(employee);
                }
                _connection.Close();

            }
            return employeelist;
        }
        private readonly string _connectionString;

        public Employee_DAL(string connectionString)
        {
            _connectionString = connectionString;

        }

        public bool CreateEmployee(Employee employee)
        {
            int id = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO Employees (FirstName, LastName, DateOfBirth, Email, Salary)
                             VALUES (@FirstName, @LastName, @DateOfBirth, @Email, @Salary)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                    command.Parameters.AddWithValue("@Email", employee.Email);
                    command.Parameters.AddWithValue("@Salary", employee.Salary);

                    try
                    {
                        connection.Open();
                        id = command.ExecuteNonQuery();
                        return id > 0 ? true : false;
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine("Veritabanı hatası: " + ex.Message);
                        return false;
                    }
                }
            }
        }
        public Employee GetEmployeeById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Employees WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Employee employee = new Employee
                            {
                                Id = (int)reader["Id"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"],
                                DateOfBirth = (DateTime)reader["DateOfBirth"],
                                Email = (string)reader["Email"],
                                Salary = (float)(decimal)reader["Salary"]
                            };
                            return employee;
                        }
                        else
                        {
                            return null; // Belirtilen Id'ye sahip çalışan bulunamadı
                        }
                    }
                }
            }
        }

        public bool UpdateEmployee(Employee employee)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE Employees 
                         SET FirstName = @FirstName, 
                             LastName = @LastName, 
                             DateOfBirth = @DateOfBirth, 
                             Email = @Email, 
                             Salary = @Salary 
                         WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", employee.Id);
                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@LastName", employee.LastName);
                    command.Parameters.AddWithValue("@DateOfBirth", employee.DateOfBirth);
                    command.Parameters.AddWithValue("@Email", employee.Email);
                    command.Parameters.AddWithValue("@Salary", employee.Salary);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (SqlException ex)
                    {
                        // Veritabanı işlemi sırasında bir hata oluştuğunda burada işleyebiliriz.
                        Console.WriteLine("Veritabanı hatası: " + ex.Message);
                        return false;
                    }
                }
            }
        }
        public bool DeleteEmployee(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Employees WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch (SqlException ex)
                    {
                        // Veritabanı işlemi sırasında bir hata oluştuğunda burada işleyebiliriz.
                        Console.WriteLine("Veritabanı hatası: " + ex.Message);
                        return false;
                    }
                }
            }
        }
    }
}
