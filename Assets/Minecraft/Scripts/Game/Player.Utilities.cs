using Minecraft.Scripts.Entities.Movable;

namespace Minecraft.Scripts.Game {
    public partial class Player {
        private void TrySetEntityCameraPriority(MovableEntity entity, int priority) {
            var cam = CurrentEntity.EntityCamera;
            if (cam == null) {
                return;
            }

            cam.Priority = priority;
        }
    }
}