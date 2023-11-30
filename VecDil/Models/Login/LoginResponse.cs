﻿using System;
namespace VecDil.Models.Login
{
    public class LoginResponse
    {
        public LoginResponse()
        {
            this.Token = String.Empty;//ac
            this.responseMsg =
            new HttpResponseMessage()
            {
                StatusCode =
               System.Net.HttpStatusCode.Unauthorized
            };
        }

        public string Token { get; set; }
        public HttpResponseMessage responseMsg
        {
            get; set;
        }

    }

}

