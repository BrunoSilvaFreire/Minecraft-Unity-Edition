using Minecraft.Scripts.Entities.Input;
using Minecraft.Scripts.Entities.Movable;
using Minecraft.Scripts.Inventories;
using UnityEngine;
using UnityUtilities;

namespace Minecraft.Scripts.Game {
    public class PlayerInputSource : EntityInputSource {
        public string HorizontalMovementKey = "Horizontal";
        public string VerticalMovementKey = "Vertical";
        public string HorizontalRotationKey = "LookX";
        public string VerticalRotationKey = "LookY";
        public string DropItemKey = "DropItem";
        public string JumpKey = "Jump";
        public float MouseRotationSpeed = 3;

        public override float GetHorizontalMovement() {
            return Input.GetAxis(HorizontalMovementKey);
        }

        public override float GetForwardMovement() {
            return Input.GetAxis(VerticalMovementKey);
        }

        public override float GetHorizontalRotation() {
            return Input.GetAxis(HorizontalRotationKey) * MouseRotationSpeed;
        }

        public override float GetVerticalRotation() {
            return Input.GetAxis(VerticalRotationKey) * MouseRotationSpeed;
        }

        public override bool GetJump() {
            return Input.GetButtonDown(JumpKey);
        }

        public override bool GetLeftMouse() {
            return Input.GetMouseButton(0);
        }

        public override void ProcessExtras(MovableEntity movableEntity) {
            var inv = movableEntity.Inventory;
            UpdateHeldSlot(movableEntity, inv);

            if (Input.GetButtonDown(DropItemKey)) {
                var obj = inv[inv.SelectedIndex];
                if (obj != null) {
                    inv[inv.SelectedIndex] = null;
                    var pos = movableEntity.transform.position;
                    pos.y += 1;
                    var fwd = movableEntity.EntityCamera.transform.forward;
                    Player.Instance.BreakingParameters.ItemPrefab.Clone(pos + fwd).DropAsItem(obj, fwd);
                }
            }
        }

        private void UpdateHeldSlot(MovableEntity movableEntity, EntityInventory inv) {
            for (byte i = 1; i <= 9; i++) {
                if (Input.GetKeyDown(i.ToString())) {
                    inv.SelectedIndex = (byte) (i - 1);
                }
            }

            var msd = Input.mouseScrollDelta.y;
            if (msd > 0 && inv.SelectedIndex != StackInventory.Width - 1) {
                inv.SelectedIndex++;
            }

            if (msd < 0 && inv.SelectedIndex != 0) {
                inv.SelectedIndex--;
            }
        }
    }
}