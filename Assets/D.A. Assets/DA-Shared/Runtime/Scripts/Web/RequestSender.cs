using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DA_Assets.Networking
{
    public class RequestSender
    {
        public static async Task<Texture2D> LoadImage(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
            {
                var asyncOp = webRequest.SendWebRequest();

                while (!asyncOp.isDone)
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
                    Debug.LogError($"LoadImage | {webRequest.error}");
                    return null;
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                    return texture;
                }
            }
        }

        public static async Task<string> Get(string url, int awaitDelayMs = 100)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                try
                {
#if UNITY_6000_0_OR_NEWER
                    await webRequest.SendWebRequest();
#else
                    webRequest.SendWebRequest();

                    while (!webRequest.isDone)
                        await Task.Delay(awaitDelayMs);
#endif
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    return null;
                }

                bool isRequestError;
#if UNITY_2020_1_OR_NEWER
                isRequestError = webRequest.result != UnityWebRequest.Result.Success;
#else
                isRequestError = webRequest.isNetworkError || webRequest.isHttpError;
#endif

                if (isRequestError)
                {
                    Debug.LogError($"RequestSender.Get | {webRequest.error}");
                    return null;
                }
                else
                {
                    string result = webRequest.downloadHandler.text;
                    return result;
                }
            }
        }

        public static async Task<string> Post(string url, string postData, string contentType, Dictionary<string, string> headers = null, int awaitDelayMs = 100)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(postData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", contentType);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        webRequest.SetRequestHeader(header.Key, header.Value);
                    }
                }

                try
                {
#if UNITY_6000_0_OR_NEWER
                    await webRequest.SendWebRequest();
#else
                    webRequest.SendWebRequest();

                    while (!webRequest.isDone)
                        await Task.Delay(awaitDelayMs);
#endif
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    return null;
                }

                while (!webRequest.isDone)
                    await Task.Delay(awaitDelayMs);

                bool isRequestError;
#if UNITY_2020_1_OR_NEWER
                isRequestError = webRequest.result != UnityWebRequest.Result.Success;
#else
                isRequestError = webRequest.isNetworkError || webRequest.isHttpError;
#endif

                if (isRequestError)
                {
                    string methodPath = $"{nameof(RequestSender)}.{nameof(Post)}";
                    Debug.LogError($"{methodPath} | Error: {webRequest.error}");
                    return null;
                }
                else
                {
                    string result = webRequest.downloadHandler.text;
                    return result;
                }
            }
        }
    }
}