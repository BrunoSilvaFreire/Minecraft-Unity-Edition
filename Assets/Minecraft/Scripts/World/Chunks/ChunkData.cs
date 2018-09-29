using System;
using Minecraft.Scripts.World.Blocks;
using UnityEngine;

namespace Minecraft.Scripts.World.Chunks {
    [Serializable]
    public class ChunkData {
        [SerializeField]
        private Block[] data;

        [SerializeField]
        private byte chunkSize, chunkHeight;

        public ChunkData(byte chunkSize, byte chunkHeight) {
            data = new Block[chunkSize * chunkHeight * chunkSize];
            this.chunkSize = chunkSize;
            this.chunkHeight = chunkHeight;
        }

        public Block[] Data {
            get {
                return data;
            }
            set {
                data = value;
            }
        }

        public Block this[byte x, byte y, byte z] {
            get {
                return data[IndexOf(x, y, z)];
            }
            set {
                data[IndexOf(x, y, z)] = value;
            }
        }


        public Block this[uint index] {
            get {
                return data[index];
            }
            set {
                data[index] = value;
            }
        }

        public byte ChunkSize => chunkSize;

        public byte ChunkHeight => chunkHeight;

        public int IndexOf(byte x, byte y, byte z) {
            return IndexOf(x, y, z, chunkSize, chunkHeight);
        }

        public static int IndexOf(int x, int y, int z, int width, int height) {
            return x + width * (y + height * z);
        }

        public static int IndexOf(byte x, byte y, byte z, byte width, byte height) {
            return x + width * (y + height * z);
        }


        public bool TryGet(int x, int y, int z, out Block neighbor) {
            if (!IsWithinBounds(x, y, z)) {
                neighbor = null;
                return false;
            }

            neighbor = this[(byte) x, (byte) y, (byte) z];
            return true;
        }

        private bool IsWithinBounds(int x, int y, int z) {
            return x >= 0 && y >= 0 && z >= 0 && x < chunkSize && y < chunkHeight && z < chunkSize;
        }
    }
}