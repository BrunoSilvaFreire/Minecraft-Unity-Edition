using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using UnityEngine;

namespace Minecraft.Scripts.World.Generation {
    [CreateAssetMenu(menuName = "Minecraft/Generators/DebugGenerator")]
    public class DebugWorldGenerator : WorldGenerator {
        public int CutoutX = 2, CutoutY = 2, CutoutZ = 2;
        public int FrameX = 2, FrameY = 2, FrameZ = 2;
        public int OffsetX = 2, OffsetY = 2, OffsetZ = 2;
        public Block SolidBlock, AirBlock;
        public override void Populate(World world, ref ChunkData data, Vector2Int chunkPosition) {
            var db = world.BlockDatabase;
            for (byte x = 0; x < world.ChunkSize; x++) {
                for (byte y = 0; y < world.ChunkHeight; y++) {
                    for (byte z = 0; z < world.ChunkSize; z++) {
                        data[x, y, z] = (x + OffsetX) % CutoutX >= FrameX && (y + OffsetY) % CutoutY >= FrameY && (z + OffsetZ) % CutoutZ >= FrameZ ? SolidBlock : AirBlock;
                    }
                }
            }
        }
    }
}