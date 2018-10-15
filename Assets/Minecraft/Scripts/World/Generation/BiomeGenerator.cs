using Minecraft.Scripts.World.Chunks;
using Minecraft.Scripts.World.Generation.Biomes;
using UnityEngine;
using UnityUtilities;
using Random = System.Random;

namespace Minecraft.Scripts.World.Generation {
    [CreateAssetMenu(menuName = "Minecraft/Generators/BiomeGenerator")]
    public class BiomeGenerator : WorldGenerator {
        [SerializeField]
        public Biome[] biomes;

        public override void Populate(World world, ref ChunkData data, Vector2Int chunkPosition) {
            FindBiome(world, chunkPosition).Populate(world, ref data, chunkPosition);
        }

        private static readonly Random random = new Random();

        private Biome FindBiome(World world, Vector2Int chunkPosition) {
            return biomes[random.Next(biomes.Length)];
        }
    }
}