using System;
using System.Collections.Generic;
using System.Text;


namespace FireForms.Database
{
    public class FirebaseDatabase
    {
        public FirebaseDatabase(string databaseURL)
        {
            this.databaseURL = new Uri(databaseURL);
        }

        public FirebaseDatabase(string databaseURL, string authToken)
        {
            this.databaseURL = new Uri(databaseURL);
            this.AccessToken = authToken;
        }

        public FirebaseDatabase(string databaseURL, string authToken, string refreshToken, string apiKey)
        {
            this.databaseURL = new Uri(databaseURL);
            this.AccessToken = authToken;
            this.RefreshToken = refreshToken;
            this.ApiKey = apiKey;
        }



        private Uri databaseURL;

        public Uri DatabaseURL
        {
            get { return databaseURL; }
        }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string ApiKey { get; set; }

        private String target;

        public String Target
        {
            get { return target; }
            
        }

        public Uri FullUri
        {
            get;
            set;
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
            if (AccessToken != null)
            {
                string query = String.Format("?auth={0}", AccessToken);
                uriBuilder.Query = query;
            }
            target = path;
            FullUri = uriBuilder.Uri;            
            return this;
        }



    }
}
