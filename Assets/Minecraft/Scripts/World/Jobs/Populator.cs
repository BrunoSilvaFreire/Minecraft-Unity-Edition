using System;
using System.Collections.Generic;
using System.Threading;
using Minecraft.Scripts.Utility.Multithreading;
using Minecraft.Scripts.World.Chunks;
using Minecraft.Scripts.World.Jobs;
using UnityEngine;
using UnityEngine.Events;
using UnityUtilities;

namespace Minecraft.Scripts.World {
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

    public sealed class PopulateJob : Job {
        private Chunk chunk;

        public PopulateJob(UnityAction startedCallback, UnityAction finishedCallback, Chunk chunk) : base(
            startedCallback, finishedCallback) {
            this.chunk = chunk;
        }

        public Chunk Chunk => chunk;
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