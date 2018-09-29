using UnityEngine;

namespace Minecraft.Scripts.Items {
    [CreateAssetMenu(menuName = "Minecraft/Items/ItemCategory")]
    public class ItemCategory : ScriptableObject {
        public Color Color;
        public string Alias;
    }
}