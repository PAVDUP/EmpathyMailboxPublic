using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts.LLMClients;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace LLMClients
{
    public enum ChatTargetType
    {
        User,
        Fairy
    }
    
    [Serializable]
    public class ChatMessage
    {
        public ChatTargetType chatTargetType;
        public string message;
    }
    
    public abstract class FairyConsultChat : MonoBehaviour
    {
        public bool isActivate = false;
        
        public FairyType fairyType;
        public virtual void SetFairyType(FairyType inputFairyType)
        {
            fairyType = inputFairyType;
        }
        
        private bool _isInitialized;
        private bool _isCreatingChatResponse;

        private readonly List<ChatMessage> _chatHistory = new List<ChatMessage>();
        public List<ChatMessage> ChatHistory => _chatHistory;
        
        public UnityEvent<List<ChatMessage>> onChatHistoryChanged = new UnityEvent<List<ChatMessage>>();
        public UnityEvent<string> onChatMessageReceived = new UnityEvent<string>();

        public virtual void Initialize()
        {
            _isInitialized = true;
        }
        
        /// <summary>
        /// 나가거나 하는 Event 시점마다 ClearChatHistory 를 호출하여야 함!
        /// </summary>
        public virtual async void ClearChatHistory()
        {
            await EmpathyMailboxDatabaseManager.Instance.RecordConsultHistory(_chatHistory);
            _chatHistory.Clear();
            
            onChatHistoryChanged.Invoke(_chatHistory);
        }
        
        public virtual void ClearInternalChatHistory()
        {
            _chatHistory.Clear();
            onChatHistoryChanged.Invoke(_chatHistory);
        }
    
        public async Task CreateAndIndicateChat(string message)
        {
            // 1. Worry 가 이미 생성 중인 경우, 에러를 출력하고, 빈 Worry 를 반환.
            if (_isCreatingChatResponse)
            {
                Debug.LogError("[FairyConsultChat] CreateAndIndicateChat : Response is already being created.");
                return;
            }
            else
            {
                _isCreatingChatResponse = true;
            }
            
            if (_isInitialized == false)
            {
                Initialize();
            }

            _chatHistory.Add(new ChatMessage(){ chatTargetType = ChatTargetType.User, message = message});
            var getMessage = await CreateAndIndicateChatInternal(message);
            Debug.Log("[FairyConsultChatGPT] tempChatHistory : " + getMessage);
            
            onChatMessageReceived.Invoke(getMessage);
            _chatHistory.Add(new ChatMessage(){ chatTargetType = ChatTargetType.Fairy, message = getMessage});
            onChatHistoryChanged.Invoke(_chatHistory);
            _isCreatingChatResponse = false;
        }

        protected abstract Task<string> CreateAndIndicateChatInternal(string message);
    }
}
