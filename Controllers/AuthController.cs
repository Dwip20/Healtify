using Healthify.Data;
using Healthify.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Healthify.Controllers
{
    public class AuthController:Controller
    {
        private readonly DbConnectionFactory _dbFactory;

        public AuthController(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel vm)
        {
           using (NpgsqlConnection connection = _dbFactory.CreateConnection())
            {
                connection.Open();
                string checkQuery = "SELECT 1 FROM users WHERE email=@email";
                using (var chkCmd = new NpgsqlCommand(checkQuery, connection))
                {
                    chkCmd.Parameters.AddWithValue("@email", vm.Email);
                    using (NpgsqlDataReader reader = chkCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            TempData["msg"] = "User Already Exists!!!";
                            return RedirectToAction("Index","Home");
                        }
                    }
                }

                string insertQuery = "INSERT INTO users (full_name,email,password_hash,role,phone) values(@name,@email,@password,@role,@phone)";

                using (var insertCmd =  new NpgsqlCommand(insertQuery, connection))
                  //using (var chkCmd = new NpgsqlCommand(checkQuery, connection))
                {
                    insertCmd.Parameters.AddWithValue("@name", vm.Name);
                    insertCmd.Parameters.AddWithValue("@email", vm.Email);
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(vm.Password);
                    insertCmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(vm.Password));
                    insertCmd.Parameters.AddWithValue("@role", "user");
                    insertCmd.Parameters.AddWithValue("@phone", vm.Phone);

                    int result = insertCmd.ExecuteNonQuery();
                    if(result > 0)
                    {
                        TempData["msg"] = "You have registered sucessfullly";
                        return RedirectToAction("Index","Home");
                    }
                }
            }
            return RedirectToAction("Index","Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login( LoginViewModel vm)
        {
            using (NpgsqlConnection connection = _dbFactory.CreateConnection())
            {
                connection.Open();

                string checkQuery = "SELECT * FROM users WHERE email=@email LIMIT 1";
                
                using (var chkCmd = new NpgsqlCommand(checkQuery, connection))
                {
                    chkCmd.Parameters.AddWithValue("@email", vm.Email);
                    
                    using (NpgsqlDataReader reader = chkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string dbPassword = reader["password_hash"].ToString();
                            bool isValid = BCrypt.Net.BCrypt.Verify(vm.Password, dbPassword);

                            if (isValid)
                            {
                                string userName = reader["full_name"].ToString();
                                int userId = reader.GetInt32(reader.GetOrdinal("id"));

                                HttpContext.Session.SetString("UserName", userName);
                                HttpContext.Session.SetString("UserId", userId.ToString());

                                TempData["msg"] = "Logged in Successfully";
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                }
            }
            TempData["msg"] = "Invalid Email or Password";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}