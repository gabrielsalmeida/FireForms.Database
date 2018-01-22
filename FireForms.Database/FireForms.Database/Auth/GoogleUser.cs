using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace FireForms.Database.Auth
{
    public class GoogleUser : OAuthUser
    {
        public GoogleUser()
        {
        }

        public override StringContent GetPostBodySignInRequest()
        {
            PostRequest post = new PostRequest();
            post.postBody = "id_token=" + Token + "&providerId=google.com";
            var json = JsonConvert.SerializeObject(post, Newtonsoft.Json.Formatting.None,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
