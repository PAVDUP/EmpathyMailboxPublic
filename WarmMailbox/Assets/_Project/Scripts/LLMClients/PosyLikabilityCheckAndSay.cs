using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LLMClients
{
    [Serializable]
    public class PosySayCollection
    {
        public string posyName;
        public UnityEvent posyEvent;
        public UnityEvent clickEvent;
    }
    
    public class PosyLikabilityCheckAndSay : MonoBehaviour // 이거 진짜 엄청나게 하드 코딩으로 짤 것. 시간 더 쏟기 힘든.
    {
        public PosySayCollection[] posySayingCollection = new PosySayCollection[]{};
        public Button posyButton;
        public GameObject volume;
        public GameObject sendingUI;
        
        public void CheckAndActivatePosy()
        {
            foreach (var posySaying in posySayingCollection)
            {
                if (EmpathyMailboxDatabaseManager.Instance.GetPosyLikeAbility(posySaying.posyName) > 40)
                {
                    if (Random.Range(0, 100) > 80)
                    {
                        posySaying.posyEvent.Invoke();

                        void Call()
                        {
                            posySaying.clickEvent.Invoke();
                            volume.SetActive(true);
                            sendingUI.SetActive(true);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            FindObjectOfType<FairyConsultChat>()
                                .CreateAndIndicateChat("내게 반갑다고 말해줘! 또한, 오랜만에 본 것처럼 굉장히 친근한 말투와, 질문을 던져줘!");
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            FindObjectOfType<FairyConsultChat>().ClearInternalChatHistory();
                            posyButton.onClick.RemoveListener(Call);
                        }
                        
                        posyButton.onClick.AddListener(Call);
                    }
                }
            }
        }
    }
}
