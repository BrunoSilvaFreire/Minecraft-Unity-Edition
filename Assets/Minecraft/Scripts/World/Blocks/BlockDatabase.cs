using System.Linq;
using UnityEngine;
using UnityUtilities.Singletons;

namespace Minecraft.Scripts.World.Blocks {
    [CreateAssetMenu(menuName = "Minecraft/BlockDatabase")]
    public class BlockDatabase : ScriptableObject {
        public Block[] Blocks;

        public Block GetBlock(BlockMaterial material) {
            return Blocks.FirstOrDefault(block => block.Material == material);
        }
    }
}