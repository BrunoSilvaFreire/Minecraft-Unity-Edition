using System;
using Minecraft.Scripts.Entities.Living;
using UnityEngine.Events;

namespace Minecraft.Scripts.Entities.Movable {
    [Serializable]
    public sealed class MovableEntityEvent : UnityEvent<MovableEntity> { }

    public partial class MovableEntity : LivingEntity {
        private void Update() {
            UpdateMovement();
            UpdateInput();
        }

        private void OnValidate() {
            ValidateMovement();
        }
    }
}