using Minecraft.Scripts.World.Chunks;
using UnityEngine;

namespace Minecraft.Scripts.World.Generation {
    public abstract class WorldGenerator : ScriptableObject {
        public abstract void Populate(World world, ref ChunkData data, Vector2Int chunkPosition);
    }
}