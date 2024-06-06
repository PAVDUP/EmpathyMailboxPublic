using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.StoryMode;
using LLMClients;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Client.ConsultMode
{
    public class ConsultModeRecordUIManager : MonoBehaviour
    {
        [Header("ListOfConsult")] 
        public FairyCollectionManager fairyCollectionManager;
        public Transform consultRecordWindow;
        public Transform recordButtonPrefab;
        public Transform recordButtonParent;
        public UnityEvent consultButtonClickEvent;

        [Header("ConsultBody")] 
        public List<Transform> consultBodyTransformList;
        public FairyHistoryChanger fairyHistoryChanger;

        public void UpdateRecordButtonLIst()
        {
            foreach (Transform child in recordButtonParent)
            {
                Destroy(child.gameObject);
            }
        
            List<ConsultMetaData> overallConsultMetaData = EmpathyMailboxDatabaseManager.Instance.GetConsultMetaData();
        
            foreach (var consultMetaData in overallConsultMetaData)
            {
                Transform newRecordButton = Instantiate(recordButtonPrefab, recordButtonParent);
                ConsultRecordInfoHolder recordButtonInfoHolder = newRecordButton.GetComponent<ConsultRecordInfoHolder>();
            
                recordButtonInfoHolder.consultMetaData = consultMetaData;

                foreach (var consultRecordTitleText in recordButtonInfoHolder.consultRecordTitleTexts)
                {
                    consultRecordTitleText.text = consultMetaData.consultTitle;
                }
                
                recordButtonInfoHolder.fairyProfileImage.sprite = fairyCollectionManager.ReturnFairy(consultMetaData.fairyName).fairySprite;

                async void Call() => await OnClickRecordButton(consultMetaData);
                newRecordButton.GetComponent<Button>().onClick.AddListener(Call);
                newRecordButton.GetComponent<Button>().onClick.AddListener(() => consultButtonClickEvent.Invoke());
            }
            
            consultRecordWindow.gameObject.SetActive(true);
        }
    
        private async Task OnClickRecordButton(ConsultMetaData consultMetaData)
        {
            fairyHistoryChanger.SetFairyType(fairyCollectionManager.ReturnFairy(consultMetaData.fairyName));
        
            List<ChatMessage> tempChatHistory = await EmpathyMailboxDatabaseManager.Instance.GetConsultHistory(consultMetaData.dateTimeId);
            fairyHistoryChanger.UpdateFairyHistory(tempChatHistory);
        
            foreach (var consultBodyTransform in consultBodyTransformList)
            {
                consultBodyTransform.gameObject.SetActive(true);
            }
        }

        private void OnEnable()
        {
            foreach (var consultBodyTransform in consultBodyTransformList)
            {
                consultBodyTransform.gameObject.SetActive(false);
            }
            
            fairyHistoryChanger.ClearFairyHistory();
        }

        private void OnDisable()
        {
            foreach (var consultBodyTransform in consultBodyTransformList)
            {
                consultBodyTransform.gameObject.SetActive(false);
            }
        }
    }
}
