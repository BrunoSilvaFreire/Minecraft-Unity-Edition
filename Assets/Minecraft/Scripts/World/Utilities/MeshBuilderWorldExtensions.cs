using Minecraft.Scripts.Utility;
using Minecraft.Scripts.World.Blocks;
using UnityEngine;

namespace Minecraft.Scripts.World.Utilities {
    public static class MeshBuilderWorldExtensions {
        public static void AddFace(this MeshBuilder builder, int x, int y, int z, BlockFace face) {
            var pos = new Vector3(x, y, z);
            builder.AddFace(
                pos + face.GetMeshCorner(),
                face.GetMeshUp(),
                face.GetMeshRight(),
                face.AreIndicesReversed()
            );
        }
    }
}