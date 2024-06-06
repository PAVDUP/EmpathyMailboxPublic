using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine;

namespace LLMClients
{
    public abstract class ConsultChatTitleCreator : MonoBehaviour
    {
        private bool _isInitialized;
        
        public virtual void Initialize()
        {
            _isInitialized = true;
        }

        public virtual async Task<string> CreateAndReturnTitle(List<ChatMessage> inputConsultHistory)
        {
            if (_isInitialized == false)
            {
                Initialize();
            }
            
            return await CreateAndReturnTitleInternal(inputConsultHistory);
        }

        protected abstract Task<string> CreateAndReturnTitleInternal(List<ChatMessage> inputConsultHistory);

        protected JSONNode ConvertChatMessageListToJSONNode(List<ChatMessage> inputConsultHistory)
        {
            JSONNode jsonNode = new JSONObject();
            foreach (var consultHistory in inputConsultHistory)
            {
                JSONNode tempChatMessage = new JSONObject();
                tempChatMessage["chatTargetType"] = consultHistory.chatTargetType.ToString();
                tempChatMessage["message"] = consultHistory.message;
                jsonNode.Add(tempChatMessage);
            }

            return jsonNode;
        }
    }
}
