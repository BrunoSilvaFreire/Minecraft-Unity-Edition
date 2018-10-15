using System;
using System.Collections.Generic;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.World.Blocks;
using UnityEngine;
using Minecraft.Scripts.World.Utilities;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Minecraft.Scripts.World.Chunks {
    [Serializable]
    public struct Vector3Byte {
        // ReSharper disable InconsistentNaming
        public byte x, y, z;
        public static readonly Vector3Byte zero = new Vector3Byte(0, 0, 0);

        public Vector3Byte(byte x, byte y, byte z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString() {
            return $"({nameof(x)}: {x}, {nameof(y)}: {y}, {nameof(z)}: {z})";
        }
    }

    public class Chunk : MonoBehaviour {
        public bool ChunkInitialized {
            get;
            private set;
        }

        private Vector2Int chunkPosition;

        public Vector2Int ChunkPosition {
            get {
                if (!ChunkInitialized) {
                    throw new ChunkNotInitializedException();
                }

                return chunkPosition;
            }
        }

        private ChunkData chunkData;

        public ChunkData ChunkData {
            get {
                if (!ChunkInitialized) {
                    throw new ChunkNotInitializedException();
                }

                return chunkData;
            }
            set => chunkData = value;
        }


        public void Initialize(Vector2Int position, ChunkData data) {
            if (ChunkInitialized) {
                return;
            }

            ChunkInitialized = true;
            chunkPosition = position;
            chunkData = data;
        }

        private CompositionGenerationStatus compositionGenerationStatus;
        private MeshGenerationStatus meshGenerationStatus;

        public CompositionGenerationStatus CompositionGenerationStatus => compositionGenerationStatus;

        public MeshGenerationStatus MeshGenerationStatus => meshGenerationStatus;

        public bool IsMeshGenerated {
            get {
                if (compositionGenerationStatus.State != GenerationState.Finished) {
                    return false;
                }

                return meshGenerationStatus != null && meshGenerationStatus.IsFinishedOrGenerating;
            }
        }

        public void GenerateComposition(World world) {
            if (compositionGenerationStatus != null && compositionGenerationStatus.State != GenerationState.Idle) {
                return;
            }

            compositionGenerationStatus = new CompositionGenerationStatus();
            compositionGenerationStatus.UpdateStatus(GenerationState.Waiting);

            var job = new PopulateJob(OnBeginCompositionGeneration, OnFinishedCompositionGeneration, this);
            world.Populator.Enqueue(job);
        }

        private void OnBeginCompositionGeneration() {
            CompositionGenerationStatus.UpdateStatus(GenerationState.Generating);
        }

        private void OnFinishedCompositionGeneration() {
            CompositionGenerationStatus.UpdateStatus(GenerationState.Finished);
        }

        public void GenerateMesh(World world, bool cancelCurrentGeneration = true) {
            if (compositionGenerationStatus.State != GenerationState.Finished) {
                GenerateComposition(world);
            }

            var currentState = meshGenerationStatus;
            if (currentState != null && !currentState.IsFinished) {
                if (cancelCurrentGeneration) {
                    currentState.Cancel();
                } else {
                    return;
                }
            }


            meshGenerationStatus = new MeshGenerationStatus(this);
            meshGenerationStatus.Generate(world);
        }

        private bool shouldClearMeshes;

        public void ClearMeshes() {
            for (var i = 0; i < transform.childCount; i++) {
                var obj = transform.GetChild(i).gameObject;
#if UNITY_EDITOR
                if (!EditorApplication.isPlaying) {
                    DestroyImmediate(obj);
                    continue;
                }
#endif
                Destroy(obj);
            }
        }
    }

    public class ChunkNotInitializedException : Exception { }
}