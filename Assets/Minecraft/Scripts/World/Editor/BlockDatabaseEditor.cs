using Minecraft.Scripts.World.Blocks;
using UnityEditor;
using UnityEngine;
using UnityUtilities.Editor;

namespace Minecraft.Scripts.World.Editor {
    [CustomEditor(typeof(BlockDatabase))]
    public class BlockDatabaseEditor : UnityEditor.Editor {
        private BlockDatabase db;

        private void OnEnable() {
            db = (BlockDatabase) target;
        }

        public override void OnInspectorGUI() {
            if (GUILayout.Button("Create Block")) {
                Selection.activeObject = db.AddToAssetFile<Block>();
            }

            DrawDefaultInspector();
        }
    }
}