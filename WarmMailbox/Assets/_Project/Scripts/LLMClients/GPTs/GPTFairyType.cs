using UnityEngine;

namespace LLMClients.GPTs
{
    [CreateAssetMenu(fileName = "GPTFairyType", menuName = "WarmMailbox/GPTFairyTypeHolder", order = 1)]
    public class GPTFairyType : ScriptableObject
    {
        public string fairyTypeName;
        public string fairyTypeAssistantId;
    }
}
