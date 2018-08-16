using System.Collections.Generic;
using System.Linq;
using Minecraft.Scripts.World.Chunks;
using Minecraft.Scripts.World.Generation;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Minecraft.Scripts.World {
    public class World : MonoBehaviour {
        public byte SpawnSize;
        public byte ChunkSize = 16;
        public byte ChunkHeight = byte.MaxValue;
        public WorldGenerator Generator;
        private readonly List<Chunk> chunkCache = new List<Chunk>();
        public Material ChunkMaterial;
        public bool RandomizeSeed;
        public int Seed;

        public BlockMaterial GetBlock(int x, byte y, int z, bool loadIfNotPresent = true) {
            var blockX = (byte) Modulus(x, ChunkSize);
            var blockZ = (byte) Modulus(z, ChunkSize);
            var c = GetChunkAt(x, z, loadIfNotPresent);
            if (c == null || y >= ChunkHeight) {
                if (c == null) { }

                return BlockMaterial.Unknown;
            }

            return c.ChunkData[blockX, y, blockZ];
        }

        private Chunk GetChunk(int chunkX, int chunkY, bool loadIfNotPresent = true) {
            return GetChunk(new Vector2Int(chunkX, chunkY), loadIfNotPresent);
        }

        private Chunk GetChunk(Vector2Int position, bool loadIfNotPresent = true) {
            Chunk cachedEntry = null;
            foreach (var chunk in chunkCache) {
                if (chunk.ChunkPosition == position) {
                    cachedEntry = chunk;
                    break;
                }
            }

            if (cachedEntry != null) {
                return cachedEntry;
            }

            if (loadIfNotPresent) {
                var chunk = LoadChunk(position);
                chunkCache.Add(chunk);
                return chunk;
            }

            return null;
        }

        private Chunk GetChunkAt(int x, int z, bool loadIfNotPresent = true) {
            var chunkX = x / ChunkSize;
            var chunkY = z / ChunkSize;
            return GetChunk(chunkX, chunkY, loadIfNotPresent);
        }

        private Chunk LoadChunk(Vector2Int position) {
            Debug.Log($"Instantiating new chunk @ {position}");
            var obj = new GameObject($"Chunk {position}");
            obj.transform.parent = transform;
            var chunk = obj.AddComponent<Chunk>();
            var data = new ChunkData(ChunkSize, ChunkHeight);
            Generator.Populate(this, ref data, position);
            chunk.Initialize(ChunkMaterial, position, data);
            chunk.transform.position = new Vector3(position.x * ChunkSize, 0, position.y * ChunkSize);
            return chunk;
        }

        private static int Modulus(int x, int m) {
            return (x % m + m) % m;
        }

        public void ClearOldChunks() {
            chunkCache.Clear();
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

        public List<Chunk> LoadedChunks => chunkCache;

        public BlockMaterial GetBlock(Vector3Int pos, bool loadIfNotPresent = true) {
            return GetBlock(pos.x, (byte) pos.y, pos.z, loadIfNotPresent);
        }
    }
}