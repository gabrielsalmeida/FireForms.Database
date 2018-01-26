using System;
using System.Net.Http;

namespace FireForms.Database.Auth
{
    public abstract class User : IUser
    {
        public User()
        {
        }

        public FirebaseAuthType Provider { get; set; }

        public abstract StringContent GetPostBodySignInRequest();
        public abstract Uri GetUrl(string apiKey);
    }
}
