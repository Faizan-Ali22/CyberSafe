using System;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class RoundingConfig
    {
        [SerializeField] int isCircleDigits = 0;
        public int IsCircleDigits => isCircleDigits;

        [SerializeField] int getBoundingSizeDigits = 2;
        public int GetBoundingSizeDigits => getBoundingSizeDigits;

        [SerializeField] int getAngleFromMatrixDigits = 2;
        public int GetAngleFromMatrixDigits => getAngleFromMatrixDigits;

        [SerializeField] int diffCheckerSizeDigits = 2;
        public int DiffCheckerSizeDigits => diffCheckerSizeDigits;

        [SerializeField] int getPaddingDigits = 0;
        public int GetPaddingDigits => getPaddingDigits;

        [SerializeField] int fontDigits = 2;
        public int FontDigits => fontDigits;

        [SerializeField] int getMaxAllowedScaleDigits = 2;
        public int GetMaxAllowedScaleDigits => getMaxAllowedScaleDigits;
    }
}