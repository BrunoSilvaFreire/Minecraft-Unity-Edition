using Minecraft.Scripts.Entities;
using Minecraft.Scripts.Game;
using UnityEngine;

namespace Minecraft.Scripts.Misc {
    public class PlayerMagnet : MonoBehaviour {
        public Rigidbody Rigidbody;
        public AnimationCurve Speed = AnimationCurve.EaseInOut(0, 1, 1, 0);
        public float Distance;

        private void Update() {
            var p = Player.Instance;
            Entity entity = p.CurrentEntity;
            if (p == null || (entity) == null) {
                return;
            }

            var entityPos = entity.transform.position;
            var pos = transform.position;
            var distance = Vector3.Distance(entityPos, pos);
            if (distance > Distance) {
                return;
            }

            var direction = entityPos - pos;
            direction.Normalize();
            direction *= Speed.Evaluate(distance / Distance);
            Rigidbody.velocity = direction;
        }
    }
}