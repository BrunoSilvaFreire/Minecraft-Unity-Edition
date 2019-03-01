using System;
using FMODUnity;
using Minecraft.Scripts.Items.Misc;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minecraft.Scripts.World.Blocks {
    public enum BlockCompositionType : byte {
        Invisible,
        Translucent,
        Opaque
    }

    [CreateAssetMenu(menuName = "Minecraft/Block")]
    public class Block : ScriptableObject {
        public const string ValuesGroup = "Values";
        public const string VisualGroup = "Visual";
        public const string IdGroup = "General";

        [SerializeField, BoxGroup(IdGroup)]
        private BlockMaterial material;

        [SerializeField, BoxGroup(ValuesGroup)]
        private BlockAudio audio;

        [SerializeField, BoxGroup(ValuesGroup)]
        private BlockCompositionType composition;

        [SerializeField, BoxGroup(ValuesGroup)]
        private float hardness;

        [BoxGroup(VisualGroup)]
        public Material VisualMaterial;

        [BoxGroup(ValuesGroup)]
        public Color SignatureColor = Color.magenta;

        public BlockMaterial Material => material;
        public bool Visible => composition == BlockCompositionType.Opaque;

        public BlockCompositionType Composition => composition;

        public float Hardness => hardness;

        public BlockAudio Audio => audio;

        public GameObject VisualOverride {
            get {
                return visualOverride;
            }
            set {
                visualOverride = value;
                hasVisualOverride = value != null;
            }
        }

        [BoxGroup(ValuesGroup)]
        public ItemDrop[] Drops;

        [SerializeField, BoxGroup(VisualGroup), InlineEditor]
        private GameObject visualOverride;

        #region ISSUE_ASYNC_VISUAL_OVERRIDE

        /* This exists because when the mesh generator "parses" this block, it can't use any operation with visualOverride.
         * Because Unity doesn't allow us to access these properties out of the main thread.
         * And yes, I desperately need to find a workaround this issue, since this solution is really buggy and fragile
         *
         * TODO: Find a way to check if a block has a visual override on a thread other than the main one
         */
        [ShowInInspector, ReadOnly]
        private bool hasVisualOverride;

        private void OnEnable() {
            // Check initially for visual override
            hasVisualOverride = VisualOverride != null;
        }

        #endregion

        public bool HasVisualOverride => hasVisualOverride;

        public override string ToString() {
            return $"Block({nameof(material)}: {material}, {nameof(composition)}: {composition})";
        }
    }

    [Serializable]
    public class BlockAudio {
        public const string FMODGroup = "FMOD Audios";
        public bool Proxy;

        [ShowIf(nameof(Proxy))]
        public Block ProxyBlock;

        [BoxGroup(FMODGroup), SerializeField, EventRef]
        private string stepEvent;

        [BoxGroup(FMODGroup), SerializeField, EventRef]
        private string hitEvent;

        [BoxGroup(FMODGroup), SerializeField, EventRef]
        private string digEvent;

        public string StepEvent => Proxy ? ProxyBlock.Audio.stepEvent : stepEvent;

        public string HitEvent => Proxy ? ProxyBlock.Audio.hitEvent : hitEvent;
        public string DigEvent => Proxy ? ProxyBlock.Audio.digEvent : digEvent;
    }
}