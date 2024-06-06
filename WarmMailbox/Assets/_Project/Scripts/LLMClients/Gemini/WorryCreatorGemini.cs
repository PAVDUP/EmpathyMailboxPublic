using System.Collections.Generic;
using System.Threading.Tasks;
using LLMConnectModule.Gemini;
using SimpleJSON;
using UnityEngine;

namespace LLMClients.Gemini
{
    public class WorryCreatorGemini : WorryCreator
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

        protected override async Task<Worry> CreateAndIndicateWorryInternal(JSONNode worryTopicPrompt, List<SpecificTopic> addedTopics = null)
        {
            structuredPromptHolder.structuredPrompt.askedPrompt = worryTopicPrompt.ToString();
            string worryTextNotParsed = await GeminiInterface.Instance.GenerateContent(structuredPromptHolder.structuredPrompt.StructuredPromptToPrompt());
            
            JSONNode worryTextParsed = JSON.Parse(worryTextNotParsed);
            string worryText = worryTextParsed["candidates"][0]["content"]["parts"][0]["text"].Value;
            
            return new Worry {worryText = worryText, features = worryTopicPrompt.ToString()};
        }
    }
}
