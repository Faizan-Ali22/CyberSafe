using DA_Assets.Networking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DA_Assets.OpenAI
{
    public class GptMini
    {
        private string _apiKey = null;
        private int _timeout = 0;
        private string _model = null;

        public const string _apiUrl = "https://api.openai.com/v1/chat/completions";

        public GptMini(string apiKey, string model = "gpt-4o-mini", int timeout = 0)
        {
            _apiKey = apiKey;
            _model = model;
            _timeout = timeout;
        }

        public async Task<string> SendRequest(UnityWebRequest webRequest)
        {
            using (webRequest)
            {
                UnityWebRequestAsyncOperation operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                bool isRequestError;
#if UNITY_2020_1_OR_NEWER
                isRequestError = webRequest.result != UnityWebRequest.Result.Success;
#else
                isRequestError = webRequest.isNetworkError || webRequest.isHttpError;
#endif

                if (isRequestError)
                {
                    Debug.LogError($"Error: {webRequest.error}");
                    return null;
                }
                else
                {
                    return webRequest.downloadHandler.text;
                }
            }
        }

        public async Task<string> InvokeChat(string prompt)
        {
            string methodPath = $"{nameof(GptMini)}.{nameof(InvokeChat)}";

            string chatRequest = CreateChatRequestBody(prompt);

            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + _apiKey }
            };

            string jsonResponse = await RequestSender.Post(_apiUrl, chatRequest, "application/json", headers);

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                Debug.LogError($"{methodPath} | Received an empty response.");
                return null;
            }

            Response data = JsonUtility.FromJson<Response>(jsonResponse);

            if (data.choices != null && data.choices.Length > 0)
            {
                return data.choices[0].message.content;
            }
            else
            {
                Debug.LogError($"{methodPath} | Invalid response structure.");
                return null;
            }
        }

        /*public async Task<string> InvokeChat(string prompt)
        {
            string chatRequest = CreateChatRequestBody(prompt);

            UnityWebRequest post = UnityWebRequest.Post(_apiUrl, chatRequest, "application/json");
            post.SetRequestHeader("Authorization", "Bearer " + _apiKey);
            post.timeout = _timeout;

            string json = await SendRequest(post);
            Response data = JsonUtility.FromJson<Response>(json);
            return data.choices[0].message.content;
        }*/

        private string CreateChatRequestBody(string prompt)
        {
            RequestMessage msg = new RequestMessage();
            msg.role = "user";
            msg.content = prompt;

            Request req = new Request();
            req.model = _model;
            req.messages = new[] { msg };

            return JsonUtility.ToJson(req);
        }
    }

    [Serializable]
    public struct ResponseMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    public struct ResponseChoice
    {
        public int index;
        public ResponseMessage message;
    }

    [Serializable]
    public struct Response
    {
        public string id;
        public ResponseChoice[] choices;
    }

    [Serializable]
    public struct RequestMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    public struct Request
    {
        public string model;
        public RequestMessage[] messages;
    }
}