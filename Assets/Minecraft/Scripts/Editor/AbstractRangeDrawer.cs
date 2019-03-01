using Minecraft.Scripts.Utility;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityUtilities;

namespace Minecraft.Scripts.Editor {
    public abstract class AbstractRangeDrawer<T> : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var startProp = property.FindPropertyRelative(nameof(AbstractRange<T>.Start));
            var endProp = property.FindPropertyRelative(nameof(AbstractRange<T>.End));
            float min = GetMin(startProp), max = GetMax(endProp);
            EditorGUI.MinMaxSlider(position.GetLine(0), label, ref min, ref max, GetMinLimit(), GetMaxLimit());
            SetMin(startProp, min);
            SetMax(endProp, max);
            var range2 = position.AddXMin(16);
            EditorGUI.PropertyField(range2.GetLine(1), startProp);
            EditorGUI.PropertyField(range2.GetLine(2), endProp);
        }

        protected abstract void SetMax(SerializedProperty endProp, float max);

        protected abstract void SetMin(SerializedProperty startProp, float min);

        protected abstract float GetMax(SerializedProperty property);

        protected abstract float GetMin(SerializedProperty property);
        protected abstract float GetMinLimit();
        protected abstract float GetMaxLimit();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 3 * RectUtility.DefaultLineHeight;
        }
    }
}