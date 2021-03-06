﻿using System;
using System.Net.Http;

namespace FireForms.Database.Auth
{
    public abstract class OAuthUser : User
    {
        public OAuthUser()
        {
        }

        public string Token { get; set; }

        public override Uri GetUrl(string apiKey)
        {
            return new Uri("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyAssertion?key=" + apiKey);
        }

    }
}
