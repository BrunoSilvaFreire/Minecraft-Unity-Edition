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
            //x + WIDTH * (y + DEPTH * z)
            return x + chunkSize * (y + chunkHeight * z);
        }
    }
}