using UnityEngine;

namespace Minecraft.Scripts.Utility {
    [ExecuteInEditMode]
    public class RhythmBeater : MonoBehaviour {
        public float BPM;
        public AnimationCurve FOVCurve = AnimationCurve.Linear(0, 65, 60, 0);
        private float currentTime;
        public float SecondsPerBeat => 60 / BPM;

        private void Update() {
            currentTime += Time.deltaTime;
            var secondsPerBeat = SecondsPerBeat;
            if (currentTime > secondsPerBeat) {
                currentTime %= secondsPerBeat;
            }
        }

        public float Evaluate() {
            return FOVCurve.Evaluate(currentTime / SecondsPerBeat);
        }
    }
}