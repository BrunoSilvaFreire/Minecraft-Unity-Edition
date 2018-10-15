using System;
using System.Collections.Generic;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using UnityEngine;

namespace Minecraft.Scripts.World.Structures {
    [Serializable]
    public class Structure {
        [SerializeField]
        private Block[] tiles;

        [SerializeField]
        private Vector3Int size;

        public Structure(Block[] tiles, Vector3Int size) {
            this.tiles = tiles;
            this.size = size;
        }

        private static void CheckAdd(World world, Vector3Int originalPos, ICollection<Chunk> chunks,
            Vector2Int direction) {
            var tilePos = originalPos;
            tilePos.x += direction.x;
            tilePos.z += direction.y;

            var chunk = world.GetChunkAt(tilePos, false);
            if (chunk == null) {
                return;
            }

            if (!chunks.Contains(chunk)) {
                chunks.Add(chunk);
            }
        }

        public void PlaceOnto(World world, Vector3Int origin) {
            PlaceOnto(world, origin, size, tiles);
        }

        public static void PlaceOnto(World world, Vector3Int origin, Vector3Int size, Block[] tiles,
            bool loadChunksIfNotPresent = true, bool updateChunks = true) {
            var chunks = new List<Chunk>();
            for (var x = 0; x < size.x; x++) {
                for (var z = 0; z < size.z; z++) {
                    var chunk = world.GetChunkAt(x, z, loadChunksIfNotPresent);
                    if (chunk == null) {
                        continue;
                    }

                    for (var y = 0; y < size.y; y++) {
                        var index = IndexingUtility.IndexOf(x, y, z, size.x, size.y);
                        var localTile = tiles[index];
                        if (localTile == null) {
                            continue;
                        }

                        var chunkData = chunk.ChunkData;
                        var tilePos = origin;
                        tilePos.x += x;
                        tilePos.y += y;
                        tilePos.z += z;
                        var localPos = world.ToLocalChunkPosition(tilePos);
                        byte chunkBlockX = (byte) localPos.x,
                            chunkBlockY = (byte) localPos.y,
                            chunkBlockZ = (byte) localPos.z;
                        var presentTile = chunkData[chunkBlockX, chunkBlockY, chunkBlockZ];
                        if (localTile == presentTile) {
                            continue;
                        }


                        CheckAdd(world, tilePos, chunks, Vector2Int.left);
                        CheckAdd(world, tilePos, chunks, Vector2Int.right);
                        CheckAdd(world, tilePos, chunks, Vector2Int.down);
                        CheckAdd(world, tilePos, chunks, Vector2Int.up);

                        if (!chunks.Contains(chunk)) {
                            chunks.Add(chunk);
                        }


                        chunkData[chunkBlockX, chunkBlockY, chunkBlockZ] = localTile;
                    }
                }
            }

            if (!updateChunks) {
                return;
            }

            foreach (var chunk in chunks) {
                chunk.GenerateMesh(World.Instance);
            }
        }

        public static void PlaceOntoSilently(Chunk chunk, Vector3Int origin, Vector3Int size, Block[] tiles) {
            var chunks = new List<Chunk>();
            for (var x = 0; x < size.x; x++) {
                for (var z = 0; z < size.z; z++) {
                    for (var y = 0; y < size.y; y++) {
                        var index = IndexingUtility.IndexOf(x, y, z, size.x, size.y);
                        var localTile = tiles[index];
                        if (localTile == null) {
                            continue;
                        }

                        var chunkData = chunk.ChunkData;
                        var tilePos = origin;
                        tilePos.x += x;
                        tilePos.y += y;
                        tilePos.z += z;
                        var localPos = origin;
                        byte chunkBlockX = (byte) localPos.x,
                            chunkBlockY = (byte) localPos.y,
                            chunkBlockZ = (byte) localPos.z;
                        var presentTile = chunkData[chunkBlockX, chunkBlockY, chunkBlockZ];
                        if (localTile == presentTile) {
                            continue;
                        }

                        if (!chunks.Contains(chunk)) {
                            chunks.Add(chunk);
                        }


                        chunkData[chunkBlockX, chunkBlockY, chunkBlockZ] = localTile;
                    }
                }
            }
        }
    }
}