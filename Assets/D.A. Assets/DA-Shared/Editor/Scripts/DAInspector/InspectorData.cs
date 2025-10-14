using DA_Assets.Constants;
using DA_Assets.Singleton;
using UnityEngine;

namespace DA_Assets.DAI
{
    [CreateAssetMenu(menuName = DAConstants.Publisher + "/" + "Inspector Data")]
    [ResourcePath("")]
    public class InspectorData : ScriptableObject
    {
        [SerializeField] InspectorResources resources;
        public InspectorResources Resources { get => resources; set => resources = value; }

        [SerializeField] DaiStyle _basicStyle;
        public DaiStyle BasicStyle { get => _basicStyle; set => _basicStyle = value; }
    }
}