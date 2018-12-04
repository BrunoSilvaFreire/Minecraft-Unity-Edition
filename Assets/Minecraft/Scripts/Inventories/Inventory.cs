using Minecraft.Scripts.Items;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Minecraft.Scripts.Inventories {
    public abstract class Inventory : MonoBehaviour {
        public UnityEvent OnModified;

        public abstract ItemStack this[uint index] {
            get;
            set;
        }

        public abstract bool TryAddStack(ItemStack stack, out byte left);
    }
}