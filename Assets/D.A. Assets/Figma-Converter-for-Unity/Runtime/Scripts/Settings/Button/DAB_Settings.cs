#if DABUTTON_EXISTS
using DA_Assets.DAB;
using DA_Assets.DAI;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class DAB_Settings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] AnimatedProperty<Vector2> _scaleProperties;
        [SerializeProperty(nameof(_scaleProperties))]
        public AnimatedProperty<Vector2> ScaleProperties { get => _scaleProperties; set => _scaleProperties = value; }

        [SerializeField] EventAnimations _scaleAnimations;
        [SerializeProperty(nameof(_scaleAnimations))]
        public EventAnimations ScaleAnimations { get => _scaleAnimations; set => _scaleAnimations = value; }

        //////////////////////

        [SerializeField] EventAnimations _colorAnimations;
        [SerializeProperty(nameof(_colorAnimations))]
        public EventAnimations ColorAnimations { get => _scaleAnimations; set => _scaleAnimations = value; }

        [SerializeField] EventAnimations _spriteAnimations;
        [SerializeProperty(nameof(_spriteAnimations))]
        public EventAnimations SpriteAnimations { get => _spriteAnimations; set => _spriteAnimations = value; }

        public override void Reset()
        {
            var defaultEventAnims = DabConfig.Instance.DefaultEventAnimations;

            _scaleProperties = DabConfig.Instance.DefaultScaleProps;
            _scaleAnimations = defaultEventAnims;
            _colorAnimations = defaultEventAnims;
            _spriteAnimations = defaultEventAnims;
        }
    }
}
#endif