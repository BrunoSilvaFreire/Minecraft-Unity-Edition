namespace Minecraft.Scripts.Entities.Movable {
    public partial class MovableEntity : Entity {

        private void Update() {
            UpdateMovement();
        }

        private void OnValidate() {
            ValidateMovement();
        }
    }
}