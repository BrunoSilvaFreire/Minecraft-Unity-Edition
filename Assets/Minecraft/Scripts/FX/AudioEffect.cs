using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Minecraft.Scripts.FX.Features;
using Shiroi.FX.Effects;
using Shiroi.FX.Features;
using Debug = UnityEngine.Debug;

namespace Minecraft.Scripts.FX {
    public abstract class AudioEffect : Effect {
        public override void Play(EffectContext context) {
            EventInstance instance;
            var eventName = GetEventName(context);
            try {
                instance = RuntimeManager.CreateInstance(eventName);
            } catch (Exception e) {
                Debug.LogWarning($"Exception occoured when playing FMOD event \'{eventName}\': {e.Message}");
                return;
            }

            var positoin = context.GetRequiredFeature<PositionFeature>();
            var feature = context.GetRequiredFeature<AudioFeature>();
            instance.set3DAttributes(new ATTRIBUTES_3D {
                position = positoin.Position.ToFMODVector(),
                forward = feature.Forward.ToFMODVector(),
                up = feature.Up.ToFMODVector(),
                velocity= feature.Velocity.ToFMODVector()
            });
            instance.start();
            instance.release();
        }

        protected abstract string GetEventName(EffectContext context);
    }
}