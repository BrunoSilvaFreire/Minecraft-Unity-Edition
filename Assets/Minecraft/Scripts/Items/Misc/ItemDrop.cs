using System;

namespace Minecraft.Scripts.Items.Misc {
    [Serializable]
    public class ItemDrop {
        public byte MinAmount, MaxAmount;
        public Item Item;
    }
}