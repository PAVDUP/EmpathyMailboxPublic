using System;
using System.Threading.Tasks;
using Authentication;
using OpenAI;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace LLMConnectModule.GPT
{
    public class GPTInterface : Singleton<GPTInterface>
    {
        private OAuthProviderAuthentication _authentication;
        public string UserId => _authentication.GetUserId();
        
        public OpenAIClient API;
        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;
        
        public UnityEvent onGPTInitialized = new UnityEvent();

        public async void Initialize(OAuthProviderAuthentication oAuthProviderAuthentication)
        {
            _authentication = oAuthProviderAuthentication;
        
            await _authentication.InitializeAndSignInAsync();
            await ConnectToOpenAIProxyServer(_authentication.GetAuthToken());
            await UserThread.Instance.RetrieveThreadsInGame();
           _isInitialized = true;
           onGPTInitialized.Invoke();
        }

        private async Task ConnectToOpenAIProxyServer(string authToken)
        {
//#if UNITY_EDITOR
            API = new OpenAIClient("Your API Key"); 
            // API Key 의 경우 꼭 삭제하여야 함! 아니면, 환경 변수 받아오는 걸로 꼭 바꾸기
// #elif UNITY_STANDALONE // 현재 사설 서버가 활성화되어 있지 않으므로 이를 비활성화 하였음.
            // var auth = new OpenAIAuthentication($"sess-{authToken}");
            // var settings = new OpenAISettings(domain: "proxy-server-domain");
            // API = new OpenAIClient(auth, settings);
// #endif
        
            await CheckConnection();
        }

        private async Task CheckConnection()
        {
            try
            {
                var assistantsList = await API.AssistantsEndpoint.ListAssistantsAsync();

                foreach (var assistant in assistantsList.Items)
                {
                    Debug.Log($"{assistant} -> {assistant.CreatedAt}");
                }
            
                Debug.Log("Successfully connected to OpenAI API.");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.Log("Failed to connect to OpenAI API.");
            }
        }

        #region Event Functions

        protected override void Awake()
        {
            base.Awake();
            Initialize(new UgsAuthentication());
        }

        #endregion

        #region Authencation

        public async Task DeleteAccount()
        {
            await _authentication.DeleteAccount();
        }

        #endregion
    }
}