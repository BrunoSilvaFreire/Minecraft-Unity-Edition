using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Jobs;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

namespace Minecraft.Scripts.World.Chunks {
    public sealed class MeshGenerationStatus {
        public enum GenerationState {
            Idle,
            Generating,
            Finished
        }

        public GenerationState State {
            get;
            private set;
        }

        private readonly List<Coroutine> meshRoutines = new List<Coroutine>();
        private Chunk chunk;
        public bool IsFinishedOrGenerating => State != GenerationState.Idle;
        public bool IsFinished => State == GenerationState.Finished;

        public MeshGenerationStatus(Chunk chunk) {
            this.chunk = chunk;
            State = GenerationState.Idle;
        }

        public void Generate(World world) {
            State = GenerationState.Generating;
            foreach (var meshRoutine in meshRoutines) {
                chunk.StopCoroutine(meshRoutine);
            }

            meshRoutines.Clear();
            var data = GenerateMeshJobData.From(chunk, world);
            var handles = new List<JobHandle>();
            var chunkData = chunk.ChunkData;
            foreach (var mat in chunkData.TableOfContent) {
                var job = new GenerateSubMeshJob(data, mat);
                var handle = job.Schedule();
                meshRoutines.Add(chunk.StartCoroutine(WaitForSubMesh(job, handle,
                    world.BlockDatabase.GetBlock(mat).VisualMaterial, mat)));
                handles.Add(handle);
            }

            chunk.StartCoroutine(WaitForHandlesAndDispose(handles, data));
        }

        private IEnumerator WaitForHandlesAndDispose(List<JobHandle> handles, GenerateMeshJobData data) {
            var finished = false;
            while (!finished) {
                finished = handles.All(handle => handle.IsCompleted);
                yield return null;
            }

            data.Dispose();
            State = GenerationState.Finished;
        }

        private IEnumerator WaitForSubMesh(GenerateSubMeshJob job, JobHandle handle, Material mat,
            BlockMaterial material) {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) {
                handle.Complete();
            }
#endif
            while (!handle.IsCompleted) {
                yield return null;
            }

            handle.Complete();

            var chunkGO = new GameObject($"Chunk {chunk.ChunkPosition} - SubMesh ({material})");
            var t = chunkGO.transform;
            t.parent = chunk.transform;
            t.localPosition = Vector3.zero;
            var filter = chunkGO.AddComponent<MeshFilter>();
            var col = chunkGO.AddComponent<MeshCollider>();
            var ren = chunkGO.AddComponent<MeshRenderer>();
            ren.material = mat;
            var mesh = job.Build();
            filter.sharedMesh = mesh;
            col.sharedMesh = mesh;
            job.Dispose();
        }

        public void Cancel() {
            foreach (var meshRoutine in meshRoutines) {
                chunk.StopCoroutine(meshRoutine);
            }
        }
    }
}