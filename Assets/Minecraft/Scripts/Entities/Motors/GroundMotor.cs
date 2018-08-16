using UnityEngine;

namespace Minecraft.Scripts.Entities.Motors {
    [CreateAssetMenu(menuName = "Minecraft/Motors/GroundMotor")]
    public class GroundMotor : Motor {
        public float MovementThreshold = .1F;

        public override void EnsureCompatible(MovableEntity entity) {
            entity.EnsureMotorStateIs<GroundMotorState>();
        }

        public override void Move(MovableEntity entity) {
            var s = entity.GetMotorState<GroundMotorState>();
            var controller = s.Controller;
            var velocity = s.Velocity;
            var source = entity.InputSource;
            var inputX = source.GetHorizontalMovement();
            var inputZ = source.GetFowardMovement();
            var inputYaw = source.GetHorizontalRotation();
            entity.transform.Rotate(new Vector3(0, inputYaw, 0));
            if (Mathf.Abs(inputX) > MovementThreshold || Mathf.Abs(inputZ) > MovementThreshold) {
                Accelerate(ref velocity, entity, inputX, inputZ);
            } else {
                Deaccelerate(ref velocity, entity);
            }


            velocity = Vector3.ClampMagnitude(velocity, entity.MaxSpeed);
            controller.SimpleMove(velocity);
            s.Velocity = velocity;
        }

        private static void Deaccelerate(ref Vector3 velocity, MovableEntity entity) {
            var deaccelerationVector = -velocity;
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