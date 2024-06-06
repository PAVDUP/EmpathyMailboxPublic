using System;
using System.Threading.Tasks;
using _Project.Scripts.LLMClients;
using UnityEngine;
using UnityEngine.Events;

namespace LLMClients
{
    /// <summary>
    /// Worry 를 가져오고, adviceText 를 받아서, 해당 Worry 에 대한 Respond 를 생성하는 클래스.
    /// </summary>
    public abstract class RespondCreator : MonoBehaviour
    {
        private bool _isInitialized;
    
        public UnityEvent<string> onCompleteRespondString;
        public UnityEvent<Respond> onCompleteRespond;
        
        private bool _isCreatingRespond;


        public virtual void Initialize()
        {
            _isInitialized = true;
        }

        public async Task<Respond> RespondToWorry(string adviceText)
        {
            // 1. 이미 Respond 가 생성 중이라면, 빈 Respond 를 반환.
            if (_isCreatingRespond)
            {   
                Debug.LogError("[RespondManager] Respond is already being created.");
                return new Respond();
            }
            else
            {
                _isCreatingRespond = true;
            }
            
            if (_isInitialized == false)
            {
                Initialize();
            }

            // 2. 현재 Worry 를 가져온 후, adviceText 인수를 사용하여 해당 Worry 에 대한 Respond 를 생성.
            Respond respond;

            try
            {
                var currentWorry = EmpathyMailboxDatabaseManager.Instance.currentWorry;

                string response = await RespondToWorry(currentWorry, adviceText);
                Debug.Log($"[RespondManager] Respond! : {response}");

                onCompleteRespondString.Invoke(response);
                respond = new Respond { worry = currentWorry, adviceText = adviceText, respondText = response };
                onCompleteRespond.Invoke(respond);
                await EmpathyMailboxDatabaseManager.Instance.RecordRespond(respond);
            }
            catch (Exception e)
            {
                _isCreatingRespond = false;
                Console.WriteLine(e);
                throw;
            }
        
            _isCreatingRespond = false;
            return respond;
        }

        public abstract Task<string> RespondToWorry(Worry worry, string adviceText);
    }
}
