using UnityEngine;
using UnityUtilities.Singletons;

namespace Minecraft.Scripts.Ambience {
    public class DayCycleController : Singleton<DayCycleController> {
        public const string SkyTintKey = "_SkyTint";
        public const string ExposureKey = "_Exposure";
        public const string AtmosphereThicknessKey = "_AtmosphereThickness";
        public Light Light;
        public AnimationCurve LightIntensity;
        public AnimationCurve LightRotation;
        public AnimationCurve AtmosphereThickness;
        public AnimationCurve Exposure;

        public Gradient EnviromentLight;
        public Gradient SkyColor;
        public Gradient LightColor;
        public float CurrentPosition;
        public float Speed;
        public Material SkyboxMat;
        public AnimationCurve StarsAlpha;
        public ParticleSystem Stars;

        private void Update() {
            CurrentPosition += Speed * Time.deltaTime;
            if (CurrentPosition >= 1) {
                CurrentPosition--;
            }

            UpdateTo(CurrentPosition);
        }

        public void UpdateTo(float time) {
            Light.intensity = LightIntensity.Evaluate(time);
            Light.color = LightColor.Evaluate(time);
            SkyboxMat.SetColor(SkyTintKey, SkyColor.Evaluate(time));
            var stars = Stars.main;
            var startColor = stars.startColor;
            var c = startColor.color;
            c.a = StarsAlpha.Evaluate(time);
            stars.startColor = startColor;
            startColor.color = c;
            SkyboxMat.SetFloat(ExposureKey, Exposure.Evaluate(time));
            SkyboxMat.SetFloat(AtmosphereThicknessKey, AtmosphereThickness.Evaluate(time));
            var r = Light.transform.rotation.eulerAngles;
            Light.transform.rotation = Quaternion.Euler(LightRotation.Evaluate(time), r.y, r.z);
            RenderSettings.ambientSkyColor = EnviromentLight.Evaluate(time);
            DynamicGI.UpdateEnvironment();
        }
    }
}