using System.Collections.Generic;
using LLMClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.ConsultMode
{
    public class FairyHistoryChanger : MonoBehaviour
    {
        private FairyType _fairyType;
        private bool _isInitialized = false;
        private FairyConsultChat _fairyConsultChat;
        
        public Image fairyProfileImage;
        public TextMeshProUGUI fairyProfileNameText;
        public Transform messageParent;
        public Transform userMessagePrefab;
        public Transform fairyMessagePrefab;

        private void Initialize()
        {
            foreach (var fairyConsultChat in FindObjectsOfType<FairyConsultChat>())
            {
                if (fairyConsultChat.isActivate)
                {
                    _fairyConsultChat = fairyConsultChat;
                    break;
                }
            }
        }

        public void SetFairyType(FairyType fairyType)
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
            
            _fairyType = fairyType;
            fairyProfileImage.sprite = fairyType.fairySprite;
            fairyProfileNameText.text = fairyType.fairyTypeName;
        }

        public void UpdateFairyHistory()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
            
            foreach (Transform childTransform in messageParent)
            {
                Destroy(childTransform.gameObject);
            }
            
            foreach (var chat in _fairyConsultChat.ChatHistory)
            {
                if(chat.chatTargetType == ChatTargetType.User)
                {
                    var userMessage = Instantiate(userMessagePrefab, messageParent);
                    userMessage.GetComponent<UserMessageHolder>().userMessageText.text = chat.message;
                }
                else
                {
                    var fairyMessage = Instantiate(fairyMessagePrefab, messageParent);
                    var fairyMessageHolder = fairyMessage.GetComponent<FairyMessageHolder>();
                    fairyMessageHolder.fairyMessageText.text = chat.message;
                    fairyMessageHolder.fairyNameText.text = _fairyType.fairyTypeName;
                    fairyMessageHolder.fairyImage.sprite = _fairyType.fairySprite;
                }
            }
            
            gameObject.SetActive(true);
        }

        public void UpdateFairyHistory(List<ChatMessage> inputChatMessages)
        {
            foreach (Transform childTransform in messageParent)
            {
                Destroy(childTransform.gameObject);
            }
            
            foreach (var chat in inputChatMessages)
            {
                if(chat.chatTargetType == ChatTargetType.User)
                {
                    var userMessage = Instantiate(userMessagePrefab, messageParent);
                    userMessage.GetComponent<UserMessageHolder>().userMessageText.text = chat.message;
                }
                else
                {
                    var fairyMessage = Instantiate(fairyMessagePrefab, messageParent);
                    var fairyMessageHolder = fairyMessage.GetComponent<FairyMessageHolder>();
                    fairyMessageHolder.fairyMessageText.text = chat.message;
                    fairyMessageHolder.fairyNameText.text = _fairyType.fairyTypeName;
                    fairyMessageHolder.fairyImage.sprite = _fairyType.fairySprite;
                }
            }
        }
        
        public void ClearFairyHistory()
        {
            foreach (Transform childTransform in messageParent)
            {
                Destroy(childTransform.gameObject);
            }
        }
    }
}
