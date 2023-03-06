using Eportfolio.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Eportfolio.Controllers
{
    public class AccountController : Controller
    {
        private readonly string connectionString = "Server=localhost;Database=eportfolio;Uid=root;Pwd=;";
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthenticationService _authService;

        public AccountController(ILogger<AccountController> logger, IAuthenticationService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("1");
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Open connection
                    connection.Open();
                    Console.WriteLine("2");
                    
                    MySqlCommand crossRefCmd = new MySqlCommand("SELECT COUNT(*) FROM users WHERE Email=@Email", connection);
                    crossRefCmd.Parameters.AddWithValue("@Email", model.Email);
                    MySqlDataReader reader = crossRefCmd.ExecuteReader();
                    Console.WriteLine("2a");
                    reader.Read();
                    if (reader.GetInt32("Count(*)") > 0)
                    {
                        ModelState.AddModelError("Email", "User with this Email already Exists");
                        Console.WriteLine("2b");
                        return View();
                    }
                    reader.Close();

                    // Create a new user and insert it into the database
                    // Use a data access layer or other service to create the user account
                    ApplicationUser newUser = new ApplicationUser { FirstName = model.FirstName, LastName = model.LastName, Email = model.Email, Password = model.Password };
                    MySqlCommand insertUserCmd = new MySqlCommand("INSERT INTO users (FirstName, LastName, Email, PasswordHash) VALUES (@FirstName, @LastName, @Email, @PasswordHash)", connection);
                    insertUserCmd.Parameters.AddWithValue("@FirstName", newUser.FirstName);
                    insertUserCmd.Parameters.AddWithValue("@LastName", newUser.LastName);
                    insertUserCmd.Parameters.AddWithValue("@Email", newUser.Email);
                    insertUserCmd.Parameters.AddWithValue("@PasswordHash", PasswordHash(newUser.Password));
                    insertUserCmd.ExecuteNonQuery();

                    Console.WriteLine("3");
                    // Close connection
                    connection.Close();
                }

                // Check if the user exists in the database
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Open connection
                    connection.Open();

                    Console.WriteLine("4");

                    MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE Email=@Email AND PasswordHash=@PasswordHash", connection);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@PasswordHash", PasswordHash(model.Password));
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        // If the user exists, set their authentication cookie and redirect to the home page
                        reader.Read();
                        int userId = reader.GetInt32("UserId");
                        string firstName = reader.GetString("FirstName");
                        string lastName = reader.GetString("LastName");
                        string userName = firstName + " " + lastName;
                        reader.Close();
                        HttpContext.Session.SetInt32("userId", userId);
                        HttpContext.Session.SetString("userName", userName);
                        return RedirectToAction("Index", "Home");
                    }

                    Console.WriteLine("3");
                    // Close connection
                    connection.Close();
                }

                // If the user does not exist, add a validation error and return the same view with the model
                ModelState.AddModelError("", "Invalid username or password.");
            }

            // If the model is not valid, return the same view with validation errors
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("1");
                // Check if the user exists in the database
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    // Open connection
                    connection.Open();

                    MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE Email=@Email AND PasswordHash=@PasswordHash", connection);
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@PasswordHash", PasswordHash(model.Password));
                    MySqlDataReader reader = command.ExecuteReader();
                    Console.WriteLine("2");
                    if (reader.HasRows)
                    {
                        // If the user exists, set their authentication cookie and redirect to the home page
                        reader.Read();
                        int userId = reader.GetInt32("UserId");
                        string firstName = reader.GetString("FirstName");
                        string lastName = reader.GetString("LastName");
                        string userName = firstName + " " + lastName;
                        reader.Close();
                        HttpContext.Session.SetInt32("userId", userId);
                        HttpContext.Session.SetString("userName", userName);
                        return RedirectToAction("Index", "Home");
                    }

                    Console.WriteLine("3");
                    // Close connection
                    connection.Close();
                }

                Console.WriteLine("4");
                // If the user does not exist, add a validation error and return the same view with the model
                ModelState.AddModelError("Email", "Invalid username or password.");
            }

            // If the model is not valid, return the same view with validation errors
            return View();
        }

        private string PasswordHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ProjectList()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectList(ProjectModel model)
        {
            Console.WriteLine("0");
            if (ModelState.IsValid)
            {
                Console.WriteLine("1");
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    Console.WriteLine("2");
                    MySqlCommand insertUserCmd = new MySqlCommand("INSERT INTO projects (ProjectName, ProjectDesc, UserId) VALUES (@ProjectName, @ProjectDesc, @UserId)", conn);
                    insertUserCmd.Parameters.AddWithValue("@ProjectName", model.ProjectName);
                    insertUserCmd.Parameters.AddWithValue("@UserId", HttpContext.Session.GetInt32("userId"));
                    if (model.ProjectDesc == null)
                    {
                        insertUserCmd.Parameters.AddWithValue("@ProjectDesc", "No Description");
                    }
                    else
                    {
                        insertUserCmd.Parameters.AddWithValue("@ProjectDesc", model.ProjectDesc);
                    }
                    insertUserCmd.ExecuteNonQuery();

                    Console.WriteLine("3");
                    conn.Close();
                }
                Console.WriteLine("4");
                return RedirectToAction("ProjectList", "Account");
            }
            ModelState.AddModelError("ProjectName", "Project Name must not be null");
            return View(); 
        }

        public IActionResult EditProject()
        {
            return RedirectToAction("ProjectList", "Account");
        }

        public IActionResult DeleteProject()
        {

            return RedirectToAction("ProjectList", "Account");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
