using System.Collections.Generic;
using System.Threading.Tasks;
using LLMConnectModule.GPT;
using UnityEngine;

namespace LLMClients.GPTs
{
    public class FairyConsultChatGPT : FairyConsultChat
    {
        private readonly Dictionary<string, MessageProcessorGPT> _messageProcessorGPTs = new Dictionary<string, MessageProcessorGPT>();
        public override void SetFairyType(FairyType inputFairyType)
        {
            fairyType = inputFairyType;
            
            if (_messageProcessorGPTs.TryGetValue(fairyType.fairyTypeName, out var tempMessageProcessorGPT))
            {
                _messageProcessorGPT = tempMessageProcessorGPT;
            }
            else
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                FindThreadBasedOnFairyType();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }
        
        private MessageProcessorGPT _messageProcessorGPT;
        
        public override void Initialize()
        {
            base.Initialize();

            if (fairyType == null)
                return;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            FindThreadBasedOnFairyType();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        
        private async Task FindThreadBasedOnFairyType()
        {
            Debug.Log("[FairyConsultChatGPT] FindThreadBasedOnFairyType : " + fairyType.fairyTypeName);
            
            var tempThreadResponse = await UserThread.Instance.RetrieveThread(fairyType.fairyTypeName + GPTInterface.Instance.UserId);
            _messageProcessorGPT = new MessageProcessorGPT(fairyType.fairyTypeMetaData, tempThreadResponse);
            _messageProcessorGPTs.Add(fairyType.fairyTypeName, _messageProcessorGPT);
        }

        protected override async Task<string> CreateAndIndicateChatInternal(string message)
        {
            string getMessage = await _messageProcessorGPT.ProcessMessage(message);
            return getMessage;
        }
    }
}
