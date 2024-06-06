using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Start_Page.UI
{
    public class SceneTransitionHelper : MonoBehaviour
    {
        public UnityEvent onSceneLoadingFailed;
        public UnityEvent onSceneTransitionEffect;
        public float time = 1f;
        public float effectTime = 0.5f;
        
        public void GoToNextScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        public void GoToPreviousScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        
        public void GoToSceneByName(string sceneName)
        {
            try
            {
                SceneManager.LoadScene(sceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                
                onSceneLoadingFailed.Invoke();
            }
        }
        
        public void GoToSceneByNameAfterDelaying(string sceneName)
        {
            StartCoroutine(GoToSceneByNameAfterDelayingCoroutine(sceneName));
        }
        
        IEnumerator GoToSceneByNameAfterDelayingCoroutine(string sceneName)
        {
            yield return new WaitForSeconds(effectTime);
            onSceneTransitionEffect.Invoke();
            
            yield return new WaitForSeconds(time-effectTime);
            
            try
            {
                SceneManager.LoadScene(sceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                
                onSceneLoadingFailed.Invoke();
            }
        }
        
        public void GameEndAfterDelaying()
        {
            StartCoroutine(GameEndAfterDelayingCoroutine());
        }
        
        private IEnumerator GameEndAfterDelayingCoroutine () {
            yield return new WaitForSeconds(effectTime);
            onSceneTransitionEffect.Invoke();
            
            yield return new WaitForSeconds(time-effectTime);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif STANDALONE
            Application.Quit();
#endif
        }
    }
}
