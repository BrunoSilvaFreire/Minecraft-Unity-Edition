using Minecraft.Scripts.Entities.Input;
using Minecraft.Scripts.Entities.Movable;
using Minecraft.Scripts.Game.World;
using UnityEngine;
using UnityEngine.Events;
using UnityUtilities.Singletons;
using MovableEntity = Minecraft.Scripts.Entities.Movable.MovableEntity;

namespace Minecraft.Scripts.Game {
    public partial class Player : Singleton<Player> {
        public PlayerInputSource InputSource;

        [SerializeField]
        private MovableEntity currentEntity;

        public MovableEntityEvent OnEntityChanged;
        public int ActiveEntityCameraPriority = 10, InactiveEntityCameraPriority = 0;

        public MovableEntity CurrentEntity {
            get {
                return currentEntity;
            }
            set {
                var old = currentEntity;
                currentEntity = value;
                OnEntityChanged.Invoke(old);
            }
        }

        private void Start() {
            StartMain();
            StartBreaking();
            InvokeEvents();
        }

        private void InvokeEvents() {
            if (currentEntity != null) {
                Debug.Log("Initializing to " + currentEntity);
                OnEntityChanged.Invoke(currentEntity);
            }
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
            UpdateWorld();
        }

        private void OnDrawGizmos() {
            DrawWorldGizmos();
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lastBreakPos, 1);
        }
    }
}