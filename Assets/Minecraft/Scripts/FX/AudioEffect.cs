using FMOD.Studio;
using FMODUnity;
using Shiroi.FX.Effects;
using UnityEngine;

namespace Minecraft.Scripts.FX {
    [CreateAssetMenu(menuName = "Minecraft/FX/AudioEffect")]
    public class AudioEffect : Effect {
        public string FMODEvent;
        public override void Play(EffectContext context) {
            var instance = RuntimeManager.CreateInstance(FMODEvent);
        }
    }
}