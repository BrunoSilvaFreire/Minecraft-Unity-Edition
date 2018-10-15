using Minecraft.Scripts.Utility;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Minecraft.Scripts.Visual {
    [ExecuteInEditMode]
    public class BloomBeater : MonoBehaviour {
        public PostProcessProfile Profile;
        public RhythmBeater Beater;
        public float Scale;
        private void Update() {
            var bloom = Profile.GetSetting<Bloom>();
            bloom.intensity.value = Beater.Evaluate() * Scale;
        }
    }
}