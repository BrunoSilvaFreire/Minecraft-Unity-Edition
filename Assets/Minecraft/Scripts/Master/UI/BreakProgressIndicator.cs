using Minecraft.Scripts.Game.World;
using UnityEngine;
using UnityEngine.UI;

namespace Minecraft.Scripts.Master.UI {
    public class BreakProgressIndicator : MonoBehaviour {
        public BlockBreaker Breaker;
        public GameObject Master;
        public Image Image;

        private void Update() {
            var b = Breaker.Breaking;
            Master.SetActive(b);

            if (!b) {
                return;
            }

            Image.fillAmount = Breaker.Progress;
        }
    }
}