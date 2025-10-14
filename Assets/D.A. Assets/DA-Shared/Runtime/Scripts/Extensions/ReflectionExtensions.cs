using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DA_Assets.Extensions
{
    public static class ReflectionExtensions
    {
        // ***
        // *** MemberInfo Extensions
        // https://stackoverflow.com/a/63382419
        // ***

        public static IEnumerable<MemberInfo> GetPropertiesOrFields(this Type t, BindingFlags bf = BindingFlags.Public | BindingFlags.Instance) =>
    t.GetMembers(bf).Where(mi => mi.MemberType == MemberTypes.Field || mi.MemberType == MemberTypes.Property);

        public static IEnumerable<string> GetPropertyOrFieldNames(this Type t) => t.GetPropertiesOrFields().Select(mi => mi.Name);

        public static MemberInfo GetPropertyOrField(this Type t, string memberName, BindingFlags bf = BindingFlags.Public | BindingFlags.Instance) =>
            t.GetMember(memberName, bf).Where(mi => mi.MemberType == MemberTypes.Field || mi.MemberType == MemberTypes.Property).Single();

        public static Type GetMemberType(this MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo mfi:
                    return mfi.FieldType;
                case PropertyInfo mpi:
                    return mpi.PropertyType;
                case EventInfo mei:
                    return mei.EventHandlerType;
                default:
                    throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", nameof(member));
            }
        }

        public static bool GetCanWrite(this MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo mfi:
                    return true;
                case PropertyInfo mpi:
                    return mpi.CanWrite;
                default:
                    throw new ArgumentException("MemberInfo must be if type FieldInfo or PropertyInfo", nameof(member));
            }
        }

        public static bool GetCanRead(this MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo mfi:
                    return true;
                case PropertyInfo mpi:
                    return mpi.CanRead;
                default:
                    throw new ArgumentException("MemberInfo must be if type FieldInfo or PropertyInfo", nameof(member));
            }
        }

        public static object GetValue(this MemberInfo member, object srcObject)
        {
            switch (member)
            {
                case FieldInfo mfi:
                    return mfi.GetValue(srcObject);
                case PropertyInfo mpi:
                    return mpi.GetValue(srcObject);
                case MethodInfo mi:
                    return mi.Invoke(srcObject, null);
                default:
                    throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo or MethodInfo", nameof(member));
            }
        }
        public static T GetValue<T>(this MemberInfo member, object srcObject) => (T)member.GetValue(srcObject);

        public static void SetValue(this MemberInfo member, object destObject, object value)
        {
            switch (member)
            {
                case FieldInfo mfi:
                    mfi.SetValue(destObject, value);
                    break;
                case PropertyInfo mpi:
                    mpi.SetValue(destObject, value);
                    break;
                case MethodInfo mi:
                    mi.Invoke(destObject, new object[] { value });
                    break;
                default:
                    throw new ArgumentException("MemberInfo must be of type FieldInfo, PropertyInfo or MethodInfo", nameof(member));
            }
        }
        public static void SetValue<T>(this MemberInfo member, object destObject, T value) => member.SetValue(destObject, (object)value);
    }
}
