using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LLMConnectModule.vLLMRunpodWorker;
using SimpleJSON;
using UnityEngine;

namespace LLMClients.VLLMRunpodWorker
{
    public class WorryCreatorRunpod : WorryCreator
    {
        private JSONNode _tempWorryTopicPrompt;

        protected override async Task<Worry> CreateAndIndicateWorryInternal(JSONNode worryTopicPrompt,
            List<SpecificTopic> addedTopics = null)
        {
            #region Chat Completion
            
            List<Message> tempMessages = new List<Message>();
            tempMessages.Add(new Message(inputRole: "system", inputContent: "JSON 형식의 고민 주제를 받으면, 해당 특성을 기반으로 고민을 생성합니다. 생성된 고민은 상담자가 대화하는 것과 같은 어투를 띄어야 합니다."));
            tempMessages.Add(new Message(inputRole: "user", inputContent: "{   \"고민성격\": \"익살\" }"));
            tempMessages.Add(new Message(inputRole: "assistant", inputContent: "요즘 제일 큰 고민이 있어요. 바로 요즘 들어서 제가 워낙 잘생겨진 것 같아서 말이죠. 어떻게 더 겸손해져야 할까요? 참 고민이네요 이거이거~"));
            tempMessages.Add(new Message(inputRole: "user", inputContent: worryTopicPrompt.ToString()));
            
            #endregion

            #region Completion

            // ReSharper disable once RedundantToStringCall
            /*string tempMessages = "JSON 형식의 고민 주제를 받으면, 해당 특성을 기반으로 고민을 생성합니다. 생성된 고민은 상담자가 대화하는 것과 같은 어투를 띄어야 합니다. 또한 고민은 3줄 이상이어야 합니다. 다음 JSON 형식의 고민을 생성해주세요 : " + worryTopicPrompt.ToString();
            */

            #endregion

            string aiMessage;
            
            try
            {
                aiMessage = await RunpodInterface.Instance.SendChatRequest(tempMessages);
                Debug.Log("[WorryCreatorRunpod] CreateAndIndicateWorryInternal : aiMessage : " + aiMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            Worry tempWorry = new Worry();
            tempWorry.worryText = aiMessage;
            tempWorry.features = _tempWorryTopicPrompt.ToString();

            return tempWorry;
        }
    }
}
