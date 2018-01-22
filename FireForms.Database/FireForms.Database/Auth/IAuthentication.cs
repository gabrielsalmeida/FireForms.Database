using System;
using System.Threading.Tasks;

namespace FireForms.Database.Auth
{
    public interface IAuthentication
    {
        Task<FirebaseUser> SignInWithPostContentAsync(IUser user);
        Task SignInWithPostContentAsync(IUser user, FirebaseUser firebaseUser);
        Task<FirebaseUser> SignUpWithLoginAndPasswordAsync(EmailUser user);
        Task SignUpWithLoginAndPasswordAsync(EmailUser user, FirebaseUser firebaseUser);
        Task<FirebaseUser> SignInWithLoginAndPassword(EmailUser user);
        Task SendPasswordReset(EmailUser user);
        Task ChangePasswordAsync(FirebaseUser firebaseUser, string newPassword);
        Task LinkWithEmailAndPassword(EmailUser user, FirebaseUser firebaseUser);
        Task LinkWithOAuthCredential(IUser user, FirebaseUser firebaseUser);
    }
}
