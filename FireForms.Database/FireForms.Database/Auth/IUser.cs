using System;
using System.Net.Http;

namespace FireForms.Database.Auth
{
    public interface IUser
    {
        FirebaseAuthType Provider { get; set; }

        StringContent GetPostBodySignInRequest();

        Uri GetUrl(string apiKey);

    }
}
