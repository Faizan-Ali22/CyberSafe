using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif

#pragma warning disable CS0162

namespace DA_Assets.FCU
{
    public class DAJson
    {
#if JSONNET_EXISTS
        private static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            Error = (sender, error) => error.ErrorContext.Handled = true,
            Formatting = Formatting.Indented
        };
#endif
        public static string ToJson(object obj)
        {
#if JSONNET_EXISTS
            return JsonConvert.SerializeObject(obj, settings);
#else
            return "";
#endif
        }

        public static T FromJson<T>(string json)
        {
#if JSONNET_EXISTS
            return JsonConvert.DeserializeObject<T>(json, settings);
#else
            return default(T);
#endif
        }

        public static async Task<DAResult<T>> FromJsonAsync<T>(string json)
        {
            DAResult<T> @return = new DAResult<T>();

            try
            {
#if JSONNET_EXISTS == false
                throw new MissingComponentException("Newtonsoft.Json packaghe is not installed.");
#endif
                JFResult jfr = default;

                await Task.Run(() =>
                {
                    jfr = DAFormatter.Format<T>(json);
                });

                if (jfr.IsValid == false)
                {
                    throw new Exception("Not valid json.");
                }

                if (jfr.MatchTargetType == false)
                {
                    throw new InvalidCastException("The input json does not match the target type.");
                }

                await Task.Run(() =>
                {
#if JSONNET_EXISTS
                    @return.Object = JsonConvert.DeserializeObject<T>(json, settings);
#endif
                });

                @return.Success = true;
            }
            catch (InvalidCastException ex)
            {
                @return.Success = false;
                @return.Error = new WebError(29, ex.Message, ex);
            }
            catch (MissingComponentException ex)
            {
                @return.Success = false;
                @return.Error = new WebError(455, ex.Message, ex);
            }
            catch (ThreadAbortException ex)
            {
                @return.Success = false;
                @return.Error = new WebError(-1, ex.Message, ex);
            }
            catch (Exception ex)
            {
                @return.Success = false;
                @return.Error = new WebError(422, ex.Message, ex);
            }

            return @return;
        }
    }
}