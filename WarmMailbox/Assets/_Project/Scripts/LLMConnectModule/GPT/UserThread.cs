using System;
using System.Threading.Tasks;
using OpenAI.Threads;
using UnityEngine;
using Utils;

namespace LLMConnectModule.GPT
{
    public class UserThread : Singleton<UserThread>
    {
        public ThreadResponse WorryCreatorThread;
        public ThreadResponse RespondThread;
        public ThreadResponse ConsultChatTitleCreatorThread;

        public async Task RetrieveThreadsInGame()
        {
            WorryCreatorThread = await RetrieveThread("WorryCreator333" + GPTInterface.Instance.UserId);
            RespondThread = await RetrieveThread("Respond333" + GPTInterface.Instance.UserId);
            ConsultChatTitleCreatorThread = await RetrieveThread("ConsultChatTitleCreator333" + GPTInterface.Instance.UserId);
        }

        // ReSharper disable once RedundantAssignment
        public async Task<ThreadResponse> RetrieveThread(string threadName = null)
        {
            ThreadResponse thread = null;
            
            try
            {
                thread = await GPTInterface.Instance.API.ThreadsEndpoint.RetrieveThreadAsync(PlayerPrefs.GetString(threadName, "0"));
                UpdateThread(thread);
                return thread;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                thread = await CreateThread(threadName);
                UpdateThread(thread);
                return thread;
            }
        }

        private async void UpdateThread(ThreadResponse thread)
        {
            thread = await thread.UpdateAsync();
            Debug.Log($"[RetrieveUserThread] Retrieve thread {thread.Id} -> {thread.CreatedAt}");
        }
        
        private async Task<ThreadResponse> CreateThread(string threadName = null)
        {
            ThreadResponse tempThreadResponse = await GPTInterface.Instance.API.ThreadsEndpoint.CreateThreadAsync(threadName);
            PlayerPrefs.SetString(threadName, tempThreadResponse.Id);
            return tempThreadResponse;
        }
    }
}
