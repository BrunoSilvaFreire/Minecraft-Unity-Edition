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
            using (new EditorGUILayout.HorizontalScope()) {
                var found = EditorGUILayout.ObjectField("Install block", null, typeof(Block), false);
                if (found != null) {
                    Block instance;
                    var mainAsset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(found));
                    if (mainAsset != db) {
                        instance = (Block) Instantiate(found);
                        AssetDatabase.AddObjectToAsset(instance, db);
                        DestroyImmediate(found, true);
                    } else {
                        instance = (Block) found;
                    }

                    db.Blocks.Add(instance);
                    EditorUtility.SetDirty(db);
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Create Block")) {
                    UnityEditor.Selection.activeObject = db.AddToAssetFile<Block>();
                }
            }

            DrawDefaultInspector();
        }
    }
}