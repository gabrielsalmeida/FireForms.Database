using System;
using System.Net.Http;
using System.Text;

namespace FireForms.Database.Auth
{
    public class EmailUser : User
    {
        public EmailUser()
        {
            Provider = FirebaseAuthType.LoginAndPassword;
        }

        public EmailUser(string email, string password)
        {
            Provider = FirebaseAuthType.LoginAndPassword;
            this.Email = email;
            this.Password = password;
        }

        public string Email { get; set; }
        public string Password { get; set; }

        public override StringContent GetPostBodySignInRequest()
        {
            var json = $"{{\"email\":\"{Email}\",\"password\":\"{Password}\",\"returnSecureToken\":true}}";
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public Uri GetVerifyPassword(string apiKey)
        {
            return new Uri("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + apiKey);
        }

        public override Uri GetUrl(string apiKey)
        {
            return new Uri("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyAssertion?key=" + apiKey);
        }
    }
}
