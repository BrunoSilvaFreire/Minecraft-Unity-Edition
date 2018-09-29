using System;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using UnityEditor;
using UnityEngine;

namespace Minecraft.Scripts.World.Editor {
    public abstract class Config<T> {
        private protected string name;

        public Config(string name) {
            this.name = name;
        }

        public abstract T Value {
            get;
            set;
        }

        public static implicit operator T(Config<T> config) {
            return config.Value;
        }
    }

    public class BoolConfig : Config<bool> {
        public BoolConfig(string name) : base(name) { }

        public override bool Value {
            get => EditorPrefs.GetBool(name);
            set => EditorPrefs.SetBool(name, value);
        }
    }

    public static class Configs {
        public static readonly BoolConfig ShowBlocks = new BoolConfig("chunks.showBlocks");
        public static readonly BoolConfig ShowLabels = new BoolConfig("chunks.showLabels");
        public static readonly BoolConfig ShowAsWire = new BoolConfig("chunks.showAsWire");

        public static void DrawConfigs() {
            ShowBlocks.Value = EditorGUILayout.Toggle("Show Blocks", ShowBlocks);
            ShowLabels.Value = EditorGUILayout.Toggle("Show Labels", ShowLabels);
            ShowAsWire.Value = EditorGUILayout.Toggle("Show Blocks As Wire", ShowAsWire);
        }
    }

    public class ChunkDebugData : AbstractChunkData<BlockDebugData> {
        public ChunkDebugData(Chunk chunk) : base(chunk.ChunkData.ChunkSize, chunk.ChunkData.ChunkHeight) {
            for (byte x = 0; x < ChunkSize; x++) {
                for (byte y = 0; y < ChunkHeight; y++) {
                    for (byte z = 0; z < ChunkSize; z++) {
                        this[x, y, z] = new BlockDebugData(chunk, x, y, z);
                    }
                }
            }
        }
    }

    public class BlockDebugData {
        public bool IsOccluded;

        public BlockDebugData(Chunk debugData, byte x, byte y, byte z) {
            var data = debugData.ChunkData;
            var currentBlock = data[x, y, z];

            if (currentBlock == null) {
                IsOccluded = true;
                return;
            }

            foreach (var face in BlockFaces.Faces) {
                var dir = face.ToDirection();
                var fX = x + dir.x;
                var fY = y + dir.y;
                var fZ = z + dir.z;
                Block neighbor;
                if (data.TryGet(fX, fY, fZ, out neighbor)) {
                    if (neighbor == null || neighbor.Opaque) {
                        continue;
                    }

                    IsOccluded = true;
                }
            }
        }
    }

    [CustomEditor(typeof(Chunk))]
    public class ChunkEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            Configs.DrawConfigs();
            var chunk = target as Chunk;
            if (chunk == null) {
                return;
            }
            using (var scope = new EditorGUI.DisabledScope(true)) {
                EditorGUILayout.Toggle("Mesh Generated", chunk.IsMeshGenerated);
            }
        }

        private ChunkDebugData data;

        private void OnEnable() {
            data = new ChunkDebugData(target as Chunk);
        }

        private void OnSceneGUI() {
            var chunk = target as Chunk;
            if (chunk == null) {
                return;
            }

            var cData = chunk.ChunkData;
            var worldSize = cData.ChunkSize;
            var worldHeight = cData.ChunkHeight;
            var chunkPosition = chunk.transform.position;
            if (data == null) {
                return;
            }

            for (byte x = 0; x < worldSize; x++) {
                for (byte y = 0; y < worldHeight; y++) {
                    for (byte z = 0; z < worldSize; z++) {
                        var blockPosition = chunkPosition + new Vector3(x + .5F, y + .5F, z + .5F);
                        var block = cData[x, y, z];
                        var debugBlock = data[x, y, z];
                        if (Configs.ShowBlocks) {
                            Handles.color = block.SignatureColor;
                            if (!debugBlock.IsOccluded && !Configs.ShowAsWire) {
                                Handles.CubeHandleCap(0, blockPosition, Quaternion.identity, 1, EventType.Repaint);
                            } else {
                                Handles.DrawWireCube(blockPosition, Vector3.one);
                            }
                        }

                        if (Configs.ShowLabels) {
                            Handles.Label(blockPosition, block.ToString());
                        }
                    }
                }
            }
        }
    }
}