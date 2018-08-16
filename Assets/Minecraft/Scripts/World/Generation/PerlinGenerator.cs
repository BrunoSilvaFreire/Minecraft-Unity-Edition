using Minecraft.Scripts.Utility;
using Minecraft.Scripts.World.Chunks;
using UnityEngine;

namespace Minecraft.Scripts.World.Generation {
    [CreateAssetMenu(menuName = "Minecraft/Generators/PerlinGenerator")]
    public class PerlinGenerator : WorldGenerator {
        public float PerlinScale = 1;
        public int Octaves;
        public float Amplitude;

        public override void Populate(World world, ref ChunkData data, Vector2Int chunkPosition) {
            //var noise = PerlinNoise.Generate(pX, pZ, Octaves, Amplitude, world.Seed);
            var worldSize = (world.SpawnSize * world.ChunkSize * 2);
            for (byte x = 0; x < world.ChunkSize; x++) {
                for (byte z = 0; z < world.ChunkSize; z++) {
                    float pX = chunkPosition.x * world.ChunkSize + x;
                    float pZ = chunkPosition.y * world.ChunkSize + z;
                    var perlin = Mathf.PerlinNoise(pZ / worldSize, pX / worldSize) * PerlinScale;
                    for (byte y = 0; y < world.ChunkHeight; y++) {
                        data[x, y, z] = (perlin < y ? Blocks.Air : Blocks.Stone).Material;
                    }
                }
            }
        }
    }
}