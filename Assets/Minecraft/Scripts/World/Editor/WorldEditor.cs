using Minecraft.Scripts.World.Chunks;
using UnityEditor;
using UnityEngine;

namespace Minecraft.Scripts.World.Editor {
    [CustomEditor(typeof(World))]
    public class WorldEditor : UnityEditor.Editor {
        private World world;

        private void OnEnable() {
            world = (World) target;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            foreach (var chunk in world.LoadedChunks) {
                EditorGUILayout.ObjectField($"Chunk @ {chunk.ChunkPosition}", chunk, typeof(Chunk), false);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate")) {
                world.GenerateSpawn();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}