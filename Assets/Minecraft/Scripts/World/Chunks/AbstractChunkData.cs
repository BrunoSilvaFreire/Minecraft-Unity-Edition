using System;
using UnityEngine;

namespace Minecraft.Scripts.World.Chunks {
    [Serializable]
    public abstract class AbstractChunkData<T> {

        [SerializeField]
        protected T[] data;

        [SerializeField]
        private byte chunkSize, chunkHeight;

        public AbstractChunkData(byte chunkSize, byte chunkHeight) {
            data = new T[chunkSize * chunkHeight * chunkSize];
            this.chunkSize = chunkSize;
            this.chunkHeight = chunkHeight;
        }

        public T[] Data {
            get => data;
            set => data = value;
        }

        public T this[byte x, byte y, byte z] {
            get => data[IndexOf(x, y, z)];
            set => data[IndexOf(x, y, z)] = value;
        }

        

        public T this[uint index] {
            get => data[index];
            set => data[index] = value;
        }

        public byte ChunkSize => chunkSize;

        public byte ChunkHeight => chunkHeight;

        public int IndexOf(byte x, byte y, byte z) {
            //x + WIDTH * (y + DEPTH * z)
            return x + chunkSize * (y + chunkHeight * z);
        }
    }
}