using System;
using System.Net.Http;

namespace FireForms.Database.Auth
{
    public abstract class User : IUser
    {
        public User()
        {
        }

        protected string GoogleIdentityUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyAssertion?key=";
        protected string GooglePasswordUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=";

        public FirebaseAuthType Provider { get; set; }

        public abstract StringContent GetPostBodySignInRequest();
        public abstract Uri GetUrl(string apiKey);
    }
}
