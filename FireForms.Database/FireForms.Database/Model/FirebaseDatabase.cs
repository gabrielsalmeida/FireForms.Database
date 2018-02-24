using System;
using System.Collections.Generic;
using System.Text;
using FireForms.Database.Auth;

namespace FireForms.Database.Model
{
    public class FirebaseDatabase
    {
        public FirebaseDatabase(string databaseURL)
        {
            this.databaseURL = new Uri(databaseURL);
        }

        public FirebaseDatabase(string databaseURL, String authToken)
        {
            this.databaseURL = new Uri(databaseURL);
            this.AccessToken = authToken;
        }

        public FirebaseDatabase(string databaseURL, FirebaseUser firebaseUser)
        {
            this.databaseURL = new Uri(databaseURL);
            this.AccessToken = firebaseUser.idToken;
            this.FirebaseUser = firebaseUser;
        }

        public FirebaseDatabase(string databaseURL, string authToken, string refreshToken, string apiKey)
        {
            this.databaseURL = new Uri(databaseURL);
            this.AccessToken = authToken;
            this.RefreshToken = refreshToken;
            this.ApiKey = apiKey;
        }


        public FirebaseUser FirebaseUser { get; set; }

        private Uri databaseURL;

        public Uri DatabaseURL
        {
            get { return databaseURL; }
        }

        public String AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string ApiKey { get; set; }

        private String target;

        public String Target
        {
            get { return target; }

        }

        private Uri fullUri;

        public Uri FullUri
        {
            get
            {
                UriBuilder uriBuilder = new UriBuilder(fullUri);
                FullUri = SetToken(uriBuilder);
                return fullUri;
            }
            set
            {
                fullUri = value;
            }
        }

        public FirebaseDatabase Child(string path)
        {
            UriBuilder uriBuilder = new UriBuilder(databaseURL);
            uriBuilder.Path += path + "/";
            databaseURL = uriBuilder.Uri;
            return this;
        }

        public FirebaseDatabase SetTarget(string path)
        {
            UriBuilder uriBuilder = new UriBuilder(databaseURL);
            uriBuilder.Path += path + ".json";
            target = path;
            FullUri = uriBuilder.Uri;
            return this;
        }

        private Uri SetToken(UriBuilder uriBuilder)
        {
            if (FirebaseUser != null)
            {
                uriBuilder.Query = String.Format("auth={0}", FirebaseUser.idToken);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(AccessToken))
                {
                    uriBuilder.Query = String.Format("auth={0}", AccessToken);
                }
            }
            return uriBuilder.Uri;
        }

    }
}
