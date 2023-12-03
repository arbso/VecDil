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

namespace VecDil.Controllers


{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        public AuthHelper authHelper = new();
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public ActionResult<object> Authenticate([FromBody] LoginRequest login)
        {
            var loginResponse = new LoginResponse { };

            try
            {
                authHelper.ValidateEmail(login.Email);

                LoginRequest loginrequest = new()
                {
                    Email = login.Email.ToLower(),
                    Password = login.Password,
                };

                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));

                using (var dbContext = new ApplicationDbContext(optionsBuilder.Options))
                {
                    // Check if the user with the provided email exists in the database
                    var user = dbContext.Users.SingleOrDefault(u => u.Email == loginrequest.Email);

                    if (user != null && authHelper.VerifyPassword(loginrequest.Password, user.Password))
                    {
                        // Credentials are valid
                        string token = CreateToken(loginrequest.Email);

                        loginResponse.Token = token;
                        loginResponse.responseMsg = new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.OK
                        };

                        // Return the token
                        return Ok(new { loginResponse });
                    }

                    else
                    {
                        // if username/password are not valid, send unauthorized status code in response               
                        return BadRequest("Email or password does not match!");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception for invalid email format
                return BadRequest(ex.Message);
            }
        }


        private string CreateToken(string email)
        {

            List<Claim> claims = new()
            {                    
                //list of Claims - we only checking username - more claims can be added.
                new Claim("email", Convert.ToString(email)),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: cred
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}

