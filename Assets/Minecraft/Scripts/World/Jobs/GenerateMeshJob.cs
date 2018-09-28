using Minecraft.Scripts.World.Chunks;
using Unity.Jobs;

namespace Minecraft.Scripts.World.Jobs {
    public struct GenerateMeshJob : IJob {
        private ChunkShapeData chunkShapeData;

        public GenerateMeshJob(ChunkShapeData chunkShapeData) {
            this.chunkShapeData = chunkShapeData;
        }

        public void Execute() { }
    }
}