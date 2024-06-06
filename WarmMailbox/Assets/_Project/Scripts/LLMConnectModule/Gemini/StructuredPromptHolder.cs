using UnityEngine;

namespace LLMConnectModule.Gemini
{
    [CreateAssetMenu(fileName = "Structured Prompt", menuName = "LLMConnectModule/StructuredPromptHolder", order = 1)]
    public class StructuredPromptHolder : ScriptableObject
    {
        public StructuredPrompt structuredPrompt = new StructuredPrompt();
    }
}
