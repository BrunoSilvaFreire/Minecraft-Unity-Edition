using System;
using System.Collections;
using System.Collections.Generic;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.Utility.Multithreading;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using Minecraft.Scripts.World.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityUtilities;

namespace Minecraft.Scripts.World.Jobs {
    [Serializable]
    public class MeshGenerator : JobSystem<MeshWorker, MeshJob> {
        // TODO: Disgusting, I Know, find a way to fix this
        public Vector3 PriorityPosition;

        protected override MeshWorker InstantiateWorker(byte b) {
            return new MeshWorker(b, this);
        }

        public void Initialize() {
            InitializeWorkers();
        }
    }

    public class MeshJob : Job {
        private Chunk chunk;
        private UnityAction<List<Tuple<MeshBuilder, Block>>> onCalculated;

        public MeshJob(
            UnityAction startedCallback,
            UnityAction finishedCallback,
            Chunk chunk,
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
        private readonly MeshGenerator owner;

        public MeshWorker(byte workerId, MeshGenerator meshGenerator) : base(workerId) {
            owner = meshGenerator;
        }

        protected override MeshJob Dequeue() {
            var pos = owner.PriorityPosition;
            var size = World.Instance.ChunkSize;
            var offset = new Vector2(size + 0.5F, size + 0.5F);
            // Using enumerator to ensure safe async modification
            var e = jobQueue.ToArray();
            MeshJob min = null;
            var lastD = float.MaxValue;
            foreach (var other in e) {
                if (other == null) {
                    continue;
                }

                var dist = Vector2.Distance(pos, other.Chunk.ChunkPosition + offset);
                if (min != null && dist > lastD) {
                    continue;
                }

                min = other;
                lastD = dist;
            }

            return min;
        }

        protected override void Execute(MeshJob job) {
            var data = job.Chunk.ChunkData;
            var pending = new Queue<Block>();
            pending.Enqueue(data[0]);
            var completed = new HashSet<Block>();
            var meshes = new List<Tuple<MeshBuilder, Block>>();
            do {
                var current = pending.Dequeue();
                meshes.Add(new Tuple<MeshBuilder, Block>(GenerateSubMesh(current, data, pending, completed), current));
                completed.Add(current);
            } while (pending.Count > 0);

            job.OnCalculated(meshes);
        }

        private static MeshBuilder GenerateSubMesh(
            Block targetMaterial,
            ChunkData chunkData,
            Queue<Block> pending,
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