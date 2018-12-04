using Minecraft.Scripts.Entities.Movable;
using Minecraft.Scripts.Inventories;
using Minecraft.Scripts.UI;
using UnityEngine;

namespace Minecraft.Scripts.Master.UI.Inventories.Player {
    public class PlayerHotbarView : PlayerInventoryView {
        private ItemView[] itemViews;
        public RectTransform SelectionIndicator;
        public Vector2 SelectionOffset;

        protected override void Awake() {
            base.Awake();
            itemViews = GetComponentsInChildren<ItemView>();
        }

        protected override void Update() {
            var inv = CurrentInventory as EntityInventory;
            if (inv == null) {
                return;
            }

            SelectionIndicator.position = ((RectTransform) itemViews[inv.SelectedIndex].transform).position + (Vector3) SelectionOffset;
        }
    }
}