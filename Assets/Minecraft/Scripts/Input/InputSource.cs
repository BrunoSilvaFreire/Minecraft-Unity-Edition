using UnityEngine;
using Object = UnityEngine.Object;

namespace Minecraft.Scripts.Input {
    public abstract class InputSource : MonoBehaviour {
        public abstract float GetHorizontalMovement();
        public abstract float GetFowardMovement();
        public abstract float GetHorizontalRotation();
        public abstract float GetVerticalRotation();

        public abstract bool GetJump();
    }
}