using UnityEngine;

namespace Minecraft.Scripts.Entities {
    public partial class MovableEntity : MonoBehaviour {
        [SerializeField]
        private GameObject managersHolder;

        private void Update() {
            UpdateMovement();
        }

        private void OnValidate() {
            ValidateMovement();
        }
    }
}