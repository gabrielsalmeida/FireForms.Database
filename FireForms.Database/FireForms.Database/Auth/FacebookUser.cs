using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace FireForms.Database.Auth
{
    public class FacebookUser : OAuthUser
    {
        public FacebookUser()
        {
            Provider = FirebaseAuthType.Facebook;
        }

        public FacebookUser(string idToken)
        {
            Provider = FirebaseAuthType.Facebook;
            this.Token = idToken;
        }

        public override StringContent GetPostBodySignInRequest()
        {
            PostRequest post = new PostRequest();
            post.postBody = "access_token=" + Token + "&providerId=facebook.com";
            var json = JsonConvert.SerializeObject(post, Newtonsoft.Json.Formatting.None,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
