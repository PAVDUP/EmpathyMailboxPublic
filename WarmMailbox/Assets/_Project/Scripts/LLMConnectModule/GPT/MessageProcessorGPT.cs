using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Threads;
using UnityEngine;

namespace LLMConnectModule.GPT
{
    /// <summary>
    /// Create and Get message based on GPT Assistant
    /// </summary>
    public class MessageProcessorGPT
    {
        private string _assistantId;
        private ThreadResponse _thread;

        public MessageProcessorGPT(string assistantId, ThreadResponse thread)
        {
            _assistantId = assistantId;
            _thread = thread;
        }
        
        public async Task<string> ProcessMessage(string message)
        {
            var messageResponses = ProcessMessageInternal(message);
            var messageResponse = GetLatestMessages(await messageResponses);
            
            return messageResponse.PrintContent();
        }

        public async Task<List<string>> ProcessMessages(string message)
        {
            var messageResponses = await ProcessMessageInternal(message);
            List<string> messages = new List<string>();
            
            foreach (var messageResponse in messageResponses.Items)
            {
                messages.Add(messageResponse.PrintContent());
            }
            
            return messages;
        }
        
        private async Task<ListResponse<MessageResponse>> ProcessMessageInternal(string message)
        {
            await CreateMessage(_thread, message);
            RunResponse run = await RunThread(_thread, _assistantId);
            
            while (true)
            {
                if (run.Status == RunStatus.Cancelled || run.Status == RunStatus.Failed || run.Status == RunStatus.Cancelling)
                {
                    Debug.LogError($"[MessageProcessor] ProcessMessage Failed : [{run.Id}] {run.Status} | {run.CreatedAt}");
                    return null;
                }
                else if (run.Status == RunStatus.Completed)
                {
                    break;
                }
                
                run = await run.UpdateAsync();
                await Task.Delay(100);
            }
            
            Debug.Log($"[MessageProcessor] ProcessMessage Completed : [{run.Id}] {run.Status} | {run.CreatedAt}");
            
            var messages = await _thread.ListMessagesAsync();
            
            return messages;
        }
        
        public async Task CreateMessage(ThreadResponse thread, string message)
        {
            var request = new CreateMessageRequest(message);
            var messageResponse = await thread.CreateMessageAsync(request);
            Debug.Log($"[MessageProcessor] CreateMessage : {messageResponse.Id}: {messageResponse.Role}: {messageResponse.PrintContent()}");
        }
        
        public async Task<RunResponse> RunThread(ThreadResponse thread, string assistant)
        {
            var run = await thread.CreateRunAsync(assistant);
            Debug.Log($"[{run.Id}] {run.Status} | {run.CreatedAt}");
            return run;
        }
        
        public MessageResponse GetLatestMessages(ListResponse<MessageResponse> messages)
        {
            return messages.Items[0];
        }
    }
}
