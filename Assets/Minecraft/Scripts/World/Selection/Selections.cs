using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using UnityEngine;

namespace Minecraft.Scripts.World.Selection {
    public struct WorldTile : IComparable<WorldTile> {
        public WorldTile(Vector3Int position, Block material) {
            this.Position = position;
            this.Material = material;
        }

        public Vector3Int Position {
            get;
        }

        public Block Material {
            get;
        }

        public int CompareTo(WorldTile other) {
            var otherPos = other.Position;
            if (Larger(Position, otherPos)) {
                return -1;
            }

            return Larger(otherPos, Position) ? 1 : 0;
        }

        private static bool Larger(Vector3Int a, Vector3Int b) {
            var chunkSize = World.Instance.ChunkSize;
            var chunkXA = a.x / chunkSize;
            var chunkYA = a.z / chunkSize;
            var chunkXB = b.x / chunkSize;
            var chunkYB = b.z / chunkSize;
            return chunkYA > chunkYB || chunkXA > chunkXB;
        }
    }


    public sealed class WorldSelection : IEnumerable<WorldTile> {
        public List<WorldTile> Tiles;
        private World world;
        private Vector3Int min;
        private Vector3Int max;

        public int SolidTilesCount => SolidTiles.Count();


        public int Count {
            get {
                return Tiles.Count;
            }
        }

        public IEnumerable<WorldTile> SolidTiles {
            get {
                foreach (var tile in Tiles) {
                    if (tile.Material.Opaque) {
                        yield return tile;
                    }
                }
            }
        }

        public void SetAllTo(Block material) {
            var total = Tiles.Count;
            if (total <= 0) {
                Debug.LogWarning("Selection is empty!");
                return;
            }

            var i = 0;
            var chunks = new List<Chunk>();
            do {
                var tile = Tiles[i++];
                if (tile.Material == material) {
                    continue;
                }

                var tilePos = tile.Position;
                CheckAdd(tilePos, chunks, Vector2Int.left);
                CheckAdd(tilePos, chunks, Vector2Int.right);
                CheckAdd(tilePos, chunks, Vector2Int.down);
                CheckAdd(tilePos, chunks, Vector2Int.up);

                var chunk = world.GetChunkAt(tilePos);
                if (!chunks.Contains(chunk)) {
                    chunks.Add(chunk);
                }

                var localPos = world.ToLocalChunkPosition(tilePos);
                chunk.ChunkData[(byte) localPos.x, (byte) localPos.y, (byte) localPos.z] = material;
            } while (i < total);

            foreach (var chunk in chunks) {
                chunk.GenerateMesh(World.Instance);
            }
        }

        private void CheckAdd(Vector3Int originalPos, ICollection<Chunk> chunks, Vector2Int direction) {
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


        private WorldSelection(List<WorldTile> tiles, World world, Vector3Int min, Vector3Int max) {
            this.Tiles = tiles;
            this.world = world;
            this.min = min;
            this.max = max;
        }

        public IEnumerator<WorldTile> GetEnumerator() => ((IEnumerable<WorldTile>) Tiles).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static WorldSelection FromEnumerable(World world, IEnumerable<Vector3Int> enumerable) {
            var list = new List<WorldTile>();
            var min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            var max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
            foreach (var pos in enumerable) {

                if (pos.x < min.x) {
                    min.x = pos.x;
                }

                if (pos.y < min.y) {
                    min.y = pos.y;
                }

                if (pos.z < min.z) {
                    min.z = pos.z;
                }

                if (pos.x > max.x) {
                    max.x = pos.x;
                }

                if (pos.y > max.y) {
                    max.y = pos.y;
                }

                if (pos.z > max.z) {
                    max.z = pos.z;
                }

                list.Add(new WorldTile(pos, world.GetBlock(pos)));
            }

            return new WorldSelection(list, world, min, max);
        }

        public void DeleteAll() {
            SetAllTo(world.BlockDatabase.GetBlock(BlockMaterial.Air));
        }
    }

    public static class Selections {
        public static WorldSelection SphereSelection(World world, Vector3Int center, float radius) {
            return WorldSelection.FromEnumerable(world, SelectSphere(center, radius));
        }

        public static IEnumerable<Vector3Int> SelectSphere(Vector3Int center, float radius) {
            var minX = (int) (center.x - radius);
            var minY = (int) (center.y - radius);
            var minZ = (int) (center.z - radius);
            var maxX = (int) (center.x + radius);
            var maxY = (int) (center.y + radius);
            var maxZ = (int) (center.z + radius);
            for (var x = minX; x < maxX; x++) {
                for (var y = minY; y < maxY; y++) {
                    for (var z = minZ; z < maxZ; z++) {
                        if (DistanceSquared(x, y, z, center) < radius) {
                            yield return new Vector3Int(x, y, z);
                        }
                    }
                }
            }
        }

        private static float DistanceSquared(int x, int y, int z, Vector3Int center) {
            var dX = Mathf.Abs(center.x - x);
            var dY = Mathf.Abs(center.y - y);
            var dZ = Mathf.Abs(center.z - z);
            return Mathf.Sqrt(dX * dX + dY * dY + dZ * dZ);
        }

        public static WorldSelection CuboidSelection(World world, Vector3Int min, Vector3Int max) {
            return WorldSelection.FromEnumerable(world, SelectCuboid(min, max));
        }

        private static IEnumerable<Vector3Int> SelectCuboid(Vector3Int min, Vector3Int max) {
            for (var x = min.x; x < max.x; x++) {
                for (var y = min.y; y < max.y; x++) {
                    for (var z = min.z; z < max.z; x++) {
                        yield return new Vector3Int(x, y, z);
                    }
                }
            }
        }
    }
}