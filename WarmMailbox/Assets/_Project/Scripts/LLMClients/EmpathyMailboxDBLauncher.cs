using System.Collections;
using LLMClients;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.LLMClients
{
    public class EmpathyMailboxDBLauncher : MonoBehaviour
    {
        private EmpathyMailboxDatabaseManager _empathyMailboxDatabaseManager;
        public UnityEvent onEmpathyMailboxDBInitialized;
        Coroutine _checkEmpathyMailboxDBInitializedCoroutine;
        
        // Start is called before the first frame update
        void Start()
        {
            _empathyMailboxDatabaseManager = EmpathyMailboxDatabaseManager.Instance;
            _checkEmpathyMailboxDBInitializedCoroutine = StartCoroutine(CheckEmpathyMailboxDBInitialized());
        }
        
        IEnumerator CheckEmpathyMailboxDBInitialized()
        {
            while (!_empathyMailboxDatabaseManager.IsInitialized)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            onEmpathyMailboxDBInitialized.Invoke();
        }
        
        private void OnDestroy()
        {
            if (_checkEmpathyMailboxDBInitializedCoroutine != null)
                StopCoroutine(_checkEmpathyMailboxDBInitializedCoroutine);
        }
    }
}
