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

        public FirebaseDatabase(Uri databaseURL, string authToken)
        {
            this.databaseURL = databaseURL;
            this.authToken = authToken;
        }

        private Uri databaseURL;

        public Uri DatabaseURL
        {
            get { return databaseURL; }
            //set { databaseURL = value; }
        }

        private string authToken;

        public string AuthToken
        {
            //get { return authToken; }
            set { authToken = value; }
        }
        

        private String target;

        public String Target
        {
            get { return target; }
            //set { path = value; }
        }


        public FirebaseDatabase Child(string path)
        {
            
            databaseURL = new Uri(databaseURL + "/" + path);
            return new FirebaseDatabase(databaseURL.AbsoluteUri);
        }

        internal FirebaseDatabase SetTarget(string path)
        {
            this.target = path;
            databaseURL = new Uri(databaseURL + "/" + this.target + ".json");
            if (!String.IsNullOrWhiteSpace(this.authToken))
            {
                databaseURL = new Uri(databaseURL, "?auth="+authToken);
                
            }
            return new FirebaseDatabase(databaseURL.AbsoluteUri);
        }

        

    }
}
