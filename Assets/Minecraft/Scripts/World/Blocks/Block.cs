using System;
using Minecraft.Scripts.Items;
using Minecraft.Scripts.Items.Misc;
using UnityEngine;

namespace Minecraft.Scripts.World.Blocks {
    public enum BlockCompositionType : byte {
        Invisible,
        Translucent,
        Opaque
    }

    [CreateAssetMenu(menuName = "Minecraft/Block")]
    public class Block : ScriptableObject {
        [SerializeField]
        private BlockMaterial material;

        [SerializeField]
        private BlockAudio audio;

        [SerializeField]
        private BlockCompositionType composition;

        [SerializeField]
        private float hardness;

        public Material VisualMaterial;
        public Color SignatureColor = Color.magenta;
        public BlockMaterial Material => material;
        public bool Opaque => composition == BlockCompositionType.Opaque;

        public BlockCompositionType Composition => composition;

        public float Hardness => hardness;

        public BlockAudio Audio => audio;
        public ItemDrop[] Drops;

        public override string ToString() {
            return $"Block({nameof(material)}: {material}, {nameof(composition)}: {composition})";
        }
    }

    [Serializable]
    public class BlockAudio {
        public bool Proxy;
        public Block ProxyBlock;

        [SerializeField]
        private string stepEvent;

        [SerializeField]
        private string hitEvent;

        [SerializeField]
        private string digEvent;

        public string StepEvent => Proxy ? ProxyBlock.Audio.stepEvent : stepEvent;

        public string HitEvent => Proxy ? ProxyBlock.Audio.hitEvent : hitEvent;
        public string DigEvent => Proxy ? ProxyBlock.Audio.digEvent : digEvent;
    }
}