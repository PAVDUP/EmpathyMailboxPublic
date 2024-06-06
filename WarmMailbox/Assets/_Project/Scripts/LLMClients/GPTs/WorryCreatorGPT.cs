using System.Collections.Generic;
using System.Threading.Tasks;
using LLMConnectModule.GPT;
using SimpleJSON;

namespace LLMClients.GPTs
{
    public class WorryCreatorGPT : WorryCreator
    {
        private MessageProcessorGPT _messageProcessorGPT;

        public override void Initialize()
        {
            base.Initialize();
            _messageProcessorGPT = new MessageProcessorGPT("asst_hUSZJK8yOIxOl4ghclHAN1Ju", UserThread.Instance.WorryCreatorThread);
        }

        protected override async Task<Worry> CreateAndIndicateWorryInternal(JSONNode worryTopicPrompt,
            List<SpecificTopic> addedTopics = null)
        {
            string worryText = await _messageProcessorGPT.ProcessMessage(worryTopicPrompt.ToString());
            return new Worry {worryText = worryText, features = worryTopicPrompt.ToString()};
        }
    }
}
