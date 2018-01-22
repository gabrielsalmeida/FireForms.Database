using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FireForms.Database.Auth
{
    class Authentication : IAuthentication
    {
        private string GoogleIdentityUrl;
        private string GoogleSignUpUrl;
        private string GooglePasswordUrl;
        private string GooglePasswordResetUrl;
        private string GoogleSetAccountUrl;
        private string RefreshTokenUrl;
        private string apiKey;

        public Authentication(string apiKey)
        {
            this.apiKey = apiKey;
            GoogleIdentityUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyAssertion?key=" + apiKey;
            GoogleSignUpUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + apiKey;
            GooglePasswordUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + apiKey;
            GooglePasswordResetUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/getOobConfirmationCode?key=" + apiKey;
            GoogleSetAccountUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/setAccountInfo?key=" + apiKey;
            RefreshTokenUrl = "https://securetoken.googleapis.com/v1/token?key=" + apiKey;

        }

        public async Task ChangePasswordAsync(FirebaseUser firebaseUser, string newPassword)
        {
            var json = $"{{\"idToken\":\"{firebaseUser.idToken}\",\"password\":\"{newPassword}\",\"returnSecureToken\":true}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(new Uri(GoogleSetAccountUrl), content).ConfigureAwait(false);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var jsonReturn = JObject.Parse(responseData);
                var message = (string)jsonReturn["error"]["message"];
                // Login
                // Email address not found in database
                if (message.Equals("INVALID_ID_TOKEN"))
                {
                    throw new Exception("The user's credential is no longer valid. The user must sign in again.");
                }
                // Login
                // Invalid password supplied
                else if (message.Equals("WEAK_PASSWORD"))
                {
                    throw new Exception("The password must be 6 characters long or more.");
                }
                // New User
                // Email address already exists
                else if (message.Equals("EMAIL_EXISTS"))
                {
                    throw new Exception("Email address already exists");
                }
                // New User
                // Week Password
                else if (message.Contains("WEAK_PASSWORD"))
                {
                    throw new Exception("Weak password, must be at least 6 characters");
                }
                // Just end on default status check
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            var userdto = JsonConvert.DeserializeObject<FirebaseUser>(responseData);
            firebaseUser = userdto;
        }

        public async Task LinkWithEmailAndPassword(EmailUser user, FirebaseUser firebaseUser)
        {
            var json = $"{{\"idToken\":\"{ firebaseUser.idToken }\",\"email\":\"{user.Email}\",\"password\":\"{user.Password}\",\"returnSecureToken\":true}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(new Uri(GooglePasswordUrl), content).ConfigureAwait(false);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var jsonReturn = JObject.Parse(responseData);
                var message = (string)jsonReturn["error"]["message"];
                // Login
                // Email address not found in database
                if (message.Equals("EMAIL_NOT_FOUND"))
                {
                    throw new Exception("Email address not found");
                }
                // Login
                // Invalid password supplied
                else if (message.Equals("INVALID_PASSWORD"))
                {
                    throw new Exception("Incorrect passord");
                }
                // New User
                // Email address already exists
                else if (message.Equals("EMAIL_EXISTS"))
                {
                    throw new Exception("Email address already exists");
                }
                // New User
                // Week Password
                else if (message.Contains("WEAK_PASSWORD"))
                {
                    throw new Exception("Weak password, must be at least 6 characters");
                }
                // Just end on default status check
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            var userdto = JsonConvert.DeserializeObject<FirebaseUser>(responseData);
            firebaseUser = userdto;
        }

        public Task LinkWithOAuthCredential(IUser user, FirebaseUser firebaseUser)
        {

            return SignInWithPostContentAsync(user, firebaseUser);

        }

        public async Task SendPasswordReset(EmailUser user)
        {
            var json = $"{{\"requestType\":\"PASSWORD_RESET\",\"email\":\"{user.Email}\"}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(new Uri(GooglePasswordUrl), content).ConfigureAwait(false);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var jsonReturn = JObject.Parse(responseData);
                var message = (string)jsonReturn["error"]["message"];
                // Login
                // Email address not found in database
                if (message.Equals("EMAIL_NOT_FOUND"))
                {
                    throw new Exception("Email address not found");
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }

        }

        public async Task<FirebaseUser> SignInWithLoginAndPassword(EmailUser user)
        {
            var json = $"{{\"email\":\"{user.Email}\",\"password\":\"{user.Password}\",\"returnSecureToken\":true}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(user.GetUrl(apiKey), content).ConfigureAwait(false);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var jsonReturn = JObject.Parse(responseData);
                var message = (string)jsonReturn["error"]["message"];
                // Login
                // Email address not found in database
                if (message.Equals("EMAIL_NOT_FOUND"))
                {
                    throw new Exception("Email address not found");
                }
                // Login
                // Invalid password supplied
                else if (message.Equals("INVALID_PASSWORD"))
                {
                    throw new Exception("Incorrect passord");
                }
                // New User
                // Email address already exists
                else if (message.Equals("EMAIL_EXISTS"))
                {
                    throw new Exception("Email address already exists");
                }
                // New User
                // Week Password
                else if (message.Contains("WEAK_PASSWORD"))
                {
                    throw new Exception("Weak password, must be at least 6 characters");
                }
                // Just end on default status check
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            var userdto = JsonConvert.DeserializeObject<FirebaseUser>(responseData);
            return userdto;
        }

        private Uri LoginURL(FirebaseAuthType authType)
        {
            if (authType is FirebaseAuthType.LoginAndPassword)
                return new Uri(GooglePasswordUrl);
            return new Uri(GoogleIdentityUrl);
        }

        public async Task SignInWithPostContentAsync(IUser user, FirebaseUser firebaseUser)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(user.GetUrl(apiKey), user.GetPostBodySignInRequest()).ConfigureAwait(false);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var jsonReturn = JObject.Parse(responseData);
                var message = (string)jsonReturn["error"]["message"];
                // Login
                // Email address not found in database
                if (message.Equals("EMAIL_NOT_FOUND"))
                {
                    throw new Exception("Email address not found");
                }
                // Login
                // Invalid password supplied
                else if (message.Equals("INVALID_PASSWORD"))
                {
                    throw new Exception("Incorrect passord");
                }
                // New User
                // Email address already exists
                else if (message.Equals("EMAIL_EXISTS"))
                {
                    throw new Exception("Email address already exists");
                }
                // New User
                // Week Password
                else if (message.Contains("WEAK_PASSWORD"))
                {
                    throw new Exception("Weak password, must be at least 6 characters");
                }
                // Just end on default status check
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            firebaseUser = JsonConvert.DeserializeObject<FirebaseUser>(responseData);
        }

        public async Task<FirebaseUser> SignInWithPostContentAsync(IUser user)
        {
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(user.GetUrl(apiKey), user.GetPostBodySignInRequest()).ConfigureAwait(false);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var jsonReturn = JObject.Parse(responseData);
                var message = (string)jsonReturn["error"]["message"];
                // Login
                // Email address not found in database
                if (message.Equals("EMAIL_NOT_FOUND"))
                {
                    throw new Exception("Email address not found");
                }
                // Login
                // Invalid password supplied
                else if (message.Equals("INVALID_PASSWORD"))
                {
                    throw new Exception("Incorrect passord");
                }
                // New User
                // Email address already exists
                else if (message.Equals("EMAIL_EXISTS"))
                {
                    throw new Exception("Email address already exists");
                }
                // New User
                // Week Password
                else if (message.Contains("WEAK_PASSWORD"))
                {
                    throw new Exception("Weak password, must be at least 6 characters");
                }
                // Just end on default status check
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            return JsonConvert.DeserializeObject<FirebaseUser>(responseData);
        }



        public async Task<FirebaseUser> SignUpWithLoginAndPasswordAsync(EmailUser user)
        {
            var json = $"{{\"email\":\"{user.Email}\",\"password\":\"{user.Password}\",\"returnSecureToken\":true}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(new Uri(GoogleSignUpUrl), content).ConfigureAwait(false);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var jsonReturn = JObject.Parse(responseData);
                var message = (string)jsonReturn["error"]["message"];
                // Login
                // Email address not found in database
                if (message.Equals("TOO_MANY_ATTEMPTS_TRY_LATER"))
                {
                    throw new Exception("We have blocked all requests from this device due to unusual activity. Try again later.");
                }
                // Login
                // Invalid password supplied
                else if (message.Equals("OPERATION_NOT_ALLOWED"))
                {
                    throw new Exception("Password sign-in is disabled for this project.");
                }
                // New User
                // Email address already exists
                else if (message.Equals("EMAIL_EXISTS"))
                {
                    throw new Exception("Email address already exists");
                }
                // New User
                // Week Password
                else if (message.Contains("WEAK_PASSWORD"))
                {
                    throw new Exception("Weak password, must be at least 6 characters");
                }
                // Just end on default status check
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            return JsonConvert.DeserializeObject<FirebaseUser>(responseData);

        }

        public async Task SignUpWithLoginAndPasswordAsync(EmailUser user, FirebaseUser firebaseUser)
        {
            var json = $"{{\"email\":\"{user.Email}\",\"password\":\"{user.Password}\",\"returnSecureToken\":true}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(new Uri(GoogleSignUpUrl), content).ConfigureAwait(false);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var jsonReturn = JObject.Parse(responseData);
                var message = (string)jsonReturn["error"]["message"];
                // Login
                // Email address not found in database
                if (message.Equals("TOO_MANY_ATTEMPTS_TRY_LATER"))
                {
                    throw new Exception("We have blocked all requests from this device due to unusual activity. Try again later.");
                }
                // Login
                // Invalid password supplied
                else if (message.Equals("OPERATION_NOT_ALLOWED"))
                {
                    throw new Exception("Password sign-in is disabled for this project.");
                }
                // New User
                // Email address already exists
                else if (message.Equals("EMAIL_EXISTS"))
                {
                    throw new Exception("Email address already exists");
                }
                // New User
                // Week Password
                else if (message.Contains("WEAK_PASSWORD"))
                {
                    throw new Exception("Weak password, must be at least 6 characters");
                }
                // Just end on default status check
                else
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            firebaseUser = JsonConvert.DeserializeObject<FirebaseUser>(responseData);
        }

        private async Task SaveUserFirebase(FirebaseUser firebaseUser)
        {
            HttpClient client = new HttpClient();
            var response = await client.PutAsync(new Uri(String.Format("https://quantocusta-164215.firebaseio.com/users/{0}/user.json?access_token={1}", firebaseUser.localId, firebaseUser.idToken)), new StringContent(JsonConvert.SerializeObject(firebaseUser), Encoding.UTF8, "application/json")).ConfigureAwait(false);
            //var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        }

        public async Task RefreshToken(FirebaseUser firebaseUser)
        {
            HttpClient client = new HttpClient();
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            nvc.Add(new KeyValuePair<string, string>("refresh_token", firebaseUser.refreshToken));
            var response = await client.PostAsync(RefreshTokenUrl, new FormUrlEncodedContent(nvc)).ConfigureAwait(false);
            var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (responseData == "null")
            {
                throw new Exception(response.StatusCode.ToString());
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.StatusCode.ToString());
            }
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseData, settings);
            firebaseUser.expiresIn = obj["expires_in"];
            firebaseUser.refreshToken = obj["refresh_token"];
            firebaseUser.idToken = obj["id_token"];
        }

        private async Task MaintainUserAuth(FirebaseUser firebaseUser)
        {
            while (true)
            {

                DateTime then = DateTime.UtcNow;
                Double.TryParse(firebaseUser.expiresIn, out double seconds);
                then.Add(TimeSpan.FromSeconds(seconds));
                int sec = (int)(then - DateTime.UtcNow).TotalSeconds;

                while (DateTime.UtcNow < then)
                {
                    await Task.Delay(sec);
                }
                HttpClient client = new HttpClient();
                var nvc = new List<KeyValuePair<string, string>>();
                nvc.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
                nvc.Add(new KeyValuePair<string, string>("refresh_token", firebaseUser.refreshToken));
                var response = await client.PostAsync(RefreshTokenUrl, new FormUrlEncodedContent(nvc)).ConfigureAwait(false);
                var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (responseData == "null")
                {
                    throw new Exception(response.StatusCode.ToString());
                }
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.StatusCode.ToString());
                }
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                var obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseData, settings);
                firebaseUser.expiresIn = obj["expires_in"];
                firebaseUser.refreshToken = obj["refresh_token"];
                firebaseUser.idToken = obj["id_token"];
            }


        }


    }
}
