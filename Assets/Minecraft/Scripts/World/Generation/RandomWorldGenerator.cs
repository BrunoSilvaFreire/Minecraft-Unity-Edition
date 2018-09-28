using System.Collections.Generic;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using UnityEngine;
using UnityUtilities;

namespace Minecraft.Scripts.World.Generation {
    [CreateAssetMenu(menuName = "Minecraft/Generators/RandomGenerator")]
    public class RandomWorldGenerator : WorldGenerator {
        public float PerlinScale = 1;
        public List<Block> Blocks;
        public float GrassLevel;

        public override void Populate(World world, ref ChunkData data, Vector2Int chunkPosition) {
            var db = world.BlockDatabase;
            var worldSize = (world.SpawnSize * world.ChunkSize * 2);
            for (byte x = 0; x < world.ChunkSize; x++) {
                for (byte z = 0; z < world.ChunkSize; z++) {
                    float pX = chunkPosition.x * world.ChunkSize + x;
                    float pZ = chunkPosition.y * world.ChunkSize + z;
                    var perlin = Mathf.PerlinNoise(pZ / worldSize, pX / worldSize) * PerlinScale;
                    var grass = perlin - GrassLevel;
                    for (byte y = 0; y < world.ChunkHeight; y++) {
                        data[x, y, z] = perlin < y ? db.Air : Blocks.RandomElement();
                    }
                }
            }
        }
    }
}