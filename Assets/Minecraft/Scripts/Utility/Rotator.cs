using UnityEngine;

namespace Minecraft.Scripts.Utility {
    [ExecuteInEditMode]
    public class Rotator : MonoBehaviour {
        public Vector3 RotateDirection;
        public float RotationSpeed;
        public bool LocalRotation;

        private void Update() {
            transform.Rotate(RotateDirection * RotationSpeed * Time.deltaTime,
                LocalRotation ? Space.Self : Space.World);
        }
    }
}