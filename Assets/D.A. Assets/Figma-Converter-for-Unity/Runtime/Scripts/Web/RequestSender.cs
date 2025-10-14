using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.Logging;
using DA_Assets.Networking;
using System;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable IDE0052

namespace DA_Assets.FCU
{
    [Serializable]
    public class RequestSender : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] float pbarProgress;
        public float PbarProgress => pbarProgress;

        [SerializeField] float pbarBytes;
        public float PbarBytes => pbarBytes;

        [SerializeField] int _requestCount;
        [SerializeField] bool _timeoutActive;
        [SerializeField] int _remainingTime;

        private static int requestCount = 0;
        private static bool timeoutActive = false;
        private static int remainingTime = 0;

        public RequestHeader GetRequestHeader(string token, AuthType? authType = null)
        {
            AuthType currentAuthType = authType != null ? (AuthType)authType : monoBeh.Authorizer.CurrentSession.AuthType;

            switch (currentAuthType)
            {
                case AuthType.OAuth2:
                    return new RequestHeader
                    {
                        Name = "Authorization",
                        Value = $"Bearer {token}"
                    };
                case AuthType.Manual:
                    return new RequestHeader
                    {
                        Name = "X-Figma-Token",
                        Value = $"{token}"
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        public void RefreshLimiterData()
        {
            _requestCount = requestCount;
            _timeoutActive = timeoutActive;
            _remainingTime = remainingTime;
        }

        private async Task CheckRateLimit()
        {
            while (requestCount >= FcuConfig.ApiRequestsCountLimit)
            {
                if (timeoutActive == false)
                {
                    timeoutActive = true;
                    remainingTime = FcuConfig.ApiTimeoutSec;
                    _ = LogRemainingTime();
                }

                await Task.Delay(1000);
            }

            requestCount++;
        }

        private async Task LogRemainingTime()
        {
            while (remainingTime > 0)
            {
                DALogger.Log(FcuLocKey.log_api_waiting.Localize(remainingTime));
                RefreshLimiterData();
                await Task.Delay(1000);
                remainingTime--;
            }

            requestCount = 0;
            timeoutActive = false;
        }

        public async Task<DAResult<T>> SendRequest<T>(DARequest request)
        {
            await CheckRateLimit();

            UnityHttpClient webRequest;

            switch (request.RequestType)
            {
                case RequestType.Post:
                    webRequest = UnityHttpClient.Post(request.Query, request.WWWForm);
                    break;
                default:
                    webRequest = UnityHttpClient.Get(request.Query);
                    break;
            }

            if (monoBeh.IsDebug())
            {
                Debug.Log(request.Query);
            }

            using (webRequest)
            {
                if (request.RequestHeader.IsDefault() == false)
                {
                    webRequest.SetRequestHeader(request.RequestHeader.Name, request.RequestHeader.Value);
                }

                try
                {
                    _ = webRequest.SendWebRequest();
                }
                catch (InvalidOperationException)
                {
                    DALogger.LogError(FcuLocKey.log_enable_http_project_settings.Localize());
                    monoBeh.AssetTools.StopImport(StopImportReason.Error);
                    return default;
                }
                catch (Exception ex)
                {
                    DALogger.LogException(ex);
                }

                await UpdateRequestProgressBar(webRequest);
                await MoveRequestProgressBarToEnd();

                DAResult<T> result = new DAResult<T>();

                if (request.RequestType == RequestType.GetFile)
                {
                    result.Success = true;
                    result.Object = (T)(object)webRequest.downloadHandler.data;
                }
                else
                {
                    _ = request.WriteLog(webRequest);

                    string text = webRequest.downloadHandler.text;

                    if (typeof(T) == typeof(string))
                    {
                        result.Success = true;
                        result.Object = (T)(object)text;
                    }
                    else
                    {
                        result = await TryParseResponse<T>(text, request, webRequest);
                    }
                }

                return result;
            }
        }

        private async Task<DAResult<T>> TryParseResponse<T>(string text, DARequest request, UnityHttpClient webRequest)
        {
            DAResult<T> result = new DAResult<T>();
            int state;

            DAResult<WebError> figmaApiError = await DAJson.FromJsonAsync<WebError>(text);
            bool isRequestError = webRequest.result != WR_Result.Success;

            if (figmaApiError.Object.Message != null)
            {
                state = 1;
                result.Success = false;
                result.Error = figmaApiError.Object;
            }
            else if (isRequestError)
            {
                result.Success = false;

                if (webRequest.error.Contains("SSL"))
                {
                    state = 2;
                    result.Error = new WebError(909, text);
                }
                else
                {
                    state = 3;
                    result.Error = new WebError((int)webRequest.responseCode, webRequest.error);
                }
            }
            else if (text.Contains("<pre>Cannot GET "))
            {
                state = 4;
                result.Error = new WebError(404, text);
            }
            else
            {
                DAResult<T> obj = await DAJson.FromJsonAsync<T>(text);

                if (obj.Success)
                {
                    state = 5;
                    result.Success = true;
                    result.Object = obj.Object;

                    if (request.Name == RequestName.Project)
                    {
                        monoBeh.ProjectCacher.Cache(obj.Object);
                    }
                }
                else
                {
                    state = 6;
                    result.Success = false;
                    result.Error = obj.Error;
                }
            }

            DALogger.Log($"TryParseResponse | {state}");
            return result;
        }

        private async Task UpdateRequestProgressBar(UnityHttpClient webRequest)
        {
            while (webRequest.isDone == false)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;

                if (webRequest.downloadProgress == 0 || webRequest.downloadedBytes == 0)
                {
                    if (pbarProgress < 1f)
                    {
                        pbarProgress += 0.01f;
                    }
                    else
                    {
                        pbarProgress = 0;
                    }

                    pbarBytes += 100;
                }
                else
                {
                    pbarProgress = webRequest.downloadProgress;
                    pbarBytes = webRequest.downloadedBytes;
                }

                await Task.Yield();
            }
        }

        private async Task MoveRequestProgressBarToEnd()
        {
            float left = 1f - pbarProgress;

            int steps = 10;
            float stepIncrement = left / steps;

            for (int i = 0; i < steps; i++)
            {
                pbarProgress += stepIncrement;
                await Task.Yield();
            }

            pbarProgress = 0f;
            pbarBytes = 0f;
        }
    }
}