using DA_Assets.Extensions;
using DA_Assets.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.DAI
{
    public class MonoBehaviourLinkerRuntime<T3> : LinkerBase<T3> where T3 : UnityEngine.Object { }

    public static class MonoBehaviourLinkerExtensions
    {
        public static T2 Link<T2, T3>(this T3 monoBeh, ref T2 linker) where T3 : UnityEngine.Object where T2 : MonoBehaviourLinkerRuntime<T3>
        {
            bool needSetDirty = false;
            bool needInit = false;

            if (linker == null)
            {
                needSetDirty = true;
                needInit = true;

                AttributeValidator.Validate<T2>(typeof(SerializableAttribute));
                linker = (T2)Activator.CreateInstance(typeof(T2));
            }

            if (linker.MonoBeh == null)
            {
                needSetDirty = true;
                linker.MonoBeh = monoBeh;
            }

            if (needInit)
            {
                linker.OnLink();
            }

            if (needSetDirty)
            {
                linker.SetDirty();
            }

            return linker;
        }
    }

    public abstract class LinkerBase<T3> where T3 : UnityEngine.Object
    {
        [SerializeField] protected T3 monoBeh;
        public T3 MonoBeh
        {
            get => monoBeh;
            set => monoBeh = value;
        }

        public virtual void OnLink() { }

        public virtual void Reset() { }

        public void SetDirty()
        {
#if UNITY_EDITOR
            DARunner.ExecuteOnMainThread(() =>
            {
                monoBeh.SetDirtyExt();
            });
#endif
        }

        public void SetValue<T>(ref T currentValue, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                if (IsTypeNestedFromType(typeof(T3), typeof(ScriptableObject)))
                {
                    SceneBackuper.MakeActiveSceneDirty();
                }

                monoBeh.SetDirtyExt();
                currentValue = newValue;
            }
        }

        public static bool IsTypeNestedFromType(Type currentType, Type parentType)
        {
            while (currentType != null)
            {
                if (currentType == parentType)
                    return true;

                currentType = currentType.BaseType;
            }

            return false;
        }
    }

    public static class AttributeValidator
    {
        public static void Validate<T>(params Type[] atts)
            where T : class
        {
            Type type = typeof(T);

            foreach (Type att in atts)
            {
                if (!typeof(Attribute).IsAssignableFrom(att))
                {
                    Debug.LogError($"Type '{att.Name}' is not an Attribute type.");
                    continue;
                }

                Attribute attribute = Attribute.GetCustomAttribute(type, att);

                if (attribute == null)
                {
                    Debug.LogError($"Attribute '{att.Name}' is not applied to '{type.Name}'.");
                }
            }
        }
    }
}