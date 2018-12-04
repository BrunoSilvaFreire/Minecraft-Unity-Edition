using Minecraft.Scripts.Entities.Living;
using Minecraft.Scripts.Items;
using UnityEngine;

namespace Minecraft.Scripts.Entities.Items {
    public partial class ItemWisp : MonoBehaviour {
        public SpriteRenderer SpriteRenderer;
        public Rigidbody Rigidbody;
        public float DropForce = 2;
        private ItemStack stack;
        public float RepickCooldown = 1.5F;
        private float repickCooldown;

        private void LoadData() {
            SpriteRenderer.sprite = stack.Item.Sprite;
        }

        private void Update() {
            repickCooldown -= Time.deltaTime;
            UpdateVanity();
        }

        private void OnTriggerEnter(Collider other) {
            if (stack == null || repickCooldown > 0) {
                return;
            }

            var e = other.GetComponentInParent<LivingEntity>();
            if (e == null) {
                return;
            }

            var inv = e.Inventory;
            byte left;
            if (inv.TryAddStack(stack, out left)) {
                Destroy(gameObject);
            } else {
                stack.Amount = left;
            }
        }

        public void DropAsItem(ItemStack item) {
            DropAsItem(item, new Vector3(Random.value, Random.value, Random.value));
        }

        public void DropAsItem(ItemStack item, Vector3 dir) {
            repickCooldown = RepickCooldown;
            stack = item;
            LoadData();
            dir.Normalize();
            dir *= DropForce;
            Rigidbody.velocity = dir;
        }
    }
}