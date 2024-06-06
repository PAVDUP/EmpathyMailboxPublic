using System.Collections.Generic;
using LLMClients;
using TMPro;
using UnityEngine;

namespace Client.StoryMode
{
    public class RecordButtonInfoHolder : MonoBehaviour
    {
        public List<RespondMetaData> todayRespondMetaData = new List<RespondMetaData>();
        public TextMeshProUGUI dateText;
    }
}
