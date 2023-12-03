using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace VecDil.Controllers;


public class AuthHelper { 
    public void ValidateEmail(string email)
    {
        if (!Regex.IsMatch(email, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
        {
            throw new ArgumentException("Invalid email format!");
        }
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        // Check if the provided password matches the hashed password
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}