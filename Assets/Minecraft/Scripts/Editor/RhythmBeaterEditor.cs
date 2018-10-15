using Minecraft.Scripts.Utility;
using UnityEditor;

namespace Minecraft.Scripts.Editor {
    [CustomEditor(typeof(RhythmBeater))]
    public sealed class RhythmBeaterEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            EditorGUILayout.Slider(((RhythmBeater) target).Evaluate(), 0, 1);
            Repaint();
        }
    }
}