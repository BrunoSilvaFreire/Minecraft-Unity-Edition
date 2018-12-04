using UnityEngine;

namespace Minecraft.Scripts.Items {
    [CreateAssetMenu(menuName = "Minecraft/Items/Item")]
    public class Item : ScriptableObject {
        public Sprite Sprite;
        public Color SignatureColor = Color.cyan;
        public ItemCategory Category;
        public ItemRarity Rarity;
        public byte StackSize;
    }
}