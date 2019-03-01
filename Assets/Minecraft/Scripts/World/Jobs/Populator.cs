using System;
using Minecraft.Scripts.Utility.Multithreading;
using Minecraft.Scripts.World.Chunks;
using UnityEngine.Events;

namespace Minecraft.Scripts.World.Jobs {
    [Serializable]
    public sealed class Populator : JobSystem<PopulatorWorker, PopulateJob> {
        private World world;

        public void Initialize(World world) {
            this.world = world;
            InitializeWorkers();
        }

        protected override PopulatorWorker InstantiateWorker(byte b) {
            return new PopulatorWorker(world, b);
        }
    }

    public sealed class PopulateJob : Job<PopulateJob> {
        public PopulateJob(
            Chunk chunk,
            UnityAction<PopulateJob, JobState> callback = null
        ) : base(callback) {
            Chunk = chunk;
        }

        public Chunk Chunk {
            get;
        }

        public override string ToString() {
            return $"{nameof(Chunk)}: {Chunk.ChunkPosition}";
        }
    }

    public sealed class PopulatorWorker : AbstractWorker<PopulateJob> {
        private readonly World world;

        public PopulatorWorker(World w, byte workerId) : base(workerId) {
            world = w;
        }

        protected override void Execute(PopulateJob job) {
            var data = new ChunkData(world.ChunkSize, world.ChunkHeight);
            var chunk = job.Chunk;
            world.Generator.Populate(world, ref data, chunk.ChunkPosition);
            chunk.ChunkData = data;
        }
    }
}