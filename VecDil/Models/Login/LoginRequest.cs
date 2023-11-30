using System;
namespace VecDil.Models.Login
{
    public class LoginRequest
    {
        public LoginRequest()
        {
            this.Email = String.Empty;
            this.Password = String.Empty;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
    }
}

