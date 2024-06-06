using System.Threading.Tasks;
using LLMConnectModule.Gemini;
using SimpleJSON;
using UnityEngine;

namespace LLMClients.Gemini
{
    public class RespondCreatorGemini : RespondCreator
    {
        public StructuredPromptHolder structuredPromptHolder;
    
        public override void Initialize()
        {
            base.Initialize();
        
            // ReSharper disable once UnusedVariable
            GeminiInterface tempGeminiInterfaceForActivating = GeminiInterface.Instance;
        }

        public override async Task<string> RespondToWorry(Worry worry, string adviceText)
        {
            JSONNode message = new JSONObject();
            
            message["고민 특성"] = worry.features;
            message["대화"] = new JSONObject();
            message["대화"]["0"] = worry.worryText;
            message["대화"]["1"] = adviceText;
            
            Debug.Log($"[RespondCreator] Sending message in RespondToWorry : {message}");
            structuredPromptHolder.structuredPrompt.askedPrompt = message.ToString();
        
            string responseTextNotParsed = await GeminiInterface.Instance.GenerateContent(structuredPromptHolder.structuredPrompt.StructuredPromptToPrompt());
            
            JSONNode responseTextParsed = JSON.Parse(responseTextNotParsed);
            string responseText = responseTextParsed["candidates"][0]["content"]["parts"][0]["text"].Value;
            
            Debug.Log($"[RespondManager] Respond! : {responseText}");
            JSONObject responseJson = JSON.Parse(responseText) as JSONObject;
            
            string actualResponse = "";
            if (responseJson != null) actualResponse = responseJson[1];
            return actualResponse;
        }
    }
}
