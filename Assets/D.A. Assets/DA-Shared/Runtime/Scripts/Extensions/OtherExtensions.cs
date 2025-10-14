using System;
using System.Linq;
using UnityEngine;

namespace DA_Assets.Extensions
{
    public static class OtherExtensions
    {
        public static bool IsDefault<T>(this T obj)
        {
            if (obj == null)
            {
                return true;
            }

            return obj.Equals(default(T));
        }

        public static bool IsFlagSet<T>(this T value, T flag) where T : struct
        {
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        public static T CopyClass<T>(this T source)
        {
            string json = JsonUtility.ToJson(source);
            T copiedObject = JsonUtility.FromJson<T>(json);
            return copiedObject;
        }

        /// <summary>
        /// <para><see href="https://stackoverflow.com/a/33784596/8265642"/></para>
        /// </summary>
        public static string GetNumbers(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return new string(text.Where(p => char.IsDigit(p)).ToArray());
        }

        public static bool ToBoolNullTrue(this bool? value)
        {
            if (value == null)
            {
                return true;
            }

            return value.Value;
        }

        public static bool ToBoolNullFalse(this bool? value)
        {
            if (value == null)
            {
                return false;
            }

            return value.Value;
        }
    }
}