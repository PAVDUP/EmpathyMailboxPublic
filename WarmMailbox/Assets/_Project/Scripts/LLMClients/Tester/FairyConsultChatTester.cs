using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LLMClients.Tester
{
    public class FairyConsultChatTester : MonoBehaviour
    {
        public FairyConsultChat fairyConsultChat;
        
        public TMP_InputField inputField;
        public TextMeshProUGUI respondText;
        
        
        private UnityAction<string> _currentAction;
        
        [Header("UI")]
        public List<Image> fairyImages;
        public List<TextMeshProUGUI> fairyTypeTexts;
        public List<TextMeshProUGUI> fairyKeywordTexts;
        public List<TextMeshProUGUI> fairyDescriptionTexts;
        public List<TextMeshProUGUI> fairyStartTexts;
        
        public void SetFairyConsultChat(FairyConsultChat inputFairyConsultChat)
        {
            if (_currentAction != null) fairyConsultChat.onChatMessageReceived.RemoveListener(_currentAction);
            
            fairyConsultChat = inputFairyConsultChat;
            
            fairyConsultChat.Initialize();
            _currentAction = OnChatMessageReceived;
            fairyConsultChat.onChatMessageReceived.AddListener(_currentAction);
        }
        
        private void OnChatMessageReceived(string message)
        {
            respondText.text = message;
        }
        
        public async void TestCreateAndIndicateChat()
        {
            await fairyConsultChat.CreateAndIndicateChat(inputField.text);
        }
        
        public void SetFairyType(FairyType inputFairyType)
        {
            fairyConsultChat.SetFairyType(inputFairyType);

            foreach (var fairyTypeText in fairyTypeTexts)
            {
                fairyTypeText.text = inputFairyType.fairyTypeName;
            }

            foreach (var fairyKeywordText in fairyKeywordTexts)
            {
                fairyKeywordText.text = inputFairyType.fairyKeyword;
            }

            foreach (var fairyDescriptionText in fairyDescriptionTexts)
            {
                fairyDescriptionText.text = inputFairyType.fairyDescription;
            }

            foreach (var fairyImage in fairyImages)
            {
                fairyImage.sprite = inputFairyType.fairySprite;
            }

            foreach (var fairyStartText in fairyStartTexts)
            {
                fairyStartText.text = inputFairyType.fairyStartText;
            }
        }
        
        public void ClearChatHistory()
        {
            fairyConsultChat.ClearChatHistory();
        }
    }
}
