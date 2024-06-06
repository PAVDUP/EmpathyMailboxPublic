using System.Threading.Tasks;
using LLMConnectModule.GPT;
using SimpleJSON;
using UnityEngine;

namespace LLMClients.GPTs
{
    public class RespondCreatorGPT : RespondCreator
    {
        private MessageProcessorGPT _messageProcessorGPT;
        
        // Start is called before the first frame update
        public override void Initialize()
        {
            base.Initialize();
            _messageProcessorGPT = new MessageProcessorGPT("asst_8VnADReTcMKF1igEhDd002yR", UserThread.Instance.RespondThread);
        }

        public override async Task<string> RespondToWorry(Worry worry, string adviceText)
        {
            JSONNode message = new JSONObject();

            message["고민 특성"] = worry.features;
            message["대화"] = new JSONObject();
            message["대화"]["0"] = worry.worryText;
            message["대화"]["1"] = adviceText;
            
            Debug.Log($"[RespondCreator] Sending message in RespondToWorry : {message}");
            string notParsedResponseText = await _messageProcessorGPT.ProcessMessage(message.ToString());
            
            Debug.Log($"[RespondManager] Respond! : {notParsedResponseText}");
            JSONObject responseJson = JSON.Parse(notParsedResponseText) as JSONObject;
            
            string actualResponse = "";
            if (responseJson != null) actualResponse = responseJson[1];
            return actualResponse;
        }
    }
}
