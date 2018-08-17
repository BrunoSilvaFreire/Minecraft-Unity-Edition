using System;
using System.Collections.Generic;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.World.Blocks;
using UnityEngine;
using Minecraft.Scripts.World.Utilities;

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

        public void Initialize(Material chunkMaterial, Vector2Int position, ChunkData data) {
            if (ChunkInitialized) {
                return;
            }

            ChunkInitialized = true;
            chunkPosition = position;
            chunkData = data;
        }

        public void GenerateMesh(World world) {
            var firstMat = FindFirstOpaqueBlock();
            if (firstMat == null) {
                return;
            }

            ClearOldMeshes();
            var listOfMaterials = new List<BlockMaterial>();
            var completedMaterials = new List<BlockMaterial> {
                firstMat.Material
            };
            GenerateSubMesh(world, firstMat.Material, listOfMaterials);
            BlockMaterial pendingMaterial;
            while ((pendingMaterial = FindPendingMaterial(listOfMaterials, completedMaterials)) != BlockMaterial.Unknown) {
                GenerateSubMesh(world, pendingMaterial, listOfMaterials);
                completedMaterials.Add(pendingMaterial);
            }
        }

        private void ClearOldMeshes() {
            for (int i = 0; i < transform.childCount; i++) {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private BlockMaterial FindPendingMaterial(List<BlockMaterial> listOfMaterials, List<BlockMaterial> completedMaterials) {
            foreach (var candidate in listOfMaterials) {
                if (completedMaterials.Contains(candidate)) {
                    continue;
                }

                return candidate;
            }

            return BlockMaterial.Unknown;
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

        private void GenerateSubMesh(World world, BlockMaterial blockMaterial, List<BlockMaterial> listOfMaterials) {
            var b = world.BlockDatabase.GetBlock(blockMaterial);
            if (b == null) {
                Debug.LogWarning("Found no block for material " + blockMaterial);
                return;
            }

            var chunkSize = world.ChunkSize;
            var chunkHeight = world.ChunkHeight;
            var builder = new MeshBuilder();
            var chunkGO = new GameObject($"Chunk {chunkPosition} - SubMesh ({blockMaterial})");
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
                        var currentBlock = world.GetBlock(currentBlockPos, false);
                        var mat = currentBlock.Material;

                        if (!currentBlock.Opaque) {
                            continue;
                        }

                        if (mat != blockMaterial) {
                            if (!listOfMaterials.Contains(mat)) {
                                listOfMaterials.Add(mat);
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