using System.Linq;

namespace Minecraft.Scripts.World {
    public class Block {
        public Block(BlockMaterial material, bool opaque) {
            this.material = material;
            this.opaque = opaque;
        }

        private readonly BlockMaterial material;
        private readonly bool opaque;

        public BlockMaterial Material => material;

        public bool Opaque => opaque;

        public override string ToString() {
            return $"Block({nameof(material)}: {material}, {nameof(opaque)}: {opaque})";
        }
    }

    public static class Blocks {
        public static readonly Block Air = new Block(new BlockMaterial(0), false);
        public static readonly Block Stone = new Block(new BlockMaterial(1), true);

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