using System;
using System.Web;
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

        public FirebaseDatabase(Uri databaseURL, string authToken)
        {
            this.databaseURL = databaseURL;
            this.AccessToken = authToken;
        }

        private Uri databaseURL;

        public Uri DatabaseURL
        {
            get { return databaseURL; }
        }

        public string AccessToken { get; set; }


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
                string query = String.Format("access_token={0}", AccessToken);
                uriBuilder.Query = query;
            }
            target = path;
            FullUri = uriBuilder.Uri;            
            return this;
        }



    }
}
