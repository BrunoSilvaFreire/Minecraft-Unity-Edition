using Minecraft.Scripts.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Minecraft.Scripts.Master.UI.Inventories {
    public class ItemView : MonoBehaviour {
        public ItemStack CurrentItem {
            get;
            private set;
        }

        public Image Image;

        public void Import(ItemStack item) {
            if (item == null) {
                Image.enabled = false;
            } else {
                Image.enabled = true;
                Image.sprite = item.Item.Sprite;
            }

            CurrentItem = item;
        }
    }
}