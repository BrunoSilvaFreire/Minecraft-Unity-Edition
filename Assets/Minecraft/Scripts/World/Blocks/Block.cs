using UnityEngine;

namespace Minecraft.Scripts.World.Blocks {
    [CreateAssetMenu(menuName = "Minecraft/Block")]
    public class Block : ScriptableObject {
        [SerializeField]
        private BlockMaterial material;

        [SerializeField]
        private bool opaque;

        [SerializeField]
        private float hardness;

        public Material VisualMaterial;

        public BlockMaterial Material => material;

        public bool Opaque => opaque;

        public float Hardness => hardness;

        public override string ToString() {
            return $"Block({nameof(material)}: {material}, {nameof(opaque)}: {opaque})";
        }
    }
}