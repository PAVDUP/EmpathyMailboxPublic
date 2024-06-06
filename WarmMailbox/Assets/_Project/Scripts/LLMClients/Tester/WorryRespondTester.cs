using System.Collections;
using LLMClients;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.LLMClients.Tester
{
    public class WorryRespondTester : MonoBehaviour
    {
        public WorryCreator worryCreator;
        public RespondCreator respondCreator;
        
        public TextMeshProUGUI testText;
        public TMP_InputField inputField;
        public TextMeshProUGUI respondText;
        
        public UnityEvent<Worry> onCompleteWorry;
        public UnityEvent<Respond> onCompleteRespond;
        
        private UnityAction<Worry> _assignedCompleteWorry;
        private UnityAction<Respond> _assignedCompleteRespond;

        public void SetWorryAndRespondCreator(WorryCreator inputWorryCreator, RespondCreator inputRespondCreator)
        {
            if (_assignedCompleteWorry != null) worryCreator.onCompleteCreateWorry.RemoveListener(_assignedCompleteWorry);
            if (_assignedCompleteRespond != null) respondCreator.onCompleteRespond.RemoveListener(_assignedCompleteRespond);

            worryCreator = inputWorryCreator;
            respondCreator = inputRespondCreator;
            
            _assignedCompleteWorry = (worry) => onCompleteWorry.Invoke(worry);
            _assignedCompleteRespond = (respond) => onCompleteRespond.Invoke(respond);
            
            worryCreator.onCompleteCreateWorry.AddListener(_assignedCompleteWorry);
            respondCreator.onCompleteRespond.AddListener(_assignedCompleteRespond);
            respondCreator.onCompleteRespondString.AddListener((respondString) => respondText.text = respondString);
        }
        
        
        public void StartTestExternal()
        {
            StartCoroutine(StartTest());
        }
        
        private IEnumerator StartTest()
        {
            yield return new WaitForSeconds(3f);
            
            worryCreator.Initialize();
            respondCreator.Initialize();
            TestCreateAndIndicate();
        }
        
        private async void TestCreateAndIndicate()
        {
            Worry worry = await worryCreator.CreateAndIndicateWorry();
            testText.text = worry.worryText;
            EmpathyMailboxDatabaseManager.Instance.currentWorry = worry;
        }
        
        public async void TestCreateAndIndicateWorry()
        {
            Worry worry = await worryCreator.CreateAndIndicateWorry();
            testText.text = worry.worryText;
            EmpathyMailboxDatabaseManager.Instance.currentWorry = worry;
        }
        
        public async void TestRespondToWorry()
        {
            await respondCreator.RespondToWorry(inputField.text);
        }

        #region 단독으로 쓰기

        [Space(10)][Header("For Standalone Use")]
        public bool isStandalone;
        
        private void Awake()
        {
            if (isStandalone)
                SetWorryAndRespondCreator(worryCreator, respondCreator);
        }

        #endregion
    }
}
