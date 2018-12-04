using System;
using UnityEngine;

namespace Minecraft.Scripts.Entities.Items {
    [Serializable]
    public class ItemFloatingParameters {
        public float FloatingDuration = 2;
        public AnimationCurve HeightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    public partial class ItemWisp {
        public float RotationSpeed = 5;
        public ItemFloatingParameters FloatingParameters;
        public GameObject Body;
        private float currentFloatPos;

        private void UpdateVanity() {
            var bt = Body.transform;
            bt.Rotate(0, RotationSpeed * Time.deltaTime, 0);
            currentFloatPos += Time.deltaTime;
            var maxDur = FloatingParameters.FloatingDuration;
            if (currentFloatPos > maxDur) {
                currentFloatPos -= maxDur;
            }

            var pos = bt.localPosition;
            pos.y = FloatingParameters.HeightCurve.Evaluate(currentFloatPos / maxDur);
            bt.localPosition = pos;
        }

        
    }
}