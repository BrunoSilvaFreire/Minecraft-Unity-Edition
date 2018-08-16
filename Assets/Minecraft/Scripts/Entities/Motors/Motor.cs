using UnityEngine;

namespace Minecraft.Scripts.Entities.Motors {
    public abstract class Motor : ScriptableObject {
        public abstract void Move(MovableEntity entity);
        public abstract void EnsureCompatible(MovableEntity entity);
    }
}