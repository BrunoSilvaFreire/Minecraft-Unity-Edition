using System;
using System.Collections;
using System.Collections.Generic;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.Utility.Multithreading;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Jobs;
using UnityEngine;
using UnityUtilities;

namespace Minecraft.Scripts.World.Chunks {
    public partial class Chunk {
        public JobState CompositionGenerationStatus {
            get;
            private set;
        } = JobState.Idle;

        public JobState MeshGenerationStatus {
            get;
            private set;
        } = JobState.Idle;


        public void GenerateComposition(World world) {
            world.Populator.Enqueue(
                new PopulateJob(
                    this,
                    (arg0, state) => { CompositionGenerationStatus = state; }
                )
            );
        }

        private Coroutine meshGenerationRoutine;

        public void GenerateMesh(World world, bool cancelCurrentGeneration = true) {
            if (CompositionGenerationStatus == JobState.Idle) {
                GenerateComposition(world);
            }

            CoroutineUtility.ReplaceCoroutine(ref meshGenerationRoutine, this, WaitForCompositionGenerationAndEnqueueMeshGeneration(world));
        }

        // Long name, but very clear
        private IEnumerator WaitForCompositionGenerationAndEnqueueMeshGeneration(World world) {
            // We pre-set the status to waiting so that other generation routines don't get queued
            MeshGenerationStatus = JobState.Waiting;

            while (CompositionGenerationStatus != JobState.Finished) {
                yield return null;
            }

            List<Tuple<MeshBuilder, Block>> finishedMeshes = null;
            List<ChunkObject> specialObjects = null;
            world.MeshGenerator.Enqueue(
                new MeshJob(
                    this,
                    (arg0, state) => {
                        MeshGenerationStatus = state;
                        if (state == JobState.Finished) {
                            finishedMeshes = arg0.GeneratedMeshes;
                            specialObjects = arg0.SpecialObjects;
                        }
                    }
                ));
            while (MeshGenerationStatus != JobState.Finished) {
                yield return null;
            }

            ClearMeshes();
            foreach (var tuple in finishedMeshes) {
                var block = tuple.Item2;
                var chunkGO = new GameObject($"Chunk {ChunkPosition} - SubMesh ({block.Material})") {
                    layer = gameObject.layer,
                    isStatic = true
                };
                var t = chunkGO.transform;
                t.parent = transform;
                t.localPosition = Vector3.zero;
                var filter = chunkGO.AddComponent<MeshFilter>();
                var col = chunkGO.AddComponent<MeshCollider>();
                var ren = chunkGO.AddComponent<MeshRenderer>();
                ren.material = block.VisualMaterial;
                var mesh = tuple.Item1.Build();
                filter.sharedMesh = mesh;
                col.sharedMesh = mesh;
            }

            foreach (var obj in specialObjects) {
                obj.Install(this);
            }
        }
    }
}