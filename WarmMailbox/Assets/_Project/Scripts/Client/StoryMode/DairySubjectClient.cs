using LLMClients;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Client
{
    public class DairySubjectClient : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI todayDateText;
        public TextMeshProUGUI todayDateTextInConsult;
        public TextMeshProUGUI todaySubjectsCountText;
        public DairySubjectsInfo dairySubjectsInfo;

        public Button eventChangeTargetButton;
    
        [Header("Events")]
        public UnityEvent onTodayDateChanged;
        public UnityEvent onTodaySubjectsCountChanged;
        
        [Header("Button Events")]
        public UnityEvent onButtonCommonClicked;
        public UnityEvent onButtonClickedTodayDateChanged;
        public UnityEvent onButtonClickedTodaySubjectsCountChanged;
    
        public void ChangeTodayDateAndSubjectsCount()
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ChangeTodayDateText();
            ChangeTodaySubjectsCountText();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void ChangeTodayDateText()
        {
            todayDateText.text = EmpathyMailboxDatabaseManager.Instance.GetStoryModeDate() + " 일차";
            todayDateTextInConsult.text = EmpathyMailboxDatabaseManager.Instance.GetStoryModeDate() + "";
        }

        private void ChangeTodaySubjectsCountText()
        {
            int todaySubjectsCount = EmpathyMailboxDatabaseManager.Instance.GetConsultedCount();
            int todaySubjectsTotalCount = FindTodaySubjectsTotalCount();
            Debug.Log("todaySubjectsTotalCount : " + todaySubjectsTotalCount);
            
            todaySubjectsCountText.text = todaySubjectsCount + "/" + todaySubjectsTotalCount + "명";
        }
    
        public async void CheckAndIncreaseTodaySubjectsCountOrDate()
        {
            eventChangeTargetButton.onClick.RemoveAllListeners();
            eventChangeTargetButton.onClick.AddListener(() => onButtonCommonClicked.Invoke());
            await EmpathyMailboxDatabaseManager.Instance.AddConsultedCount();
        
            if (EmpathyMailboxDatabaseManager.Instance.GetConsultedCount() >= FindTodaySubjectsTotalCount())
            {
                eventChangeTargetButton.onClick.AddListener(() => onButtonClickedTodayDateChanged.Invoke());
                await EmpathyMailboxDatabaseManager.Instance.AddStoryModeDate();
                await EmpathyMailboxDatabaseManager.Instance.ResetConsultedCount();
                onTodayDateChanged.Invoke();
            }
            else
            {
                eventChangeTargetButton.onClick.AddListener(() =>onButtonClickedTodaySubjectsCountChanged.Invoke());
                onTodaySubjectsCountChanged.Invoke();
            }
        }

        private int FindTodaySubjectsTotalCount()
        {
            var todayDate = EmpathyMailboxDatabaseManager.Instance.GetStoryModeDate();
            var subjectsCountInfo = dairySubjectsInfo.dairySubjects;
        
            foreach (var subjects in subjectsCountInfo)
            {
                if (todayDate <= subjects.todayDate)
                {
                    return subjects.todaySubjectsCount;
                }
            }

            return -1;
        }
    }
}
