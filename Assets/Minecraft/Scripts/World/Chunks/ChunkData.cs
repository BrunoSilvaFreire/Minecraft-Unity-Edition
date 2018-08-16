using System;
using UnityEngine;

namespace Minecraft.Scripts.World.Chunks {
    [Serializable]
    public class ChunkData {
        [SerializeField]
        private BlockMaterial[] data;

        [SerializeField]
        private byte chunkSize, chunkHeight;

        public ChunkData(byte chunkSize, byte chunkHeight) {
            data = new BlockMaterial[chunkSize * chunkHeight * chunkSize];
            this.chunkSize = chunkSize;
            this.chunkHeight = chunkHeight;
        }

        public BlockMaterial[] Data {
            get {
                return data;
            }
            set {
                data = value;
            }
        }

        public BlockMaterial this[byte x, byte y, byte z] {
            get {
                return data[IndexOf(x, y, z)];
            }
            set {
                data[IndexOf(x, y, z)] = value;
            }
        }

        public BlockMaterial this[uint index] {
            get {
                return data[index];
            }
            set {
                data[index] = value;
            }
        }


        public int IndexOf(byte x, byte y, byte z) {
            //x + WIDTH * (y + DEPTH * z)
            return x + chunkSize * (y + chunkHeight * z);
        }
    }
}