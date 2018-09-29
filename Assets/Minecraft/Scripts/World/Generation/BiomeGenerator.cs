using Minecraft.Scripts.World.Chunks;
using Minecraft.Scripts.World.Generation.Biomes;
using UnityEngine;
using UnityUtilities;

namespace Minecraft.Scripts.World.Generation {
    [CreateAssetMenu(menuName = "Minecraft/Generators/BiomeGenerator")]
    public class BiomeGenerator : WorldGenerator {
        [SerializeField]
        private Biome[] biomes;

        public override void Populate(World world, ref ChunkData data, Vector2Int chunkPosition) {
            FindBiome(world, chunkPosition).Populate(world, ref data, chunkPosition);
        }

        private Biome FindBiome(World world, Vector2Int chunkPosition) {
            return biomes.RandomElement();
        }
    }
}