using Minecraft.Scripts.Ambience;
using UnityEditor;

namespace Minecraft.Scripts.Editor {
    [CustomEditor(typeof(DayCycleController))]
    public class DayCycleControllerEditor : UnityEditor.Editor {
        private DayCycleController controller;
        private float previewPosition;

        private void OnEnable() {
            controller = (DayCycleController) target;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            if (EditorApplication.isPlaying) {
                controller.CurrentPosition = EditorGUILayout.Slider(controller.CurrentPosition, 0, 1);
            } else {
                previewPosition = EditorGUILayout.Slider(previewPosition, 0, 1);
                controller.UpdateTo(previewPosition);
            }
        }
    }
}