using System.Linq;
using UnityEngine;
using UnityUtilities.Singletons;

namespace Minecraft.Scripts.World.Blocks {
    [CreateAssetMenu(menuName = "Minecraft/BlockDatabase")]
    public class BlockDatabase : ScriptableObject {
        public Block Air;
        public Block Stone;
        public Block Grass;

        public Block[] AllBlocks => new[] {
            Air,
            Stone,
            Grass
        };

        public Block GetBlock(BlockMaterial material) {
            return AllBlocks.FirstOrDefault(block => block.Material == material);
        }
    }
}