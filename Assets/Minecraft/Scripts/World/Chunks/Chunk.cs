using System;
using System.Collections.Generic;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.Utility.Multithreading;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Jobs;
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

    public partial class Chunk : MonoBehaviour {
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