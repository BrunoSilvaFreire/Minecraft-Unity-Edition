using Minecraft.Scripts.Items.Misc;
using UnityEngine;
using UnityUtilities;

namespace Minecraft.Scripts.Entities.Items.Misc {
    public static class ItemDropsExtensions {
        public static void Drop(this ItemDrop drop, ItemEntity prefab, Vector3 position) {
            var total = Random.Range(drop.MinAmount, drop.MaxAmount);
            for (var i = 0; i < total; i++) {
                prefab.Clone(position).DropAsItem(drop.Item);
            }
        }
    }
}