using Minecraft.Scripts.Input;

namespace Minecraft.Scripts.Entities.Movable {
    public delegate void OwnershipRemovalCallback(OwnershipRemovalReason reason);


    public enum OwnershipRemovalReason {
        Revoked,
        Override,
        Destroyed
    }

    public partial class MovableEntity {
        private OwnershipRemovalCallback callback;

        public InputSource InputSource {
            get;
            private set;
        }

        public bool HasInputSource => InputSource != null;

        public void RequestOwnership(InputSource controller, OwnershipRemovalCallback removalCallback) {
            if (HasInputSource) {
                RevokeOwnership(OwnershipRemovalReason.Override);
            }

            InputSource = controller;
            callback = removalCallback;
        }

        private void RevokeOwnership(OwnershipRemovalReason reason) {
            var cb = callback;
            cb?.Invoke(reason);
            callback = null;
            InputSource = null;
        }

        public InputSource RevokeOwnership() {
            var oldOwner = InputSource;
            RevokeOwnership(OwnershipRemovalReason.Revoked);
            return oldOwner;
        }
    }
}