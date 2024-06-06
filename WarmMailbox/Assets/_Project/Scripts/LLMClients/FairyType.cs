using UnityEngine;

namespace LLMClients
{
    [CreateAssetMenu(fileName = "FairyType", menuName = "WarmMailbox/FairyTypeHolder", order = 1)]
    public class FairyType : ScriptableObject
    {
        /// <summary>
        /// 요정 이름
        /// </summary>
        public string fairyTypeName;
        
        /// <summary>
        /// 각 LLM 에서 FairyType 을 제작할 때, 해당 FairyType 의 MetaData 를 저장하는 곳.
        /// - GPT : GPT Playground 내에 설정된 AssistantId 를 저장.
        /// - Gemini : 맨 처음 Model 의 Chat 성격을 결정하는 User Prompt 를 저장.
        /// </summary>
        public string fairyTypeMetaData;
        
        public Sprite fairySprite;
        
        public string fairyKeyword;
        
        public string fairyDescription;
        
        public string fairyStartText;
    }
}
