using Minecraft.Scripts.Entities.Movable.Motors;
using UnityEditor;
using UnityUtilities;

namespace Minecraft.Scripts.Entities.Movable {
    public partial class MovableEntity {
        public Motor Motor;
        private MotorState state;
        public float MaxSpeed = 5;
        public float Aceleration = 1;

        public float DeAcceleration = 1;

        private void UpdateMovement() {
            Motor.Move(this);
        }

        private void ValidateMovement() {
            var m = Motor;
            if (m == null) {
                return;
            }

            m.EnsureCompatible(this);
        }

        public void EnsureMotorStateIs<T>() where T : MotorState {
            if (state != null && !(state is T)) {
#if UNITY_EDITOR
                if (!EditorApplication.isPlaying) {
                    DestroyImmediate(state);
                } else {
#endif
                    Destroy(state);
#if UNITY_EDITOR
                }
#endif
            }

            state = managersHolder.GetOrAddComponent<T>();
        }

        public T GetMotorState<T>() where T : MotorState {
            return state as T;
        }
    }
}