using System;
using UnityEngine;

namespace Minecraft.Scripts.World {
    public enum BlockFace {
        Forward,
        Backward,
        Up,
        Down,
        Left,
        Right
    }

    public static class BlockFaces {
        public const byte TotalFaces = 6;

        public static readonly BlockFace[] Faces = {
            BlockFace.Forward,
            BlockFace.Backward,
            BlockFace.Up,
            BlockFace.Down,
            BlockFace.Left,
            BlockFace.Right
        };

        public static Vector3Int ToDirection(this BlockFace face) {
            switch (face) {
                case BlockFace.Forward:
                    return new Vector3Int(0, 0, 1);
                case BlockFace.Backward:
                    return new Vector3Int(0, 0, -1);
                case BlockFace.Up:
                    return new Vector3Int(0, 1, 0);
                case BlockFace.Down:
                    return new Vector3Int(0, -1, 0);
                case BlockFace.Left:
                    return new Vector3Int(-1, 0, 0);
                case BlockFace.Right:
                    return new Vector3Int(1, 0, 0);
                default:
                    throw new ArgumentOutOfRangeException("face", face, null);
            }
        }

        public static bool AreIndicesReversed(this BlockFace face) {
            switch (face) {
                case BlockFace.Backward:
                case BlockFace.Up:
                case BlockFace.Right:
                    return true;
                default:
                    return false;
            }
        }
        public static Vector3 GetMeshRight(this BlockFace face) {
            switch (face) {
                case BlockFace.Left:
                case BlockFace.Right:
                    return Vector3.forward;
                default:
                    return Vector3.right;
            }
        }

        public static Vector3 GetMeshUp(this BlockFace face) {
            switch (face) {
                case BlockFace.Up:
                case BlockFace.Down:
                    return Vector3.forward;
                default:
                    return Vector3.up;
            }
        }

        public static Vector3 GetMeshCorner(this BlockFace face) {
            switch (face) {
                case BlockFace.Forward:
                    return Vector3.forward;
                case BlockFace.Up:
                    return Vector3.up;
                case BlockFace.Right:
                    return Vector3.right;
                default:
                    return Vector3.zero;
            }
        }
    }
}