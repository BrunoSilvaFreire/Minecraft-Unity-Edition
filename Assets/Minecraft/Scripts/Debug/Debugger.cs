using Minecraft.Scripts.Game;
using UnityEditor;
using UnityEngine;

namespace Minecraft.Scripts.Debug {
    public abstract class Debugger : MonoBehaviour {
        private void OnDrawGizmos() {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) {
                return;
            }
#endif
            Debug(Player.Instance, World.World.Instance);
        }

        protected abstract void Debug(Player player, World.World world);
    }
}