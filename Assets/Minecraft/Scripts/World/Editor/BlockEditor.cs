using Minecraft.Scripts.World.Blocks;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace Minecraft.Scripts.World.Editor {
    [CustomEditor(typeof(Block))]
    public class BlockEditor : OdinEditor {
        public override void OnInspectorGUI() {
            target.name = EditorGUILayout.TextField("Name", target.name);
            DrawTree();
        }
    }
}