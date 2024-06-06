using UnityEngine;

namespace Client
{
    public class RandomObjectActivator : MonoBehaviour
    {
        public GameObject[] objects; // Inspector에서 할당할 오브젝트 배열
        private GameObject _activeObject; // 현재 활성화된 오브젝트를 추적

        void OnEnable()
        {
            ActivateRandomObject();
        }

        void OnDisable()
        {
            if (_activeObject != null)
            {
                _activeObject.SetActive(false);
            }
        }

        // 랜덤 오브젝트를 활성화하는 메서드
        void ActivateRandomObject()
        {
            if (objects.Length == 0)
            {
                Debug.LogError("No objects assigned in the array.");
                return;
            }

            // 모든 오브젝트를 비활성화
            foreach (GameObject obj in objects)
            {
                obj.SetActive(false);
            }

            // 랜덤 인덱스 선택
            int randomIndex = Random.Range(0, objects.Length);
            _activeObject = objects[randomIndex];
            _activeObject.SetActive(true);
        }
    }
}