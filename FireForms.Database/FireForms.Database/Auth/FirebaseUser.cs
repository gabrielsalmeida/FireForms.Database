using System;
namespace FireForms.Database.Auth
{
    public class FirebaseUser
    {
        public FirebaseUser()
        {

        }
        public int id { get; set; }
        public string kind { get; set; }
        public string federatedId { get; set; }
        public string providerId { get; set; }
        public string localId { get; set; }
        public bool emailVerified { get; set; }
        public string email { get; set; }
        public string oauthAccessToken { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string displayName { get; set; }
        public string idToken { get; set; }
        public string photoUrl { get; set; }
        public string refreshToken { get; set; }
        public string expiresIn { get; set; }
        public string rawUserInfo { get; set; }
    }
}
