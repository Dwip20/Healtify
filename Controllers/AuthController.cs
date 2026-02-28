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
                    insertCmd.Parameters.AddWithValue("@password", vm.Password);
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
                string checkQuery = "SELECT 1 FROM users WHERE email=@email and password_hash=@password";
                
                using (var chkCmd = new NpgsqlCommand(checkQuery, connection))
                {
                    chkCmd.Parameters.AddWithValue("@email", vm.Email);
                    chkCmd.Parameters.AddWithValue("@password", vm.Password);
                    using (NpgsqlDataReader reader = chkCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            TempData["msg"] = "Logged in Successfully";
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            TempData["msg"] = "Invalid Email or Password";
            return RedirectToAction("Index", "Home");
        }
    }
}