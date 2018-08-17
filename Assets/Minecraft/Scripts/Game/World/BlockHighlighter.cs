using UnityEngine;

namespace Minecraft.Scripts.Game.World {
    public class BlockHighlighter : MonoBehaviour {
        public GameObject Highlighter;

        public void Deselect() {
            Highlighter.SetActive(false);
        }

        public void Highlight(Vector3Int position) {
            Highlighter.SetActive(true);
            Highlighter.transform.position = position + new Vector3(.5F, .5F, .5F);
        }
    }
}