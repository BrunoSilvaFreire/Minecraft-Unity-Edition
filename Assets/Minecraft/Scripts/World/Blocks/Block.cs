using System.Linq;

namespace Minecraft.Scripts.World.Blocks {
    public class Block {
        public Block(BlockMaterial material, bool opaque, float hardness) {
            this.material = material;
            this.opaque = opaque;
            this.hardness = hardness;
        }

        private readonly BlockMaterial material;
        private readonly bool opaque;
        private readonly float hardness;
        public BlockMaterial Material => material;

        public bool Opaque => opaque;

        public float Hardness => hardness;

        public override string ToString() {
            return $"Block({nameof(material)}: {material}, {nameof(opaque)}: {opaque})";
        }
    }

    public static class Blocks {
        public static readonly Block Air = new Block(new BlockMaterial(0), false, 0);
        public static readonly Block Stone = new Block(new BlockMaterial(1), true, 1);

        public static readonly Block[] AllBlocks = {
            Air,
            Stone
        };

        public static Block GetBlock(BlockMaterial material) {
            return AllBlocks.FirstOrDefault(block => block.Material == material);
        }

        public static bool IsOpaque(BlockMaterial material) {
            //Debug.Log("Block @ " + material + " = " + GetBlock(material));
            return GetBlock(material).Opaque;
        }
    }
}