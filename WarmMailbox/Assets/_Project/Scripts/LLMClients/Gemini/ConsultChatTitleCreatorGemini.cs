using System.Collections.Generic;
using System.Threading.Tasks;
using LLMConnectModule.Gemini;
using SimpleJSON;
using UnityEngine;

namespace LLMClients.Gemini
{
    public class ConsultChatTitleCreatorGemini : ConsultChatTitleCreator
    {
        public StructuredPromptHolder structuredPromptHolder;
        
        public override void Initialize()
        {
            base.Initialize();
            if (structuredPromptHolder == null)
            {
                Debug.LogError("structuredPromptHolder is null");
                return;
            }
        
            // ReSharper disable once UnusedVariable
            GeminiInterface tempGeminiInterfaceForActivating = GeminiInterface.Instance;
        }

        protected override async Task<string> CreateAndReturnTitleInternal(List<ChatMessage> inputConsultHistory)
        {
            JSONNode jsonNode = ConvertChatMessageListToJSONNode(inputConsultHistory);
            
            structuredPromptHolder.structuredPrompt.askedPrompt = jsonNode.ToString();
            string titleNotParsed = await GeminiInterface.Instance.GenerateContent(structuredPromptHolder.structuredPrompt.StructuredPromptToPrompt());
            
            JSONNode titleParsed = JSON.Parse(titleNotParsed);
            string title = titleParsed["candidates"][0]["content"]["parts"][0]["text"].Value;

            return title;
        }
    }
}
