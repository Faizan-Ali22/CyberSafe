using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace DA_Assets.Networking
{
    public class UnityHttpClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private HttpRequestMessage _requestMessage;
        private CancellationTokenSource _cancellationTokenSource;

        public DownloadHandler downloadHandler { get; private set; }
        public long downloadedBytes { get; private set; }
        public float downloadProgress { get; private set; }
        public string error { get; private set; }
        public bool isDone { get; private set; }
        public HttpStatusCode responseCode { get; private set; }
        public WR_Result result { get; private set; }

        private UnityHttpClient(HttpMethod method, string url, HttpContent content = null)
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5)
            };

            _requestMessage = new HttpRequestMessage(method, url);

            if (content != null)
            {
                _requestMessage.Content = content;
            }

            downloadHandler = new DownloadHandler();
            downloadedBytes = 0;
            error = string.Empty;
            isDone = false;
            responseCode = 0;
            result = WR_Result.InProgress;
            downloadProgress = 0f;
        }

        public static UnityHttpClient Get(string url)
        {
            return new UnityHttpClient(HttpMethod.Get, url);
        }

        public static UnityHttpClient Post(string url, WWWForm form)
        {
            var content = new ByteArrayContent(form.data);
            foreach (var header in form.headers)
            {
                content.Headers.Add(header.Key, header.Value);
            }
            return new UnityHttpClient(HttpMethod.Post, url, content);
        }

        public void SetRequestHeader(string name, string value)
        {
            if (_requestMessage.Headers.Contains(name))
            {
                _requestMessage.Headers.Remove(name);
            }
            _requestMessage.Headers.Add(name, value);
        }

        public async Task SendWebRequest(CancellationTokenSource cancellationTokenSource = null)
        {
            if (cancellationTokenSource == null)
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }
            else
            {
                _cancellationTokenSource = cancellationTokenSource;
            }

            try
            {
                using (HttpResponseMessage response = await _httpClient.SendAsync(
                    _requestMessage, 
                    HttpCompletionOption.ResponseHeadersRead, 
                    _cancellationTokenSource.Token).ConfigureAwait(false))
                {
                    responseCode = response.StatusCode;
                    if (response.IsSuccessStatusCode)
                    {
                        var contentLength = response.Content.Headers.ContentLength;
                        using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        using (var memoryStream = new MemoryStream())
                        {
                            if (contentLength.HasValue)
                            {
                                Progress<long> progress = new Progress<long>(totalBytes =>
                                {
                                    downloadedBytes = totalBytes;
                                    downloadProgress = (float)downloadedBytes / contentLength.Value;
                                });
                                await responseStream.CopyToAsync(memoryStream, 81920, progress, _cancellationTokenSource.Token).ConfigureAwait(false);
                                downloadProgress = 1f;
                            }
                            else
                            {
#if UNITY_2021_3_OR_NEWER
                                await responseStream.CopyToAsync(memoryStream, _cancellationTokenSource.Token).ConfigureAwait(false);
#else
                                await responseStream.CopyToAsync(memoryStream).ConfigureAwait(false);
#endif
                                downloadProgress = -1f;
                            }

                            byte[] data = memoryStream.ToArray();
                            Encoding encoding = Encoding.UTF8;
                            string charset = response.Content.Headers.ContentType?.CharSet;

                            if (!string.IsNullOrEmpty(charset))
                            {
                                try
                                {
                                    encoding = Encoding.GetEncoding(charset);
                                }
                                catch (Exception)
                                {
                                }
                            }
                            var text = encoding.GetString(data);
                            downloadHandler.SetData(data);
                            downloadHandler.SetText(text);
                            result = WR_Result.Success;
                        }
                    }
                    else
                    {
                        string errorMessage = $"Error: {response.StatusCode}";
                        downloadHandler.SetError(errorMessage);
                        error = errorMessage;
                        result = WR_Result.ProtocolError;
                    }
                }
            }
            catch (Exception ex)
            {
                downloadHandler.SetError(ex.Message);
                error = ex.Message;
                result = WR_Result.ConnectionError;
            }
            finally
            {
                isDone = true;
            }
        }

        public void Dispose()
        {
            _requestMessage?.Dispose();
            _httpClient?.Dispose();
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }

    /// <summary>
    /// https://stackoverflow.com/a/46497896
    /// </summary>
    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }
    }

    public class DownloadHandler
    {
        public byte[] data { get; private set; }
        public string text { get; private set; }
        public string error { get; private set; }

        public void SetData(byte[] data)
        {
            this.data = data;
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        public void SetError(string error)
        {
            this.error = error;
        }

        public bool HasError => !string.IsNullOrEmpty(error);
    }

    public enum WR_Result
    {
        InProgress,
        Success,
        ConnectionError,
        ProtocolError,
        DataProcessingError
    }
}
