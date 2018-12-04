using Minecraft.Scripts.Entities.Movable;

namespace Minecraft.Scripts.Entities.Input {
    public abstract class EntityInputSource : InputSource {
        public abstract void ProcessExtras(MovableEntity movableEntity);
    }
}