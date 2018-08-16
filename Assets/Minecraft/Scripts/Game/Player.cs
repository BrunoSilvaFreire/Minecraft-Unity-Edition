using Minecraft.Scripts.Entities.Movable;
using Minecraft.Scripts.Game.World;
using Minecraft.Scripts.Input;
using UnityEngine;
using MovableEntity = Minecraft.Scripts.Entities.Movable.MovableEntity;

namespace Minecraft.Scripts.Game {
    public partial class Player : MonoBehaviour {
        public PlayerInputSource InputSource;
        public MovableEntity CurrentEntity;
        public int ActiveEntityCameraPriority = 10, InactiveEntityCameraPriority = 0;

        private void Start() {
            StartMain();
            StartBreaking();
        }

        private void StartMain() {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (CurrentEntity == null) {
                return;
            }

            CurrentEntity.RequestOwnership(InputSource, OnEntityRevoke);
            TrySetEntityCameraPriority(CurrentEntity, ActiveEntityCameraPriority);
        }

        private void OnEntityRevoke(OwnershipRemovalReason reason) {
            TrySetEntityCameraPriority(CurrentEntity, InactiveEntityCameraPriority);
            CurrentEntity = null;
        }

        private void Update() {
            UpdateBreaker();
        }
    }
}