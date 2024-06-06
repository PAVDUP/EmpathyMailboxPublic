using System;
using System.Collections.Generic;
using _Project.Scripts.LLMClients;
using _Project.Scripts.LLMClients.Tester;
using LLMClients.Tester;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace LLMClients
{
    [Serializable]
    public enum LLMType
    {
        GPT,
        Gemini
    }

    [Serializable]
    public class LLMApplication
    {
        [FormerlySerializedAs("LLMType")] public LLMType llmType = LLMType.GPT;

        public WorryCreator worryCreator;
        public RespondCreator respondCreator;
        public FairyConsultChat fairyConsultChat;
        public ConsultChatTitleCreator consultChatTitleCreator;
    }
    
    public class EmpathyMailboxLLMManager : MonoBehaviour
    {
        public LLMType currentLLMType;
        public TextMeshProUGUI currentLLMTypeText;
        public string currentFairyName;
        
        [Space(15)]
        public WorryRespondTester worryRespondTester;
        public FairyConsultChatTester fairyConsultChatTester;
        public FairyCollectionManager fairyCollectionManager;
        public List<LLMApplication> llmApplications;

        [Header("Events")] 
        public UnityEvent onFairyTypeChanged;
        public UnityEvent<string> onFairyTypeChangedWithFairyName;
        public UnityEvent<FairyType> onFairyTypeChangedWithFairyType;

        public void Awake()
        {
            SetCurrentLLMType(currentLLMType.GetHashCode());
        }
        
        [VisibleEnum(typeof(LLMType))]
        public void SetCurrentLLMType(int inputLLMType)
        {
            currentLLMType = (LLMType)inputLLMType;
            if (currentLLMTypeText != null)
                currentLLMTypeText.text = currentLLMType.ToString();
            fairyCollectionManager.currentLLMType = currentLLMType;
            
            foreach (var llmApplication in llmApplications)
            {
                if (llmApplication.llmType != currentLLMType) continue;
                
                if (llmApplication.worryCreator != null)
                    worryRespondTester.SetWorryAndRespondCreator(llmApplication.worryCreator, llmApplication.respondCreator);
                if (llmApplication.fairyConsultChat != null)
                    fairyConsultChatTester.SetFairyConsultChat(llmApplication.fairyConsultChat);
                if (llmApplication.consultChatTitleCreator != null)
                    EmpathyMailboxDatabaseManager.Instance.consultChatTitleCreator = llmApplication.consultChatTitleCreator;
                
                break;
            }
        }
        
        public void SetFairyCharacter (string fairyName)
        {
            Debug.Log("[EmpathyMailboxLLMManager] SetFairyCharacter : " + fairyName);
            currentFairyName = fairyName;
            var fairyType = fairyCollectionManager.ReturnFairy(fairyName);
            fairyConsultChatTester.SetFairyType(fairyType);
            
            onFairyTypeChanged.Invoke();
            onFairyTypeChangedWithFairyName.Invoke(fairyName);
            onFairyTypeChangedWithFairyType.Invoke(fairyType);
        }
    }
}
