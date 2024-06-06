using System.Collections;
using LLMClients;
using LLMConnectModule.GPT;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Client
{
    public class CheckResetGame : MonoBehaviour
    {
        public TextMeshProUGUI randomNumText;
        private int _randomNum;
        
        public TMP_InputField inputField;
        
        public UnityEvent onResetGameStart;
        
        public void GenerateRandomNum()
        {
            _randomNum = Random.Range(0, 100);
            randomNumText.text = _randomNum.ToString();
        }
        
        public void ResetGame()
        {
            CheckRandomNumAndReset();
        }
        
        private async void CheckRandomNumAndReset()
        {
            if (inputField.text == _randomNum.ToString())
            {
                Debug.Log("Correct!");
                onResetGameStart.Invoke();
                inputField.text = "";

                await GPTInterface.Instance.DeleteAccount();
                EmpathyMailboxDatabaseManager.Instance.Reset();
                UserThread.Instance.Reset();
                GPTInterface.Instance.Reset();

                StartCoroutine(ResetScene());
            }
            else
            {
                Debug.Log("Incorrect!");
                inputField.text = "<color=red>숫자가 다릅니다</color>";
            }
        }

        private IEnumerator ResetScene()
        {
            yield return new WaitForSeconds(0.6f);
            SceneManager.LoadScene("0.MainTitle"); // Scene 을 다시 로드하여 완전 리셋.
        }
    }
}
