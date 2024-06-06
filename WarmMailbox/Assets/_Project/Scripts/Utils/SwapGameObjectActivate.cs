using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class SwapGameObjectActivate : MonoBehaviour
    {
        public List<GameObject> currentActiveGameObjects = new List<GameObject>();
        public List<GameObject> currentInactiveGameObjects = new List<GameObject>();
    
        public void Swap()
        {
            if (currentActiveGameObjects.Count != 0) {
                if (currentActiveGameObjects[0].activeSelf)
                {
                    foreach (var currentActiveGameObject in currentActiveGameObjects)
                    {
                        currentActiveGameObject.SetActive(false);
                    }
                    
                    foreach (var currentInactiveGameObject in currentInactiveGameObjects)
                    {
                        currentInactiveGameObject.SetActive(true);
                    }
                }
                else
                {
                    foreach (var currentActiveGameObject in currentActiveGameObjects)
                    {
                        currentActiveGameObject.SetActive(true);
                    }
                    
                    foreach (var currentInactiveGameObject in currentInactiveGameObjects)
                    {
                        currentInactiveGameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
