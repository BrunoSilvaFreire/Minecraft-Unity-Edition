using System.Collections;
using DG.Tweening;
using Minecraft.Scripts.Inventories;
using UnityEngine;
using UnityEngine.UI;
using UnityUtilities;

namespace Minecraft.Scripts.Master.UI.Inventories {
    public class SelectedItemView : MonoBehaviour {
        private byte lastSelected;
        public Text TextLabel;
        public float HoldDuration = 1, FadeDuration = 1;
        private Coroutine displayRoutine;

        private void Update() {
            var inv = Game.Player.Instance.CurrentEntity.Inventory;
            var selected = inv.SelectedIndex;
            if (selected == lastSelected) {
                return;
            }

            lastSelected = selected;
            CoroutineUtility.ReplaceCoroutine(ref displayRoutine, this, ShowSelected(inv));
        }

        private IEnumerator ShowSelected(Inventory inv) {
            var stack = inv[lastSelected];
            if (stack == null) {
                TextLabel.text = string.Empty;
                yield break;
            }

            var msg = stack.Item.Alias;
            if (stack.Amount > 1) {
                msg += " x" + stack.Amount;
            }

            TextLabel.text = msg;
            TextLabel.color = Color.white;
            yield return new WaitForSeconds(HoldDuration);
            TextLabel.DOColor(Color.clear, FadeDuration);
        }
    }
}