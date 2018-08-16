namespace Minecraft.Scripts.Input {
    public class PlayerInputSource : InputSource {
        public string HorizontalMovementKey = "Horizontal";
        public string VerticalMovementKey = "Vertical";
        public string HorizontalRotationKey = "LookX";
        public string VerticalRotationKey = "LookY";
        public string JumpKey = "Jump";
        public float MouseRotationSpeed = 3;

        public override float GetHorizontalMovement() {
            return UnityEngine.Input.GetAxis(HorizontalMovementKey);
        }

        public override float GetFowardMovement() {
            return UnityEngine.Input.GetAxis(VerticalMovementKey);
        }

        public override float GetHorizontalRotation() {
            return UnityEngine.Input.GetAxis(HorizontalRotationKey) * MouseRotationSpeed;
        }

        public override float GetVerticalRotation() {
            return UnityEngine.Input.GetAxis(VerticalRotationKey) * MouseRotationSpeed;
        }

        public override bool GetJump() {
            return UnityEngine.Input.GetButtonDown(JumpKey);
        }
    }
}