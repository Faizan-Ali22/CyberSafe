using System;
using System.Reflection;

namespace DA_Assets.Tools
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ClearAttribute : Attribute { }

    public static class ObjectCleaner
    {
        /// <summary>
        /// Cleans serialized and other fields or properties of an object marked with the <typeparamref name="T"/> attribute.
        /// This is especially useful for objects generated from Figma layouts, which produce a substantial amount of data.
        /// Cleaning these objects helps reduce memory usage and removes unnecessary serialized data from the Inspector.
        /// </summary>
        /// <typeparam name="T">The attribute type used to identify fields or properties to be cleared.</typeparam>
        /// <param name="obj">The object whose fields or properties will be cleared.</param>
        public static void ClearByAttribute<T>(object obj) where T : Attribute
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            Type type = obj.GetType();

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (Attribute.IsDefined(field, typeof(T)))
                {
                    SetMemberValue(field, obj);
                }
            }

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(T)))
                {
                    if (property.CanWrite)
                    {
                        SetMemberValue(property, obj);
                    }
                }
            }
        }

        private static void SetMemberValue(MemberInfo member, object obj)
        {
            Type memberType;
            Action<object, object> setter;

            if (member is FieldInfo field)
            {
                memberType = field.FieldType;
                setter = (target, value) => field.SetValue(target, value);
            }
            else if (member is PropertyInfo property)
            {
                memberType = property.PropertyType;
                setter = (target, value) => property.SetValue(target, value);
            }
            else
            {
                throw new ArgumentException("Unsupported.");
            }

            object defaultValue = memberType.IsValueType ? Activator.CreateInstance(memberType) : null;

            setter(obj, defaultValue);
        }
    }
}
