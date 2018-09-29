using System;
using UnityEngine;
using UnityUtilities;
using Random = UnityEngine.Random;

namespace Minecraft.Scripts.Items.Misc {
    [Serializable]
    public class ItemDrop {
        public byte MinAmount, MaxAmount;
        public Item Item;

        public void Drop(ItemEntity prefab, Vector3 position) {
            var total = Random.Range(MinAmount, MaxAmount);
            for (var i = 0; i < total; i++) {
                prefab.Clone(position).DropAsItem(Item);
            }
        }
    }
}