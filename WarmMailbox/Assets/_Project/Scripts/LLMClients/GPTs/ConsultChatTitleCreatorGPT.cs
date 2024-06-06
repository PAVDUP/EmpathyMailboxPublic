using System.Collections.Generic;
using System.Threading.Tasks;
using LLMConnectModule.GPT;
using SimpleJSON;

namespace LLMClients.GPTs
{
    public class ConsultChatTitleCreatorGPT : ConsultChatTitleCreator
    {
        private MessageProcessorGPT _messageProcessorGPT;

        public override void Initialize()
        {
            base.Initialize();
            _messageProcessorGPT = new MessageProcessorGPT("asst_p9KNCa8yd9eo2CpvoUpvTIhu", UserThread.Instance.ConsultChatTitleCreatorThread);
        }

        protected override async Task<string> CreateAndReturnTitleInternal(List<ChatMessage> inputConsultHistory)
        {
            JSONNode jsonNode = ConvertChatMessageListToJSONNode(inputConsultHistory);
        
            string title = await _messageProcessorGPT.ProcessMessage(jsonNode.ToString());
            return title;
        }
    }
}
