using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using Utilities.Async;
using Utils;

namespace LLMConnectModule.Gemini
{
    [Serializable]
    public class Prompt
    {
        /// <summary>
        /// "contents > index"(jsonObject) > "parts > index"(jsonObject) > text or inlineData
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public JSONArray contents;
        // ReSharper disable once InconsistentNaming
        public JSONObject generationConfig;
        // ReSharper disable once InconsistentNaming
        public JSONArray safetySettings;
        
        public Prompt()
        {
            this.contents = new JSONArray();
            this.generationConfig = new JSONObject();
            this.safetySettings = new JSONArray();
        }
        
        public string ToJsonString()
        {
            JSONObject prompt = new JSONObject();
            prompt["contents"] = contents;
            prompt["generationConfig"] = generationConfig;
            prompt["safetySettings"] = safetySettings;
            
            return prompt.ToString();
        }

        #region Add Content - Text, Chat, InlineData
        
        /// <summary>
        /// add text to specific content
        /// </summary>
        /// <param name="index"> content's index </param>
        /// <param name="text"> added value </param>
        public void AddTextContent(int index, string text)
        {
            if (contents.Count > index)
            {
                JSONObject textObject = new JSONObject();
                textObject["text"] = text;
                
                contents[index]["parts"].Add(textObject);
            } 
            else
            {
                AddTextContent(text);
            }
        }
        
        public void AddTextContent(string text)
        {
            JSONObject textObject = new JSONObject();
            textObject["text"] = text;
            
            JSONObject content = new JSONObject();
            content["parts"] = new JSONArray();
            content["parts"].Add(textObject);

            contents.Add(content);
        }
        
        public void AddChatContent(int index, string text, string role)
        {
            if (contents.Count > index)
            {
                JSONObject textObject = new JSONObject();
                textObject["text"] = text;
                
                contents[index]["role"] = role;
                contents[index]["parts"].Add(textObject);
            } 
            else
            {
                AddChatContent(text, role);
            }
        }

        public void AddChatContent(string text, string role)
        {
            JSONObject textObject = new JSONObject();
            textObject["text"] = text;
            
            JSONObject content = new JSONObject();
            content["role"] = role;
            content["parts"] = new JSONArray();
            content["parts"].Add(textObject);

            contents.Add(content);
        }
        
        public void AddInlineData (int index, JSONObject inlineData)
        {
            JSONObject inlineObject = new JSONObject();
            inlineObject["inlineData"] = inlineData;
            
            if (contents.Count > index)
            {
                contents[index]["parts"].Add(inlineObject);
            } 
            else
            {
                JSONObject content = new JSONObject();
                content["parts"] = new JSONArray();

                content["parts"].Add(inlineObject);

                contents.Add(content);
            }
        }

        #endregion

        #region Delete Content
        
        public void DeleteContent(int index)
        {
            contents.Remove(index);
        }

        public void DeletePartsElement(int contentIndex, int partIndex)
        {
            contents[contentIndex]["parts"].Remove(partIndex);
        }

        #endregion

        #region About Generation Config and Safe Settings
        
        public void SetGenerationConfig(float temperature, int topP, int topK, int maxOutputToken)
        {
            generationConfig["temperature"] = temperature;
            generationConfig["top_p"] = topP;
            generationConfig["top_k"] = topK;
            generationConfig["max_output_token"] = maxOutputToken;
        }

        public void AddSafeSettings(string category, string threshold)
        {
            JSONObject safeSetting = new JSONObject();
            safeSetting["category"] = category;
            safeSetting["threshold"] = threshold;
            
            safetySettings.Add(safeSetting);
        }
        
        public void DeleteSafeSettings(int index)
        {
            safetySettings.Remove(index);
        }
        
        #endregion
    }

    #region Type Of Prompt - Structured, Chat
    
    [Serializable]
    public class StructuredPrompt
    {
        [Serializable]
        public class InputOutputExample
        {
            public string input;
            public string output;
        }

        public string instructions;
        public string inputName = "input: ";
        public string outputName = "output: ";
        public List<InputOutputExample> inputOutputExamples = new List<InputOutputExample>();
        public string askedPrompt;
        
        public Prompt StructuredPromptToPrompt()
        {
            Prompt prompt = new Prompt();
            prompt.contents = new JSONArray();
            prompt.generationConfig = new JSONObject();
            prompt.safetySettings = new JSONArray();

            prompt.AddTextContent(0, instructions);
            
            foreach (var inputOutputExample in inputOutputExamples)
            {
                prompt.AddTextContent(0, inputName + inputOutputExample.input);
                prompt.AddTextContent(0, outputName + inputOutputExample.output);
            }
            
            prompt.AddTextContent(0, inputName + askedPrompt);
            prompt.AddTextContent(0, outputName);

#if UNITY_EDITOR
            // ReSharper disable once RedundantToStringCall
            Debug.Log("StructuredPromptToPrompt : " + prompt.contents.ToString());
#endif
            return prompt;
        }
    }

    [Serializable]
    public class ChatPrompt
    {
        [Serializable]
        public struct Message
        {
            public string role;
            public string text;
        }
        
        public List<Message> messages = new();

        public void AddMessage(string role, string text)
        {
            messages.Add(new Message()
            {
                role = role,
                text = text
            });
        }
        
        public Prompt ChatPromptToPrompt()
        {
            Prompt prompt = new Prompt();
            prompt.contents = new JSONArray();
            prompt.generationConfig = new JSONObject();
            prompt.safetySettings = new JSONArray();

            foreach (var message in messages)
            {
                prompt.AddChatContent(message.text, message.role);
            }

            return prompt;
        }
    }

    #endregion

    public class GeminiInterface : Singleton<GeminiInterface>
    {
        public string apiKey = "Your API Key"; // API 키를 여기에 입력
        public string modelId = "gemini-pro";
        public string version = "v1";
        private const string URL = "https://generativelanguage.googleapis.com/";
        private string _combinedUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent";
    
        private string CombineUrl(string inputUrl, string inputVersion, string inputModelId)
        {
            return inputUrl + inputVersion + "/models/" + inputModelId;
        }
    
        public async Task<string> GenerateContent(Prompt prompt)
        {
            _combinedUrl = CombineUrl(URL, version, modelId);
            
            UnityWebRequest webRequest = new UnityWebRequest(_combinedUrl + ":generateContent" + "?key=" + apiKey, "POST");
            
            // string requestBody = JsonUtility.ToJson(prompt); : prompt 는 SimpleJSON 의 JSONNode 와 JSONObject 등을 갖고 있음. 이들은 JsonUtility.ToJson 을 사용할 경우, 내부의 모든 값이 직렬화되므로 문제가 발생함.
            string requestBody = prompt.ToJsonString();
            Debug.Log(requestBody);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
            
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            UnityWebRequestAsyncOperation ao = webRequest.SendWebRequest();
            await ao;
            
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                Debug.LogError("Response: " + webRequest.downloadHandler.text);
                return null;
            }
            else
            {
                Debug.Log("Response: " + webRequest.downloadHandler.text);
                return webRequest.downloadHandler.text;
            }
        }
    }
}