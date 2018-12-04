using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Minecraft.Scripts.UI {
    public class ImageView : UIView {
        public Color RevealedColor = Color.white, ConcealedColor = new Color(1, 1, 1, 0);
        public Image Image;
        public float RevealTransitionDuration = 0.25F;

        private void KillTweens() {
            Image.DOKill();
        }

        public override void OnReveal() {
            KillTweens();
            Image.DOColor(RevealedColor, RevealTransitionDuration);
        }

        public override void OnConceal() {
            KillTweens();
            Image.DOColor(ConcealedColor, RevealTransitionDuration);
        }

        public override void SnapReveal() {
            KillTweens();
            Image.color = RevealedColor;
        }

        public override void SnapConceal() {
            KillTweens();
            Image.color = ConcealedColor;
        }
    }
}