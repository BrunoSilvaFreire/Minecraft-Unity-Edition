using UnityEngine;

namespace Minecraft.Scripts.World.Utilities {
    public static class BlockUtility {
        public static Vector3 ToBlockCentralPosition(this Vector3Int pos) {
            return pos + new Vector3(.5F * Mathf.Sign(pos.x), .5F * Mathf.Sign(pos.y), .5F * Mathf.Sign(pos.z));
        }

        public static Vector3 AddHalf(this Vector3Int pos) {
            return pos + new Vector3(.5F, .5F, .5F);
        }
    }
}