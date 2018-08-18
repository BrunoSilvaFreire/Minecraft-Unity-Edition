using System;
using Minecraft.Scripts.FX.Features;
using Shiroi.FX.Effects;
using UnityEngine;

namespace Minecraft.Scripts.FX {
    [CreateAssetMenu(menuName = "Minecraft/FX/AudioEffect")]
    public class BlockAudioEffect : AudioEffect {
        public BlockAudioType AudioType;

        protected override string GetEventName(EffectContext context) {
            var block = context.GetRequiredFeature<BlockFeature>().Block.Audio;
            switch (AudioType) {
                case BlockAudioType.Step:
                    return block.StepEvent;
                case BlockAudioType.Hit:
                    return block.HitEvent;
                case BlockAudioType.Dig:
                    return block.DigEvent;
            }

            throw new ArgumentOutOfRangeException(nameof(AudioType));
        }

        public enum BlockAudioType {
            Step,
            Hit,
            Dig
        }
    }
}