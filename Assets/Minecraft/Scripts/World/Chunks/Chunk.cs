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
        }

        public bool IsMeshGenerated {
            get {
                return isMeshGenerated;
            }
        }

        public void Initialize(Material chunkMaterial, Vector2Int position, ChunkData data) {
            if (ChunkInitialized) {
                return;
            }

            ChunkInitialized = true;
            chunkPosition = position;
            chunkData = data;
        }

        private bool isMeshGenerated;

        public void GenerateMesh(World world) {
            var firstMat = FindFirstOpaqueBlock();
            if (firstMat == null) {
                Debug.LogWarning("Mesh generation cancelled because no opaque block was found!");
                return;
            }

            if (!isMeshGenerated) {
                isMeshGenerated = true;
            }

            ClearOldMeshes();
            var listOfMaterials = new List<Block>();
            var completedMaterials = new List<Block> {
                firstMat
            };
            GenerateSubMesh(world, firstMat, listOfMaterials);
            Block pendingMaterial;
            while ((pendingMaterial = FindPendingMaterial(listOfMaterials, completedMaterials)) != null) {
                GenerateSubMesh(world, pendingMaterial, listOfMaterials);
                completedMaterials.Add(pendingMaterial);
            }
        }

        private void ClearOldMeshes() {
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

        private Block FindPendingMaterial(List<Block> listOfMaterials, List<Block> completedMaterials) {
            foreach (var candidate in listOfMaterials) {
                if (completedMaterials.Contains(candidate)) {
                    continue;
                }

                return candidate;
            }

            return null;
        }

        private Block FindFirstOpaqueBlock() {
            var chunkSize = chunkData.ChunkSize;
            var chunkHeight = chunkData.ChunkHeight;
            for (byte x = 0; x < chunkSize; x++) {
                for (byte y = 0; y < chunkHeight; y++) {
                    for (byte z = 0; z < chunkSize; z++) {
                        var block = chunkData[x, y, z];
                        if (block.Opaque) {
                            return block;
                        }
                    }
                }
            }

            return null;
        }

        private void GenerateSubMesh(World world, Block b, List<Block> listOfMaterials) {
            if (b == null) {
                Debug.LogWarning("Found no block for material " + b);
                return;
            }

            Debug.LogWarning("Generating mesh for block @ " + b);
            var chunkSize = world.ChunkSize;
            var chunkHeight = world.ChunkHeight;
            var builder = new MeshBuilder();
            var chunkGO = new GameObject($"Chunk {chunkPosition} - SubMesh ({b})");
            var t = chunkGO.transform;
            t.parent = transform;
            t.localPosition = Vector3.zero;
            var filter = chunkGO.AddComponent<MeshFilter>();
            var col = chunkGO.AddComponent<MeshCollider>();
            var ren = chunkGO.AddComponent<MeshRenderer>();

            ren.material = b.VisualMaterial;
            for (byte x = 0; x < chunkSize; x++) {
                for (byte y = 0; y < chunkHeight; y++) {
                    for (byte z = 0; z < chunkSize; z++) {
                        var currentBlockPos = new Vector3Int(x + chunkPosition.x * world.ChunkSize, y, z + chunkPosition.y * world.ChunkSize);
                        var currentBlock = world.GetBlock(currentBlockPos, true);

                        if (currentBlock == null) {
                            Debug.LogError($"Block @ {currentBlockPos} is null!");
                            continue;
                        }

                        if (!currentBlock.Opaque) {
                            continue;
                        }


                        if (currentBlock != b) {
                            if (!listOfMaterials.Contains(currentBlock)) {
                                listOfMaterials.Add(currentBlock);
                            }

                            continue;
                        }

                        foreach (var face in BlockFaces.Faces) {
                            var dir = face.ToDirection();
                            var worldPos = currentBlockPos + dir;
                            var neightboor = world.GetBlock(worldPos, false);
                            if (neightboor == null || neightboor.Opaque) {
                                continue;
                            }


                            builder.AddFace(x, y, z, face);
                        }
                    }
                }
            }

            var mesh = builder.Build();
            filter.sharedMesh = mesh;
            col.sharedMesh = mesh;
        }
    }

    public class ChunkNotInitializedException : Exception { }
}