using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.World.Blocks;
using UnityEngine;

namespace Minecraft.Scripts.World.Chunks {
    [Serializable]
    public sealed class ChunkData {
        [SerializeField]
        private List<BlockMaterial> tableOfContent;

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
            get => data;
            set => data = value;
        }

        public IEnumerable<BlockMaterial> TableOfContent => tableOfContent;

        public Block this[byte x, byte y, byte z] {
            get => data[IndexOf(x, y, z)];
            set {
                CheckAddToTOC(value.Material);
                data[IndexOf(x, y, z)] = value;
            }
        }

        private void CheckAddToTOC(BlockMaterial material) {
            if (tableOfContent == null) {
                tableOfContent = new List<BlockMaterial>();
            }

            if (!tableOfContent.Contains(material)) {
                tableOfContent.Add(material);
            }
        }


        public Block this[uint index] {
            get => data[index];
            set {
                CheckAddToTOC(value.Material);
                data[index] = value;
            }
        }

        public byte ChunkSize => chunkSize;

        public byte ChunkHeight => chunkHeight;

        public int IndexOf(byte x, byte y, byte z) {
            return IndexingUtility.IndexOf(x, y, z, chunkSize, chunkHeight);
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

        public void PrintData() {
            var buffer = new StringBuilder();
            foreach (var block in data) {
                buffer.Append(block == null ? "null" : block.Material + ", ");
            }

            Debug.Log(buffer.ToString());
        }
    }
}