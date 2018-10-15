using System;
using System.Collections;
using System.Collections.Generic;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Jobs;
using UnityEngine;

namespace Minecraft.Scripts.World.Chunks {
    public enum GenerationState {
        Idle,
        Waiting,
        Generating,
        Finished
    }

    public sealed class CompositionGenerationStatus {
        private GenerationState state;

        public GenerationState State => state;

        public CompositionGenerationStatus() {
            state = GenerationState.Idle;
        }

        public void UpdateStatus(GenerationState state) {
            this.state = state;
        }
    }

    public sealed class MeshGenerationStatus {
        public GenerationState State {
            get;
            private set;
        }

        private Chunk chunk;
        public bool IsFinishedOrGenerating => State != GenerationState.Idle;
        public bool IsFinished => State == GenerationState.Finished;
        private Coroutine coroutine;

        public MeshGenerationStatus(Chunk chunk) {
            this.chunk = chunk;
            State = GenerationState.Idle;
        }

        public void Generate(World world) {
            if (State != GenerationState.Idle) {
                return;
            }

            coroutine = chunk.StartCoroutine(BeginGeneration(world));
        }

        public void Cancel() {
            chunk.StopCoroutine(coroutine);
        }

        private IEnumerator BeginGeneration(World world) {
            State = GenerationState.Waiting;
            var status = chunk.CompositionGenerationStatus;
            if (status == null) {
                yield break;
            }

            while (status.State != GenerationState.Finished) {
                yield return null;
            }

            State = GenerationState.Generating;

            List<Tuple<MeshBuilder, Block>> finishedMeshes = null;
            world.MeshGenerator.Enqueue(new MeshJob(
                delegate { },
                delegate { },
                chunk,
                delegate(List<Tuple<MeshBuilder, Block>> meshes) { finishedMeshes = meshes; }
            ));
            Debug.Log("Enqueued job");
            while (finishedMeshes == null) {
                yield return null;
            }

            Debug.Log("finished job");

            foreach (var tuple in finishedMeshes) {
                var block = tuple.Item2;
                var chunkGO = new GameObject($"Chunk {chunk.ChunkPosition} - SubMesh ({block.Material})");
                var t = chunkGO.transform;
                t.parent = chunk.transform;
                t.localPosition = Vector3.zero;
                var filter = chunkGO.AddComponent<MeshFilter>();
                var col = chunkGO.AddComponent<MeshCollider>();
                var ren = chunkGO.AddComponent<MeshRenderer>();
                ren.material = block.VisualMaterial;
                var mesh = tuple.Item1.Build();
                filter.sharedMesh = mesh;
                col.sharedMesh = mesh;
            }
        }
    }
}