using Minecraft.Scripts.UI;
using UnityEditor;

namespace Minecraft.Scripts.Editor {
    [CustomEditor(typeof(UIView), true)]
    public class UIViewEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var v = target as UIView;
            if (v != null) {
                v.Revealed = EditorGUILayout.Toggle("Revealed", v.Revealed);
            }
            DrawDefaultInspector();
        }
    }
}