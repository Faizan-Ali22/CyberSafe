using DA_Assets.DAI;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.DAG
{
    [CustomEditor(typeof(DAGradient)), CanEditMultipleObjects]
    public class DAGradientEditor : Editor
    {
        [SerializeField] DAInspector gui;
        [SerializeField] Texture2D assetLogo;

        public DAGradient monoBeh;

        private void OnEnable()
        {
            monoBeh = (DAGradient)target;
        }

        public override void OnInspectorGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = gui.ColoredStyle.Background,
                Body = () =>
                {
                    if (assetLogo != null)
                    {
                        //gui.DrawAssetHeader(assetLogo);
                    }

                    gui.SerializedPropertyField<DAGradient>(serializedObject, x => x.Gradient, false);
                    gui.Space5();
                    gui.SerializedPropertyField<DAGradient>(serializedObject, x => x.BlendMode);
                    gui.Space5();
                    gui.SerializedPropertyField<DAGradient>(serializedObject, x => x.Intensity);
                    gui.Space5();
                    gui.SerializedPropertyField<DAGradient>(serializedObject, x => x.Angle);

                    gui.DrawFooter();
                }
            });
        }
    }
}