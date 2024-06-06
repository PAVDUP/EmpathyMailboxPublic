using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LLMClients;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Client.StoryMode
{
    public class StoryModeRecordUIManager : MonoBehaviour
    {
        [Header("ListOfDairy")]
        public Transform dairyListWindow;
        public DairySubjectsInfo dairySubjectsInfo;
        public Transform recordButtonPrefab;
        public Transform recordButtonParent;
        public Scrollbar recordButtonScrollbar;
        public UnityEvent dairyButtonClickEvent;

        [Header("ListOfSubjectsInDairy")] 
        public Transform subjectListWindow;
        public SubjectMaker subjectMaker;
        public Transform subjectButtonPrefab;
        public Transform subjectButtonParent;
        private List<GameObject> _haveToDestroy = new List<GameObject>();
        public UnityEvent subjectButtonClickEvent;
        
        [Header("WorryAndRespondUI")]
        public Transform worryAndRespondWindow;
        public SubjectPartCollection subjectPartCollection;
        public TextMeshProUGUI worryText;
        public TextMeshProUGUI adviceText;
        public TextMeshProUGUI respondText;
        
        private void Update()
        {
            recordButtonScrollbar.size = 0;
        }

        public void UpdateRecordButtonList()
        {
            foreach (Transform child in recordButtonParent)
            {
                Destroy(child.gameObject);
            }
            
            List<RespondMetaData> overallRespondMetaData = EmpathyMailboxDatabaseManager.Instance.GetRespondMetaData();
            Dictionary<int, Transform> recordButtonDict = new Dictionary<int, Transform>();
            
            foreach (var respondMetaData in overallRespondMetaData)
            {
                if (recordButtonDict.TryGetValue(respondMetaData.storyModeDate, out Transform recordButton))
                {
                    recordButton.GetComponent<RecordButtonInfoHolder>().todayRespondMetaData.Add(respondMetaData);
                }
                else
                {
                    Transform newRecordButton = Instantiate(recordButtonPrefab, recordButtonParent);
                    RecordButtonInfoHolder recordButtonInfoHolder = newRecordButton.GetComponent<RecordButtonInfoHolder>();
                    recordButtonInfoHolder.dateText.text = respondMetaData.storyModeDate-1 + "일 차";
                    recordButtonInfoHolder.todayRespondMetaData.Add(respondMetaData);
                    
                    // Add listener to the record button
                    newRecordButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        dairyListWindow.gameObject.SetActive(false);
                        subjectListWindow.gameObject.SetActive(true);
                    });
                    newRecordButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        UpdateSubjectButtonList(recordButtonInfoHolder);
                    });
                    newRecordButton.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        dairyButtonClickEvent.Invoke();
                    });
                    
                    recordButtonDict.Add(respondMetaData.storyModeDate, newRecordButton);
                }
            }
        }
        
        public void UpdateSubjectButtonList(RecordButtonInfoHolder recordButtonInfoHolder)
        {
            foreach (GameObject destroy in _haveToDestroy)
            {
                Destroy(destroy);
            }

            foreach (var respondMetaData in recordButtonInfoHolder.todayRespondMetaData)
            {
                Transform subjectButton = Instantiate(subjectButtonPrefab, subjectButtonParent);
                _haveToDestroy.Add(subjectButton.gameObject);
                
                subjectButton.SetSiblingIndex((int)Math.Round(subjectButtonParent.childCount / 2f));
                SubjectButtonInfoHolder subjectButtonInfoHolder = subjectButton.GetComponent<SubjectButtonInfoHolder>();
                subjectButtonInfoHolder.respondMetaData = respondMetaData;
                subjectMaker.SetImageWithRespondMetaDataAndPartCollection(respondMetaData, subjectButtonInfoHolder.subjectPartCollection);
                
                // Add listener to the subject button
                async void Call() // Lambda function 말고, 이렇게 함수 내부에서 함수 만드는 게 가능하네?? => 이건 첨 알았네.
                {
                    await UpdateWorryAndRespondUI(subjectButtonInfoHolder);
                    worryAndRespondWindow.gameObject.SetActive(true);
                }
                subjectButton.GetComponent<Button>().onClick.AddListener(Call);
                subjectButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    subjectButtonClickEvent.Invoke();
                });
            }
        }
        
        public async Task UpdateWorryAndRespondUI(SubjectButtonInfoHolder subjectButtonInfoHolder)
        {
            subjectMaker.SetImageWithRespondMetaDataAndPartCollection(subjectButtonInfoHolder.respondMetaData, subjectPartCollection);
            
            Respond tempRespond = await EmpathyMailboxDatabaseManager.Instance.GetRespond(subjectButtonInfoHolder.respondMetaData.dateTimeId);
            
            worryText.text = tempRespond.worry.worryText;
            adviceText.text = tempRespond.adviceText;
            respondText.text = tempRespond.respondText;
        }
    }
}
