using Minecraft.Scripts.Entities;
using Minecraft.Scripts.Game;
using UnityEngine;

namespace Minecraft.Scripts.Combat {
    public class Bazooka : MonoBehaviour {
        public Explosion Explosion;
        public KeyCode KeyCode = KeyCode.B;
        public Entity Entity;

        private void Update() {
            if (!UnityEngine.Input.GetKeyDown(KeyCode)) {
                return;
            }

            var camera = Entity.EntityCamera;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out var hit)) {
                var w = World.World.Instance;
                var blockPos = hit.point + -hit.normal * Mathf.Epsilon;
                Explosion.Play(w, new Vector3Int((int) blockPos.x, (int) blockPos.y, (int) blockPos.z));
            }
        }
    }
}