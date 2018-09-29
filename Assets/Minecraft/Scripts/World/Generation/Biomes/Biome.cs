using Minecraft.Scripts.World.Chunks;
using UnityEngine;

namespace Minecraft.Scripts.World.Generation.Biomes {
    public abstract class Biome : ScriptableObject {
        public abstract void Populate(World world, ref ChunkData data, Vector2Int chunkPosition);
    }
}