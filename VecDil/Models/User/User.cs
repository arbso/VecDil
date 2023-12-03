using System;
using System.ComponentModel.DataAnnotations;
namespace VecDil.Models.User
{
	public class User
	{
        [Key]
        public int user_id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}

