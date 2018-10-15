using System;
using System.Collections.Generic;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.Utility.Multithreading;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using Minecraft.Scripts.World.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Minecraft.Scripts.World.Jobs {
    [Serializable]
    public class MeshGenerator : JobSystem<MeshWorker, MeshJob> {
        protected override MeshWorker InstantiateWorker(byte b) {
            return new MeshWorker(b);
        }

        public void Initialize() {
            InitializeWorkers();
        }
    }

    public class MeshJob : Job {
        private Chunk chunk;
        private UnityAction<List<Tuple<MeshBuilder, Block>>> onCalculated;

        public MeshJob(UnityAction startedCallback, UnityAction finishedCallback, Chunk chunk,
            UnityAction<List<Tuple<MeshBuilder, Block>>> onCalculated) : base(startedCallback, finishedCallback) {
            this.chunk = chunk;
            this.onCalculated = onCalculated;
        }

        public Chunk Chunk => chunk;

        public void OnCalculated(List<Tuple<MeshBuilder, Block>> meshes) {
            onCalculated(meshes);
        }
    }

    public sealed class MeshWorker : AbstractWorker<MeshJob> {
        public MeshWorker(byte workerId) : base(workerId) { }

        protected override void Execute(MeshJob job) {
            var data = job.Chunk.ChunkData;
            var pending = new Queue<Block>();
            pending.Enqueue(data[0]);
            var completed = new HashSet<Block>();
            var meshes = new List<Tuple<MeshBuilder, Block>>();
            Debug.Log("Executing job " + job);
            do {
                var current = pending.Dequeue();
                Debug.Log("Generating sub mesh for " + current);
                meshes.Add(new Tuple<MeshBuilder, Block>(GenerateSubMesh(current, data, pending, completed), current));
                completed.Add(current);
            } while (pending.Count > 0);

            job.OnCalculated(meshes);
        }

        private static MeshBuilder GenerateSubMesh(Block targetMaterial, ChunkData chunkData, Queue<Block> pending,
            ICollection<Block> completed) {
            var chunkSize = chunkData.ChunkSize;
            var chunkHeight = chunkData.ChunkHeight;
            var builder = new MeshBuilder();
            for (byte x = 0; x < chunkSize; x++) {
                for (byte y = 0; y < chunkHeight; y++) {
                    for (byte z = 0; z < chunkSize; z++) {
                        var currentBlock = chunkData[x, y, z];
                        if (!currentBlock.Visible) {
                            continue;
                        }

                        if (currentBlock != targetMaterial) {
                            if (!completed.Contains(currentBlock) && !pending.Contains(currentBlock)) {
                                pending.Enqueue(currentBlock);
                            }
                            continue;
                        }

                        foreach (var face in BlockFaces.Faces) {
                            var dir = face.ToDirection();
                            var isTopOrBottom = y == 0 && dir.y < 0 || y == chunkHeight - 1 && dir.y > 0;
                            if (isTopOrBottom) {
                                continue;
                            }

                            if (chunkData.TryGet(x + dir.x, y + dir.y, z + dir.z, out var neighbor)) {
                                if (neighbor.Composition == targetMaterial.Composition) {
                                    continue;
                                }
                            }

                            builder.AddFace(x, y, z, face);
                        }
                    }
                }
            }

            return builder;
        }
    }
}