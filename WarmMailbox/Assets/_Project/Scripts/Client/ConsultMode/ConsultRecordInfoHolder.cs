using System.Collections.Generic;
using LLMClients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.ConsultMode
{
    public class ConsultRecordInfoHolder : MonoBehaviour
    {
        public ConsultMetaData consultMetaData;
        public List<TextMeshProUGUI> consultRecordTitleTexts;
        public Image fairyProfileImage;
    }
}
