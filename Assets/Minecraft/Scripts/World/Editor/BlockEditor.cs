using Minecraft.Scripts.World.Blocks;
using UnityEditor;

namespace Minecraft.Scripts.World.Editor {
    [CustomEditor(typeof(Block))]
    public class BlockEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            target.name = EditorGUILayout.TextField("Name", target.name);
            DrawDefaultInspector();
        }
    }
}