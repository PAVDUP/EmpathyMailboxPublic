using System;
using UnityEngine;
using UnityEngine.Events;

namespace Utils
{
    public class CollisionUnityEvent : MonoBehaviour
    {
        public UnityEvent onCollisionEnter;
        public UnityEvent onCollisionExit;
        public UnityEvent onTriggerEnter;
        public UnityEvent onTriggerExit;

        #region 2D
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            onCollisionEnter.Invoke();
            Debug.Log("OnCollisionEnter2D");
        }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            onCollisionExit.Invoke();
            Debug.Log("OnCollisionExit2D");
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            onTriggerEnter.Invoke();
            Debug.Log("OnTriggerEnter2D");
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            onTriggerExit.Invoke();
            Debug.Log("OnTriggerExit2D");
        }

        #endregion

        #region 3D

        private void OnCollisionEnter(Collision other)
        {
            onCollisionEnter.Invoke();
        }
        
        private void OnCollisionExit(Collision other)
        {
            onCollisionExit.Invoke();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter.Invoke();
            Debug.Log("OnTriggerEnter");
        }
        
        private void OnTriggerExit(Collider other)
        {
            onTriggerExit.Invoke();
            Debug.Log("OnTriggerExit");
        }

        #endregion
    }
}
