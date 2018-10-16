using Minecraft.Scripts.Items;
using UnityEngine;

namespace Minecraft.Scripts.Entities.Items {
    public partial class ItemEntity : Entity {
        public SpriteRenderer SpriteRenderer;
        public Rigidbody Rigidbody;
        public float DropForce = 2;

        public void LoadData(Item item) {
            SpriteRenderer.sprite = item.Sprite;
        }

        private void Update() {
            UpdateVanity();
        }


        public void DropAsItem(Item item) {
            LoadData(item);
            var dir = new Vector3(Random.value, Random.value, Random.value);
            dir.Normalize();
            dir *= DropForce;
            Rigidbody.velocity = dir;
        }
    }
}