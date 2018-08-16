using UnityEngine;

namespace Minecraft.Scripts.Entities.Movable.Motors {
    public class GroundMotorState : MotorState {
        public CharacterController Controller;
        public float JumpForce = 8;
        [SerializeField]
        private float lookPitch;

        public float MaxPitch = 90;
        public float MinPitch = -90;

        public float LookPitch {
            get {
                return lookPitch;
            }
            set {
                lookPitch = value;
                if (lookPitch > MaxPitch) {
                    lookPitch = MaxPitch;
                }

                if (lookPitch < MinPitch) {
                    lookPitch = MinPitch;
                }
            }
        }
    }
}