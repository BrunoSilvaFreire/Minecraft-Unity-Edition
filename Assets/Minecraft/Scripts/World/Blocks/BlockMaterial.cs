using System;
using UnityEngine;

namespace Minecraft.Scripts.World.Blocks {
    [Serializable]
    public struct BlockMaterial {
        private const byte DefaultVariation = 0;
        public static readonly BlockMaterial Unknown = new BlockMaterial(0);


        [SerializeField]
        private readonly ushort id;

        [SerializeField]
        private readonly byte variation;

        public BlockMaterial(ushort id, byte variation = DefaultVariation) {
            this.id = id;
            this.variation = variation;
        }

        public ushort ID => id;

        public byte Variation => variation;


        public bool Equals(BlockMaterial other) {
            return id == other.id && variation == other.variation;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is BlockMaterial && Equals((BlockMaterial) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (id.GetHashCode() * 397) ^ variation.GetHashCode();
            }
        }

        public static bool operator ==(BlockMaterial a, BlockMaterial b) {
            return a.id == b.id && a.variation == b.variation;
        }

        public static bool operator !=(BlockMaterial a, BlockMaterial b) {
            return !(a == b);
        }

        public override string ToString() {
            return $"{id}:{variation}";
        }
    }
}