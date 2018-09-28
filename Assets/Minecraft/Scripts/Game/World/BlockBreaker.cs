using Minecraft.Scripts.FX;
using Minecraft.Scripts.FX.Features;
using Minecraft.Scripts.World;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Utilities;
using Shiroi.FX.Effects;
using Shiroi.FX.Features;
using Shiroi.FX.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Minecraft.Scripts.Game.World {
    public class BlockBreaker : MonoBehaviour {
        public const string MetallicKey = "_Metallic";
        public const string MainTextureKey = "_MainTex";
        public const string GlossinessKey = "_Glossiness";
        public MeshRenderer BlockCutout;
        public UnityEvent OnBlockBroke;
        public Effect BlockBreakEffect;
        public float BlockHitEffectFrequency = 1;
        public Effect BlockHitEffect;

        public bool Breaking {
            get;
            private set;
        }

        public Texture[] BreakingTextures;
        private Block currentBlock;
        private Material lastMaterial;

        public void StopBreaking() {
            Breaking = false;
            BlockCutout.gameObject.SetActive(false);
        }

        private void Start() {
            propertyBlock = new MaterialPropertyBlock();
        }

        private MaterialPropertyBlock propertyBlock;

        public Vector3 LastHitPosition {
            get;
            set;
        }

        public void SetBreaking(Vector3Int blockWorldPosition, Block block, Material blockMaterial) {
            BlockCutout.GetPropertyBlock(propertyBlock);
            lastMaterial = blockMaterial;
            propertyBlock.SetFloat(MetallicKey, blockMaterial.GetFloat(MetallicKey));
            propertyBlock.SetFloat(GlossinessKey, blockMaterial.GetFloat(GlossinessKey));
            BlockCutout.SetPropertyBlock(propertyBlock);
            var go = BlockCutout.gameObject;
            go.SetActive(true);
            go.transform.position = blockWorldPosition.AddHalf();
            Breaking = true;
            currentBlock = block;
            currentBreakingProgress = 0;
        }

        private float currentBreakingProgress;
        public float Progress => currentBreakingProgress;
        private float lastHit;

        private void Update() {
            if (currentBlock == null || !Breaking) {
                return;
            }

            lastHit += Time.deltaTime;
            if (Breaking) {
                currentBreakingProgress += 1 / currentBlock.Hardness * Time.deltaTime;
            }

            var hitEffectDelay = 1 / BlockHitEffectFrequency;
            if (Mathf.Approximately(0, lastHit) || lastHit > hitEffectDelay) {
                lastHit -= hitEffectDelay;
                BlockHitEffect.PlayIfPresent(
                    new EffectContext(
                        this,
                        new BlockFeature(currentBlock),
                        new PositionFeature(LastHitPosition),
                        new MaterialFeature(lastMaterial),
                        new AudioFeature(BlockCutout.transform, Vector3.zero)
                    )
                );
            }

            if (currentBreakingProgress >= 1) {
                Break();
            } else {
                var currentTexture = (int) (currentBreakingProgress * 10);
                propertyBlock.SetTexture(MainTextureKey, BreakingTextures[currentTexture]);
                BlockCutout.SetPropertyBlock(propertyBlock);
            }
        }

        private EffectContext GetEffectContext() {
            return new EffectContext(
                this,
                new BlockFeature(currentBlock),
                new PositionFeature(BlockCutout.transform.position),
                new MaterialFeature(lastMaterial),
                new AudioFeature(BlockCutout.transform, Vector3.zero)
            );
        }

        private void Break() {
            OnBlockBroke.Invoke();
            BlockBreakEffect.PlayIfPresent(GetEffectContext);
            currentBreakingProgress = 0;
            lastHit = 0;
            StopBreaking();
        }
    }
}