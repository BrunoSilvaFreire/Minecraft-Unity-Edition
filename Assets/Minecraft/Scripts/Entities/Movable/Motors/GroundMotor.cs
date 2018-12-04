using Cinemachine;
using UnityEngine;

namespace Minecraft.Scripts.Entities.Movable.Motors {
    [CreateAssetMenu(menuName = "Minecraft/Motors/GroundMotor")]
    public class GroundMotor : Motor {
        public float MovementThreshold = .1F;
        public float GravityScale = 1;
        public override void EnsureCompatible(MovableEntity entity) {
            entity.EnsureMotorStateIs<GroundMotorState>();
        }

        public override void Move(MovableEntity entity) {
            var s = entity.GetMotorState<GroundMotorState>();
            var controller = s.Controller;
            var velocity = s.Velocity;
            var source = entity.InputSource;
            var inputX = source.GetHorizontalMovement();
            var inputZ = source.GetForwardMovement();
            var inputYaw = source.GetHorizontalRotation();
            var inputPitch = source.GetVerticalRotation();
            s.LookPitch -= inputPitch;
            TryUpdateEntityCamera(entity.EntityCamera, s.LookPitch);
            entity.transform.Rotate(new Vector3(0, inputYaw, 0));
            if (Mathf.Abs(inputX) > MovementThreshold || Mathf.Abs(inputZ) > MovementThreshold) {
                Accelerate(ref velocity, entity, inputX, inputZ);
            } else if (controller.isGrounded) {
                Deaccelerate(ref velocity, entity);
            }

            var maxSpeed = entity.MaxSpeed;
            ClampVelocity(ref velocity, maxSpeed);
            if (controller.isGrounded && source.GetJump()) {
                velocity.y += s.JumpForce;
            }
            velocity += Physics.gravity * (GravityScale * Time.deltaTime);

            var flags = controller.Move(velocity * Time.deltaTime);
            if (flags == CollisionFlags.Below || flags == CollisionFlags.Above) {
                velocity.y = 0;
            }

            s.Velocity = velocity;
        }

        private static void ClampVelocity(ref Vector3 velocity, float maxSpeed) {
            var oldY = velocity.y;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            velocity.y = oldY;
        }

        private static void TryUpdateEntityCamera(CinemachineVirtualCamera camera, float lookPitch) {
            if (camera == null) {
                return;
            }

            camera.transform.localRotation = Quaternion.Euler(lookPitch, 0, 0);
        }

        private static void Deaccelerate(ref Vector3 velocity, MovableEntity entity) {
            var deaccelerationVector = new Vector3(-velocity.x, 0, -velocity.z);
            deaccelerationVector.Normalize();
            deaccelerationVector *= entity.DeAcceleration;
            deaccelerationVector = Vector3.ClampMagnitude(deaccelerationVector, velocity.magnitude);
            velocity += deaccelerationVector;
        }

        private static void Accelerate(ref Vector3 velocity, MovableEntity entity, float inputX, float inputZ) {
            var acel = new Vector3(inputX * entity.Aceleration, 0, inputZ * entity.Aceleration);
            acel = entity.transform.rotation * acel;
            velocity += acel;
        }
    }
}