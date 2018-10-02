using System.Collections.Generic;
using System.Threading;
using Minecraft.Scripts.World.Chunks;
using Minecraft.Scripts.World.Generation;

namespace Minecraft.Scripts.World.Jobs {

    public class ChunkPopulator {
        private Thread thread;
        private Queue<Chunk> toGenerate = new Queue<Chunk>();
        private WorldGenerator generator;
        private World world;

        public ChunkPopulator(World world, WorldGenerator generator) {
            this.world = world;
            this.generator = generator;
            thread = new Thread(Start);
        }

        public void Add(Chunk chunk) {
            toGenerate.Enqueue(chunk);
        }

        private void Start() {
            Chunk currentChunk;
            while ((currentChunk = toGenerate.Dequeue()) != null) {
                var data = new ChunkData(world.ChunkSize, world.ChunkHeight);
                generator.Populate(world, ref data, currentChunk.ChunkPosition);
            }
        }
    }
}