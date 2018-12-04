using Minecraft.Scripts.Items;
using UnityEngine;

namespace Minecraft.Scripts.Inventories {
    public abstract class StackInventory : Inventory {
        public const uint Width = 9;
        private ItemStack[] items;

        public abstract byte Height {
            get;
        }

        private void Awake() {
            items = new ItemStack[Height * Width];
        }


        public override ItemStack this[uint index] {
            get => items[index];
            set {
                if (Equals(items[index], value)) {
                    return;
                }
                items[index] = value;
                OnModified.Invoke();
            }
        }

        public override bool TryAddStack(ItemStack stack, out byte left) {
            if (stack == null) {
                left = 0;
                return false;
            }

            left = stack.Amount;
            for (var i = 0; i < items.Length; i++) {
                var itemStack = items[i];
                if (itemStack != null) {
                    if (itemStack.Item == stack.Item) {
                        if (itemStack.TryAdd(left, out var consumed)) {
                            left = 0;
                        } else {
                            left -= consumed;
                        }
                    }
                } else {
                    items[i] = stack;
                    left = 0;
                    break;
                }

                if (left <= 0) {
                    break;
                }
            }

            OnModified.Invoke();
            return left <= 0;
        }
    }
}