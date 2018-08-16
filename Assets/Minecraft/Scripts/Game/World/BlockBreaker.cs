using Minecraft.Scripts.World;
using Minecraft.Scripts.World.Blocks;
using UnityEngine;
using UnityEngine.Events;

namespace Minecraft.Scripts.Game.World {
    public class BlockBreaker : MonoBehaviour {
        public const string MetallicKey = "_Metallic";
        public const string MainTextureKey = "_MainTex";
        public const string GlossinessKey = "_Glossiness";
        public MeshRenderer BlockCutout;
        public UnityEvent OnBlockBroke;

        public bool Breaking {
            get;
            private set;
        }

        public Texture[] BreakingTextures;
        private Block currentBlock;

        public void StopBreaking() {
            Breaking = false;
            BlockCutout.gameObject.SetActive(false);
        }

        private void Start() {
            propertyBlock = new MaterialPropertyBlock();
        }

        private MaterialPropertyBlock propertyBlock;

        public void SetBreaking(Vector3Int blockWorldPosition, Block block, Material blockMaterial) {
            BlockCutout.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(MetallicKey, blockMaterial.GetFloat(MetallicKey));
            propertyBlock.SetFloat(GlossinessKey, blockMaterial.GetFloat(GlossinessKey));
            BlockCutout.SetPropertyBlock(propertyBlock);
            var go = BlockCutout.gameObject;
            go.SetActive(true);
            go.transform.position = blockWorldPosition + new Vector3(0.5F, 0.5F, 0.5F);
            Breaking = true;
            currentBlock = block;
            currentBreakingProgress = 0;
        }

        private float currentBreakingProgress;

        private void Update() {
            if (currentBlock == null || !Breaking) {
                return;
            }

            if (Breaking) {
                currentBreakingProgress += 1 / currentBlock.Hardness * Time.deltaTime;
            }


            if (currentBreakingProgress >= 1) {
                Break();
            } else {
                var currentTexture = (int) (currentBreakingProgress * 10);
                propertyBlock.SetTexture(MainTextureKey, BreakingTextures[currentTexture]);
                BlockCutout.SetPropertyBlock(propertyBlock);
            }
        }

        private void Break() {
            OnBlockBroke.Invoke();
            currentBreakingProgress = 0;
            StopBreaking();
        }
    }
}