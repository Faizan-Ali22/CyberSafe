using System;
using System.Collections.Generic;
using UnityEngine;

//https://github.com/Unity-Technologies/SimpleUIDemo/blob/c33907f1ffa0f8a30b46f958d3cf45e5da08b01c/Tiny3D/Library/PackageCache/com.unity.render-pipelines.core%407.1.6/Runtime/Common/SerializedDictionary.cs

namespace DA_Assets.Tools
{
    [Serializable]
    public class SerializedDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField] List<K> m_Keys = new List<K>();

        [SerializeField] List<V> m_Values = new List<V>();

        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();

            foreach (var kvp in this)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < m_Keys.Count; i++)
            {
                try
                {
                    Add(m_Keys[i], m_Values[i]);
                }
                catch
                {

                }
            }
             
            m_Keys.Clear();
            m_Values.Clear();
        }
    }
}