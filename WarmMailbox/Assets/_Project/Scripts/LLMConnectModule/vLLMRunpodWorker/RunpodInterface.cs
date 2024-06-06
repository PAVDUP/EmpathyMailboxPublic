using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Utilities.Async;
using Utils;

namespace LLMConnectModule.vLLMRunpodWorker  
{  
    #region RunPodInputWithChatCompletion
    public struct Message  
    {  
        public string content;  
        public string role;  
  
        public Message(string inputRole, string inputContent)  
        {            
            role = inputRole;  
            content = inputContent;  
        }    
    }
    
    public class RunPodInputWithChatCompletion
    {
        public List<Message> messages;
        public bool use_openai_format;
    }
    
    public class InputHolderWithChatCompletion
    {
        public RunPodInputWithChatCompletion input;
    }
    #endregion

    #region RunPodInputWithCompletion

    public class RunPodInputWithCompletion
    {
        public string prompt;
    }
    
    public class InputHolderWithCompletion
    {
        public RunPodInputWithCompletion input;
    }

    #endregion

    
    
    public class RunpodInterface : Singleton<RunpodInterface>
    {
        public UnityEvent<string> onSendChatRequestComplete = new UnityEvent<string>();

        
        #region Completion
        
        public async Task<string> SendChatRequest(string prompt)
        {
            // 1. Make Request Contents
            var request = new InputHolderWithCompletion()
            {
                input = new RunPodInputWithCompletion()
                    { 
                        prompt = prompt
                    }
            };

            var json = JsonConvert.SerializeObject(request);
            
            try
            {
                return await SendRequest(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion

        #region Chat Completion
        
        public async Task<string> SendChatRequest(List<Message> messages)
        {
            // 1. Make Request Contents
            var request = new InputHolderWithChatCompletion()
            {
                input = new RunPodInputWithChatCompletion()
                    { 
                        messages = messages,
                        use_openai_format = true 
                    }
            };

            var json = JsonConvert.SerializeObject(request);

            try
            {
                return await SendRequest(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion
        
        private async Task<string> SendRequest(string json)
        {
            var url = "https://api.runpod.ai/v2/qvekzizdixl5dz/runsync"; // API Endpoint URL
            
            
            Debug.Log(json);
            
            var postData = System.Text.Encoding.UTF8.GetBytes(json);
    
            
            // 2. Send Request
            var webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(postData),
                downloadHandler = new DownloadHandlerBuffer()
            };
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("content-Type", "application/json");
            webRequest.SetRequestHeader("authorization", "WRW6BRW6402G3YFTPQQ6LP2622GAG8G9RETIWZOO");

            AsyncOperation tempSendAsyncOperation = await webRequest.SendWebRequest();
            await tempSendAsyncOperation;

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                var response = webRequest.downloadHandler.text;
                Debug.Log("Response received: " + response);
                JSONNode jsonResponse = response;
                Debug.Log("Response received: " + jsonResponse["output"] + " / " + jsonResponse["output"]["assistant"]);
                
                onSendChatRequestComplete.Invoke(jsonResponse["output"]["assistant"]);
                return response;
            }

            return null;
        }
    }
}