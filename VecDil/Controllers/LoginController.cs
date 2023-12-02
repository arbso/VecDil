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

namespace VecDil.Controllers


{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public ActionResult<object> Authenticate([FromBody] LoginRequest login)
        {
            var loginResponse = new LoginResponse { };

            try
            {
                ValidateEmail(login.Email);

                LoginRequest loginrequest = new()
                {
                    Email = login.Email.ToLower(),
                    Password = login.Password,
                };

                bool isUsernamePasswordValid = false;

                if (login != null)
                {
                    // make await call to the Database to check username and password. here we only check if the password value is "admin"
                    isUsernamePasswordValid = loginrequest.Password == "admin";
                }

                // if credentials are valid
                if (isUsernamePasswordValid)
                {
                    string token = CreateToken(loginrequest.Email);

                    loginResponse.Token = token;
                    loginResponse.responseMsg = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK
                    };

                    // return the token
                    return Ok(new { loginResponse });
                }
                else
                {
                    // if username/password are not valid, send unauthorized status code in response               
                    return BadRequest("Email or password does not match!");
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

        private void ValidateEmail(string email)
        {
            if (!Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
            {
                throw new ArgumentException("Invalid email format!");
            }
        }

    }
}

