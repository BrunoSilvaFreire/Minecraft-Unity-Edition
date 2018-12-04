using DG.Tweening;
using UnityEngine;
using UnityUtilities;

namespace Minecraft.Scripts.UI {
    public class CanvasGroupView : UIView {
        public CanvasGroup CanvasGroup;
        public float RevealedAlpha = 1, ConcealedAlpha = 0;
        public float RevealTransitionDuration = 0.25F;
        public float RevealedScale = 1, ConcealedScale = 0.9F;
        public RectTransform ScaleEffectTransform;
        
        protected override void Reset() {
            CanvasGroup = this.GetOrAddComponent<CanvasGroup>();
            ScaleEffectTransform = RectTransform;
        }

        public override void OnReveal() {
            KillTweens();
            CanvasGroup.DOFade(RevealedAlpha, RevealTransitionDuration);
            ScaleEffectTransform.DOScale(RevealedScale, RevealTransitionDuration);
        }

        public override void OnConceal() {
            KillTweens();
            CanvasGroup.DOFade(ConcealedAlpha, RevealTransitionDuration);
            ScaleEffectTransform.DOScale(ConcealedScale, RevealTransitionDuration);
        }

        public override void SnapReveal() {
            KillTweens();
            CanvasGroup.alpha = RevealedAlpha;
            ScaleEffectTransform.localScale = new Vector3(RevealedScale, RevealedScale, RevealedScale);
        }

        public override void SnapConceal() {
            KillTweens();
            CanvasGroup.alpha = ConcealedAlpha;
            ScaleEffectTransform.localScale = new Vector3(RevealedScale, RevealedScale, RevealedScale);
        }

        private void KillTweens() {
            CanvasGroup.DOKill();
            ScaleEffectTransform.DOKill();
        }
    }
}