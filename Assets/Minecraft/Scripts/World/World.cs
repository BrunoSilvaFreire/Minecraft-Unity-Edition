using System;
using System.Collections.Generic;
using System.Linq;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using Minecraft.Scripts.World.Generation;
using Minecraft.Scripts.World.Jobs;
using UnityEngine;
using UnityUtilities.Singletons;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Minecraft.Scripts.World {
    public class World : Singleton<World> {
        public byte SpawnSize;
        public byte ChunkSize = 16;
        public byte ChunkHeight = byte.MaxValue;
        public WorldGenerator Generator;
        public BlockDatabase BlockDatabase;
        public bool RandomizeSeed;
        public int Seed;
        public bool RegenerateOnStart;
        public bool GenerateSingleOnStart;
        public LayerMask WorldMask;

        public Populator Populator;
        public MeshGenerator MeshGenerator;

        public List<Chunk> LoadedChunks {
            get;
        } = new List<Chunk>();

        private void OnEnable() {
            MeshGenerator.Initialize();
            Populator.Initialize(this);
        }

        private void OnDisable() {
            MeshGenerator.TerminateWorkers();
            Populator.TerminateWorkers();
        }

        private void Start() {
            if (RegenerateOnStart) {
                GenerateSpawn();
            }
        }

        public Block GetBlock(int x, byte y, int z, bool loadIfNotPresent = true) {
            var blockX = (byte) Modulus(x, ChunkSize);
            var blockZ = (byte) Modulus(z, ChunkSize);
            var c = GetChunkAt(x, z, loadIfNotPresent);
            if (c == null || y >= ChunkHeight) {
                return null;
            }

            return c.ChunkData[blockX, y, blockZ];
        }

        public Chunk GetChunk(int chunkX, int chunkY, bool loadIfNotPresent = true) {
            return GetChunk(new Vector2Int(chunkX, chunkY), loadIfNotPresent);
        }

        public Chunk GetChunk(Vector2Int position, bool loadIfNotPresent = true) {
            Chunk cachedEntry = null;

            for (var i = 0; i < LoadedChunks.Count; i++) {
                var chunk = LoadedChunks[i];
                if (chunk.ChunkPosition == position) {
                    cachedEntry = chunk;
                    break;
                }
            }

            if (cachedEntry != null) {
                return cachedEntry;
            }

            return loadIfNotPresent ? LoadChunk(position) : null;
        }

        public Vector2Int GetChunkPositionAt(int x, int z) {
            var chunkX = x / ChunkSize;
            var chunkY = z / ChunkSize;
            return new Vector2Int(chunkX, chunkY);
        }

        public Chunk GetChunkAt(int x, int z, bool loadIfNotPresent = true) {
            var chunkX = x / ChunkSize;
            var chunkY = z / ChunkSize;
            if (x < 0) {
                chunkX--;
            }

            if (z < 0) {
                chunkY--;
            }

            return GetChunk(chunkX, chunkY, loadIfNotPresent);
        }

        public static int ToLayer(int bitmask) {
            var result = bitmask > 0 ? 0 : 31;
            while (bitmask > 1) {
                bitmask = bitmask >> 1;
                result++;
            }

            return result;
        }

        private Chunk LoadChunk(Vector2Int position) {
            var obj = new GameObject($"Chunk ({position})");
            obj.transform.parent = transform;
            obj.layer = ToLayer(WorldMask);
            obj.isStatic = true;
            var chunk = obj.AddComponent<Chunk>();
            var data = new ChunkData(ChunkSize, ChunkHeight);
            chunk.Initialize(position, data);
            chunk.GenerateComposition(this);
            chunk.transform.position = new Vector3(position.x * ChunkSize, 0, position.y * ChunkSize);
            LoadedChunks.Add(chunk);
            return chunk;
        }

        private static int Modulus(int x, int m) {
            return (x % m + m) % m;
        }

        public void ClearOldChunks() {
            LoadedChunks.Clear();
            for (var i = 0; i < transform.childCount; i++) {
                var child = transform.GetChild(i).gameObject;
                Debug.Log("Destroying " + child);
#if UNITY_EDITOR
                if (!EditorApplication.isPlaying) {
                    DestroyImmediate(child);
                    continue;
                }
#endif
                Destroy(child);
            }
        }

        public void GenerateSpawn() {
            ClearOldChunks();
            if (RandomizeSeed) {
                Seed = Random.Range(int.MinValue, int.MaxValue);
            }

            if (GenerateSingleOnStart) {
                GetChunk(0, 0).GenerateMesh(this);
                return;
            }

            for (var x = -SpawnSize; x < SpawnSize; x++) {
                for (var y = -SpawnSize; y < SpawnSize; y++) {
                    GetChunk(x, y);
                }
            }

            for (var x = -SpawnSize; x < SpawnSize; x++) {
                for (var y = -SpawnSize; y < SpawnSize; y++) {
                    GetChunk(x, y).GenerateMesh(this);
                }
            }
        }


        public Block GetBlock(Vector3Int pos, bool loadIfNotPresent = true) {
            return GetBlock(pos.x, (byte) pos.y, pos.z, loadIfNotPresent);
        }

        public void SetBlock(Vector3Int position, Block material) {
            var x = position.x;
            var y = (byte) position.y;
            var z = position.z;
            var blockX = (byte) Modulus(x, ChunkSize);
            var blockZ = (byte) Modulus(z, ChunkSize);
            var c = GetChunkAt(x, z, false);
            if (c == null || y >= ChunkHeight) {
                if (c == null) { }

                return;
            }

            c.ChunkData[blockX, y, blockZ] = material;
            c.GenerateMesh(this);
            if (blockX == 0) {
                TryRegenChunk(x - 1, z);
            }

            if (blockZ == 0) {
                TryRegenChunk(x, z - 1);
            }

            if (blockX == ChunkSize - 1) {
                TryRegenChunk(x + 1, z);
            }

            if (blockZ == ChunkSize - 1) {
                TryRegenChunk(x, z + 1);
            }
        }

        private void TryRegenChunk(int x, int y) {
            var c = GetChunkAt(x, y, false);
            if (c == null) {
                return;
            }

            c.GenerateMesh(this);
        }

        public Vector2Int WorldPositionOf(Chunk chunk) {
            var pos = chunk.ChunkPosition;
            pos.x *= ChunkSize;
            pos.y *= ChunkSize;
            return pos;
        }

        public void UnloadChunk(Chunk chunk) {
            LoadedChunks.Remove(chunk);
            Destroy(chunk.gameObject);
        }

        public Chunk GetChunkAt(Vector3 position, bool loadIfNotPresent = true) {
            return GetChunkAt(position.ExtractBlockX(), position.ExtractBlockZ(), loadIfNotPresent);
        }

        public Vector3Int ToLocalChunkPosition(Vector3Int tilePos) {
            var blockX = (byte) Modulus(tilePos.x, ChunkSize);
            var blockZ = (byte) Modulus(tilePos.z, ChunkSize);
            return new Vector3Int(blockX, tilePos.y, blockZ);
        }
    }
}