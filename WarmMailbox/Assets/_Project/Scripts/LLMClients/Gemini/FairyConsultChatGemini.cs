using System.Threading.Tasks;
using LLMConnectModule.Gemini;
using SimpleJSON;

namespace LLMClients.Gemini
{
    public class FairyConsultChatGemini : FairyConsultChat
    {
        private ChatPrompt _chatPrompt = new ChatPrompt();
        public override void SetFairyType(FairyType inputFairyType)
        {
            base.SetFairyType(inputFairyType);
            InitChatPrompt();
        }
        
        public override void Initialize()
        {
            base.Initialize();
            if (fairyType == null)
            {
                return;
            }
        
            InitChatPrompt();
            // ReSharper disable once UnusedVariable
            GeminiInterface tempGeminiInterfaceForActivating = GeminiInterface.Instance;
        }

        private void InitChatPrompt()
        {
            _chatPrompt = new ChatPrompt();
            _chatPrompt.messages = new System.Collections.Generic.List<ChatPrompt.Message>();
            
            _chatPrompt.messages.Add(new ChatPrompt.Message()
            {
                role = "user",
                text = fairyType.fairyTypeMetaData
            });
            
            _chatPrompt.messages.Add(new ChatPrompt.Message()
            {
                role = "model",
                text = "해당 요정으로 동작."
            });
        }
        
        protected override async Task<string> CreateAndIndicateChatInternal(string message)
        {
            _chatPrompt.messages.Add(new ChatPrompt.Message()
            {
                role = "user",
                text = message
            });
            
            string getMessages = await GeminiInterface.Instance.GenerateContent(_chatPrompt.ChatPromptToPrompt());
            JSONNode getMessagesParsed = JSON.Parse(getMessages);
            string getMessage = getMessagesParsed["candidates"][0]["content"]["parts"][0]["text"].Value;
            _chatPrompt.messages.Add(new ChatPrompt.Message()
            {
                role = "model",
                text = getMessage
            });

            return getMessage;
        }
    }
}
