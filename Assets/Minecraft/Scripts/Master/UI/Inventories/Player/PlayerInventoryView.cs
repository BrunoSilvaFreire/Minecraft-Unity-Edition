using System.Collections.Generic;
using Minecraft.Scripts.Entities.Movable;
using Minecraft.Scripts.Inventories;
using Minecraft.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Minecraft.Scripts.Master.UI.Inventories.Player {
    public class PlayerInventoryView : CanvasGroupView {
        private readonly List<ItemView> slots = new List<ItemView>();
        public GameObject[] SlotHolders;
        protected Inventory CurrentInventory;

        protected override void Awake() {
            base.Awake();
            LoadSlots();
            Game.Player.Instance.OnEntityChanged.AddListener(ReloadInventory);
        }

        private void LoadSlots() {
            foreach (var slotHolder in SlotHolders) {
                slots.AddRange(slotHolder.GetComponentsInChildren<ItemView>());
            }
        }


        private void ReloadInventory(MovableEntity entity) {
            if (CurrentInventory != null) {
                CurrentInventory.OnModified.RemoveListener(OnModified);
            }

            CurrentInventory = entity.Inventory;
            CurrentInventory.OnModified.AddListener(OnModified);
            if (CurrentInventory == null) {
                return;
            }

            UpdateInventory();
        }

        public void OnModified() {
            UpdateInventory();
        }

        private void UpdateInventory() {
            for (var i = 0; i < slots.Count; i++) {
                var view = slots[i];
                view.Import(CurrentInventory[(uint) i]);
            }
        }

        protected virtual void Update() {
            if (Input.GetButtonDown("Cancel")) {
                Revealed = !Revealed;
            }
        }
    }
}