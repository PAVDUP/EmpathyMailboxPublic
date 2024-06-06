using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client.ConsultMode
{
    public class ButtonColorChanger : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public Button targetButton;
        public TextMeshProUGUI targetText;
        private Color _normalColor;
        private Color _selectedColor = new Color32(84, 60, 39, 255); 

        void Start()
        {
            if (targetText != null)
            {
                _normalColor = targetText.color;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (targetText != null)
            {
                targetText.color = _selectedColor;
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (targetText != null)
            {
                targetText.color = _normalColor;
            }
        }
    }
}