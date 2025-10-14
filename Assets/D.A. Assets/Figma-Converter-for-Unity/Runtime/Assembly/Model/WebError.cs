#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif

using System;

namespace DA_Assets.FCU
{
    public struct WebError
    {
        public WebError(int status = 0, string message = null, Exception ex = null)
        {
            this.Status = status;
            this.Message = message;
            this.Exception = ex;
        }

        public Exception Exception { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("status")] 
#endif
        public int Status { get; set; }
#if JSONNET_EXISTS
        [JsonProperty("err")] 
#endif
        public string Message { get; set; }
    }
}