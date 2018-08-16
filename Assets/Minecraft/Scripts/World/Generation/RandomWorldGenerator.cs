using Minecraft.Scripts.World.Chunks;
using UnityEngine;
using UnityUtilities;

namespace Minecraft.Scripts.World.Generation {
    [CreateAssetMenu(menuName = "Minecraft/Generators/RandomGenerator")]
    public class RandomWorldGenerator : WorldGenerator {
        public override void Populate(World world, ref ChunkData data, Vector2Int chunkPosition) {
            for (byte x = 0; x < world.ChunkSize; x++) {
                for (byte y = 0; y < world.ChunkHeight; y++) {
                    for (byte z = 0; z < world.ChunkSize; z++) {
                        var mat = (RandomUtility.NextBool() ? Blocks.Stone : Blocks.Air).Material;
                        data[x, y, z] = mat;
                    }
                }
            }
        }
    }
}