using System.Collections;
using Authentication;
using UnityEngine;
using UnityEngine.Events;

namespace LLMConnectModule.GPT
{
    public class GPTInterfaceLauncher : MonoBehaviour
    {
        private GPTInterface _gptInterface;
        public UnityEvent onGPTInterfaceInitialized;
        private Coroutine _checkGPTInterfaceInitializedCoroutine;
        
        void Awake()
        {
            _gptInterface = GPTInterface.Instance;
            _checkGPTInterfaceInitializedCoroutine = StartCoroutine(CheckGPTInterfaceInitialized());
        }
        
        IEnumerator CheckGPTInterfaceInitialized()
        {
            while (!_gptInterface.IsInitialized)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            onGPTInterfaceInitialized.Invoke();
        }
        
        private void OnDestroy()
        {
            if (_checkGPTInterfaceInitializedCoroutine != null)
                StopCoroutine(_checkGPTInterfaceInitializedCoroutine);
        }
    }
}
