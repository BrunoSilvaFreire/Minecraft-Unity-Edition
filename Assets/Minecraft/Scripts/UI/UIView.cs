using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityUtilities;

namespace Minecraft.Scripts.UI {
    public abstract class UIView : UIBehaviour {
        public RectTransform RectTransform => (RectTransform) transform;

        [SerializeField, HideInInspector]
        private bool revealed;

        public List<UIView> Attached;

        public bool Revealed {
            get {
                return revealed;
            }
            set {
                if (value) {
                    Reveal();
                } else {
                    Conceal();
                }

                if (Attached.IsNullOrEmpty()) {
                    return;
                }

                foreach (var view in Attached) {
                    view.Revealed = value;
                }
            }
        }

        protected override void Start() {
            UpdateStateWithSnap();
        }

        private void UpdateStateWithSnap() {
            if (revealed) {
                SnapReveal();
            } else {
                SnapConceal();
            }
        }

        public void Reveal() {
            revealed = true;
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) {
                SnapReveal();
                return;
            }
#endif
            OnReveal();
        }

        public void Conceal() {
            revealed = false;
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) {
                SnapConceal();
                return;
            }
#endif
            OnConceal();
        }

        public abstract void OnReveal();
        public abstract void OnConceal();
        public abstract void SnapReveal();
        public abstract void SnapConceal();
    }
}