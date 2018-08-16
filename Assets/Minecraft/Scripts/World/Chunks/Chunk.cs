using System;
using Minecraft.Scripts.Utility;
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

        private MeshFilter filter;
        private MeshCollider collider;
        private MeshRenderer renderer;

        public void Initialize(Material chunkMaterial, Vector2Int position, ChunkData data) {
            if (ChunkInitialized) {
                return;
            }

            ChunkInitialized = true;
            chunkPosition = position;
            chunkData = data;
            filter = gameObject.AddComponent<MeshFilter>();
            collider = gameObject.AddComponent<MeshCollider>();
            renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.material = chunkMaterial;
        }

        public void GenerateMesh(World world) {
            var chunkSize = world.ChunkSize;
            var chunkHeight = world.ChunkHeight;
            var builder = new MeshBuilder();
            for (byte x = 0; x < chunkSize; x++) {
                for (byte y = 0; y < chunkHeight; y++) {
                    for (byte z = 0; z < chunkSize; z++) {
                        var currentBlockPos = new Vector3Int(x + chunkPosition.x * world.ChunkSize, y, z + chunkPosition.y * world.ChunkSize);
                        var currentBlock = world.GetBlock(currentBlockPos, false);
                        if (!Blocks.IsOpaque(currentBlock)) {
                            continue;
                        }


                        foreach (var face in BlockFaces.Faces) {
                            var dir = face.ToDirection();
                            var worldPos = currentBlockPos + dir;
                            var block = world.GetBlock(worldPos, false);
                            if (Blocks.IsOpaque(block)) {
                                continue;
                            }


                            builder.AddFace(x, y, z, face);
                        }
                    }
                }
            }

            var mesh = builder.Build();
            filter.sharedMesh = mesh;
            collider.sharedMesh = mesh;
        }
    }

    public class ChunkNotInitializedException : Exception { }
}