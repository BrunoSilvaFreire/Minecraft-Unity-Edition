using Minecraft.Scripts.Utility;
using UnityEditor;

namespace Minecraft.Scripts.Editor {
    [CustomPropertyDrawer(typeof(UInt8Range))]
    public class UInt8RangeDrawer : AbstractRangeDrawer<byte> {
        protected override void SetMax(SerializedProperty endProp, float max) {
            endProp.intValue = (int) max;
        }

        protected override void SetMin(SerializedProperty startProp, float min) {
            startProp.intValue = (int) min;
        }

        protected override float GetMax(SerializedProperty property) {
            return property.intValue;
        }

        protected override float GetMin(SerializedProperty property) {
            return property.intValue;
        }

        protected override float GetMinLimit() {
            return byte.MinValue;
        }

        protected override float GetMaxLimit() {
            return byte.MaxValue;
        }
    }
}