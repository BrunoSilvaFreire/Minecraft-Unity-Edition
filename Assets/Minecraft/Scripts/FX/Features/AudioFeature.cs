using Shiroi.FX.Features;
using UnityEngine;

namespace Minecraft.Scripts.FX.Features {
    public class AudioFeature : EffectFeature {
        public AudioFeature(Vector3 forward, Vector3 up, Vector3 velocity, params PropertyName[] tags) : base(tags) {
            Forward = forward;
            Up = up;
            Velocity = velocity;
        }

        public AudioFeature(Transform transform, Vector3 velocity, params PropertyName[] tags) : this(
            transform.forward,
            transform.up,
            velocity,
            tags
        ) { }

        public Vector3 Forward {
            get;
        }

        public Vector3 Up {
            get;
        }

        public Vector3 Velocity {
            get;
        }
    }
}