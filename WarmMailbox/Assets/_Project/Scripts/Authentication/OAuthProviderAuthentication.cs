using System.Threading.Tasks;
using UnityEngine;

namespace Authentication
{
    public abstract class OAuthProviderAuthentication 
    {
        protected string AuthToken;
        protected string UserId;
        
        public abstract Task InitializeAndSignInAsync ();

        public string GetAuthToken()
        {
            return AuthToken;
        }
        
        public string GetUserId()
        {
            return UserId;
        }
        
        public abstract Task SignOut();
        public abstract Task DeleteAccount();
    }
}
