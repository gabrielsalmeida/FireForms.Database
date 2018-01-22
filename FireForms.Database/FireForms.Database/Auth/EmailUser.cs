﻿using System;
using System.Net.Http;
using System.Text;

namespace FireForms.Database.Auth
{
    public class EmailUser : User
    {
        public EmailUser()
        {
        }
        public string Email { get; set; }
        public string Password { get; set; }

        public override StringContent GetPostBodySignInRequest()
        {
            var json = $"{{\"email\":\"{Email}\",\"password\":\"{Password}\",\"returnSecureToken\":true}}";
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public override Uri GetUrl(string apiKey)
        {
            return new Uri(GooglePasswordUrl + apiKey);
        }
    }
}
