using System;
namespace VecDil.Models
{
    public class LoginResponse
    {
        public LoginResponse()
        {
            // ac ac2
            this.Token = String.Empty;
            this.responseMsg =
            new HttpResponseMessage()
            {
                StatusCode =
               System.Net.HttpStatusCode.Unauthorized
            };
        }
        //ac3
        public string Token { get; set; }
        public HttpResponseMessage responseMsg
        {
            get; set;
        }

    }

}

