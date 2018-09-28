using UnityEngine;

namespace Minecraft.Scripts.Utility {
    public static class BlockUtility {
        public static int ExtractBlockX(this Vector3 position) {
            return ExtractBlockPosition(position.x);
        }
        public static int ExtractBlockZ(this Vector3 position) {
            return ExtractBlockPosition(position.z);
        }
        public static int ExtractBlockPosition(float value) {
            var r = (int) value;
            if (value < 0) {
                r--;
            }

            return r;
        }
    }
}