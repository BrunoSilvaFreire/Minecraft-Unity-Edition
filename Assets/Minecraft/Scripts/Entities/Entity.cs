using Cinemachine;
using UnityEngine;

namespace Minecraft.Scripts.Entities {
    public class Entity : MonoBehaviour {
        [SerializeField]
        protected GameObject managersHolder;

        public CinemachineVirtualCamera EntityCamera;
    }
}