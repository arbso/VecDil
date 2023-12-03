using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Net;
using System.Security.Claims;
using VecDil.Models.Login;
using System.Text.RegularExpressions;
using VecDil.Models.User;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;


namespace VecDil.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class SignupController : ControllerBase
    {
        public AuthHelper authHelper = new AuthHelper();
        private readonly IConfiguration _configuration;

        public SignupController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("SignUp")]
        public ActionResult<object> Register([FromBody] RegisterRequest register)
        {
            try
            {
                authHelper.ValidateEmail(register.Email);

                // Check if the email is already registered
                if (IsEmailRegistered(register.Email))
                {
                    return BadRequest("Email is already registered!");
                }

                // Hash the password before saving it to the database (you should use a more secure hashing method in a real-world scenario)
                string hashedPassword = HashPassword(register.Password);

                User newUser = new User
                {
                    Username = register.Username,
                    Email = register.Email.ToLower(),
                    Password = hashedPassword,
                    Role = "User" // You might want to adjust the role based on your application's logic
                };

                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
                try
                {
                    using (var dbContext = new ApplicationDbContext(optionsBuilder.Options))
                    {
                        dbContext.Users.Add(newUser);
                        dbContext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception details for debugging
                    Console.WriteLine($"Error saving to the database: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    // Handle the exception as needed
                }

                return Ok("User registered successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool IsEmailRegistered(string email)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));

            using (var context = new ApplicationDbContext(optionsBuilder.Options))
            {
                return context.Users.Any(u => u.Email.ToLower() == email.ToLower());
            }
        }

        private string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            // Hash the password with the generated salt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hashedPassword;
        }

    }
}