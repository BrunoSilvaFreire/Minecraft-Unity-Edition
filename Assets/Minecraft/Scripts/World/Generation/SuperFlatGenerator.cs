using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using UnityEngine;

namespace Minecraft.Scripts.World.Generation {
    [CreateAssetMenu(menuName = "Minecraft/Generators/SuperFlatGenerator")]
    public class SuperFlatGenerator : WorldGenerator {
        public Block SolidBlock, AirBlock;

        public override void Populate(World world, ref ChunkData data, Vector2Int chunkPosition) {
            for (byte x = 0; x < world.ChunkSize; x++) {
                for (byte y = 0; y < world.ChunkHeight; y++) {
                    for (byte z = 0; z < world.ChunkSize; z++) {
                        data[x, y, z] = y == 0 ? SolidBlock : AirBlock;
                    }
                }
            }
        }
    }
}