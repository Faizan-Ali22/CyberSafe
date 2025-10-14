using UnityEditor;
using UnityEngine;

namespace DA_Assets.DAI
{
    public class DAEditor<T1, T2> : Editor, IDAEditor where T1 : DAEditor<T1, T2> where T2 : UnityEngine.Object
    {
        public T2 monoBeh;

        [SerializeField] DAInspector _gui;
        public DAInspector gui { get => _gui; set => _gui = value; }

        protected virtual void OnEnable()
        {
            monoBeh = (T2)target;
            DARunner.update += Repaint;
        }

        protected virtual void OnDisable()
        {
            DARunner.update -= Repaint;
        }
    }
}