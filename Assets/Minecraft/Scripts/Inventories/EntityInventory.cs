using UnityEngine;

namespace Minecraft.Scripts.Inventories {
    public sealed class EntityInventory : StackInventory {
        public const byte DefaultHeight = 4;
        public override byte Height => DefaultHeight;

        public byte SelectedIndex {
            get => selectedIndex;
            set {
                selectedIndex = value;
                if (selectedIndex > Width) {
                    selectedIndex = (byte) Width;
                }
            }
        }

        [SerializeField]
        private byte selectedIndex;

        
    }
}