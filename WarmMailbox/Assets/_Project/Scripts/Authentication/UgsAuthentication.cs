using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Authentication
{
    public class UgsAuthentication : OAuthProviderAuthentication 
    {
        public override async Task InitializeAndSignInAsync ()
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized && UnityServices.State != ServicesInitializationState.Initializing)
                {
                    
                        await UnityServices.InitializeAsync();
                        
                        SetupEvents();
                        Debug.Log("Unity Services Initialized!");
                    
                }
                
                if (AuthenticationService.Instance.IsSignedIn)
                {
                    AuthToken = AuthenticationService.Instance.AccessToken;
                    UserId = AuthenticationService.Instance.PlayerId;
                }
                else
                {
                    await SignInAnonymouslyAsync();
                    AuthToken = AuthenticationService.Instance.AccessToken;
                    UserId = AuthenticationService.Instance.PlayerId;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                    Application.Quit();
#endif
            }
        }
    
        async Task SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Sign in anonymously succeeded!");
        
                // Shows how to get the playerID
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); 

            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }
    
        void SetupEvents() {
            AuthenticationService.Instance.SignedIn += () => {
                // Shows how to get a playerID
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

                // Shows how to get an access token
                Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

            };

            AuthenticationService.Instance.SignInFailed += (err) => {
                Debug.LogError(err);
            };

            AuthenticationService.Instance.SignedOut += () => {
                Debug.Log("Player signed out.");
            };

            AuthenticationService.Instance.Expired += () =>
            {
                Debug.Log("Player session could not be refreshed and expired.");
            };
        }

        public override Task SignOut()
        {
            AuthenticationService.Instance.SignOut();
            return Task.CompletedTask;
        }
        
        public override async Task DeleteAccount()
        {
            await AuthenticationService.Instance.DeleteAccountAsync();
        }
    }
}