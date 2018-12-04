using UnityEngine;

namespace Minecraft.Scripts.Entities.Input {
    public abstract class InputSource : MonoBehaviour {
        public abstract float GetHorizontalMovement();
        public abstract float GetForwardMovement();
        public abstract float GetHorizontalRotation();
        public abstract float GetVerticalRotation();
        public abstract bool GetJump();
        public abstract bool GetLeftMouse();
    }
}