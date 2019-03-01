using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minecraft.Scripts.World.Blocks {
    [CreateAssetMenu(menuName = "Minecraft/BlockDatabase")]
    public class BlockDatabase : ScriptableObject {
        public List<Block> Blocks;

        public Block GetBlock(BlockMaterial material) {
            return Blocks.FirstOrDefault(block => block.Material == material);
        }
    }
}