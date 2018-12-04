using System;

namespace Minecraft.Scripts.Items {
    [Serializable]
    public class ItemStack {
        public Item Item;
        public byte Amount;

        public ItemStack(Item item, byte amount = 1) {
            Item = item;
            Amount = amount;
        }

        public bool TryAdd(byte amount, out byte totalConsumed) {
            var allowedToTake = (byte) (Item.StackSize - Amount);

            if (allowedToTake > amount) {
                Amount += amount;
                totalConsumed = amount;
                return true;
            }

            Amount += allowedToTake;
            totalConsumed = allowedToTake;
            return false;
        }

        protected bool Equals(ItemStack other) {
            return Equals(Item, other.Item) && Amount == other.Amount;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((ItemStack) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Item != null ? Item.GetHashCode() : 0) * 397) ^ Amount.GetHashCode();
            }
        }
    }
}