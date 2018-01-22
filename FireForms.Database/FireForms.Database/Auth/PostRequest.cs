using System;
namespace FireForms.Database.Auth
{
    public class PostRequest
    {
        public string idToken { get; set; }
        public string postBody { get; set; }
        public string requestUri = "http://localhost";
        public readonly bool returnIdpCredential = true;
        public readonly bool returnSecureToken = true;
        
    }
}
