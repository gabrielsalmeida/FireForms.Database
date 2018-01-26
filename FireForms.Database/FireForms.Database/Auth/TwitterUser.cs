using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace FireForms.Database.Auth
{
    public class TwitterUser : OAuthUser
    {
        public TwitterUser()
        {
            Provider = FirebaseAuthType.Twitter;
        }

        public TwitterUser(string idToken, string tokenSecret)
        {
            Provider = FirebaseAuthType.Twitter;
            this.Token = idToken;
            this.TokenSecret = tokenSecret;
        }

        public string TokenSecret { get; set; }

        public override StringContent GetPostBodySignInRequest()
        {
            PostRequest post = new PostRequest();
            post.postBody = "access_token=" + Token + "&oauth_token_secret=" + TokenSecret + "&providerId=twitter.com";
            var json = JsonConvert.SerializeObject(post, Newtonsoft.Json.Formatting.None,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return new StringContent(json, Encoding.UTF8, "application/json");

        }


    }
}
