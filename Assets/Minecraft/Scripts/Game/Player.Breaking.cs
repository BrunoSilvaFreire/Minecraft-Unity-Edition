using System;
using Cinemachine;
using Minecraft.Scripts.Entities.Items;
using Minecraft.Scripts.Entities.Items.Misc;
using Minecraft.Scripts.Game.World;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using Minecraft.Scripts.World.Utilities;
using UnityEngine;

namespace Minecraft.Scripts.Game {
    [Serializable]
    public class PlayerBreakingParameters {
        public float BreakDistance = 2;
        public float BreakRaycastEpsilon = .1F;
        public ItemEntity ItemPrefab;
    }

    public partial class Player {
        public BlockBreaker Breaker;
        public BlockHighlighter Highlighter;
        public PlayerBreakingParameters BreakingParameters;

        private Vector3Int lastBreakPos;

        private void StartBreaking() {
            Breaker.OnBlockBroke.AddListener(OnBlockBroke);
        }

        private void OnBlockBroke() {
            var world = Scripts.World.World.Instance;
            var db = world.BlockDatabase;
            var block = world.GetBlock(lastBreakPos);
            Debug.Log("sSetting blocok @ " + lastBreakPos + " to air");
            world.SetBlock(lastBreakPos, db.GetBlock(BlockMaterial.Air));
            foreach (var drop in block.Drops) {
                drop.Drop(BreakingParameters.ItemPrefab, lastBreakPos.AddHalf());
            }
        }

        private void UpdateBreaker() {
            var entity = CurrentEntity;
            if (entity == null) {
                return;
            }

            var cam = entity.EntityCamera;
            if (cam == null) {
                return;
            }

            var playerPressingBreak = entity.InputSource.GetLeftMouse();


            RaycastHit hitInfo;
            Block hitBlock;
            Material blockMaterial;
            bool hit;
            //TODO: Make this better
            if (Breaker.Breaking) {
                if (!playerPressingBreak) {
                    Breaker.StopBreaking();
                    return;
                }

                hit = CastBreakRay(cam, out hitInfo, out var currentBlockPos, out hitBlock, out blockMaterial);
                if (hit) {
                    if (currentBlockPos != lastBreakPos) {
                        Breaker.SetBreaking(currentBlockPos, hitBlock, blockMaterial);
                    }

                    lastBreakPos = currentBlockPos;
                } else {
                    Breaker.StopBreaking();
                }
            } else {
                hit = CastBreakRay(cam, out hitInfo, out lastBreakPos, out hitBlock, out blockMaterial);
                if (playerPressingBreak && hit) {
                    Breaker.SetBreaking(lastBreakPos, hitBlock, blockMaterial);
                }
            }


            if (!hit) {
                Highlighter.Deselect();
            } else {
                Breaker.LastHitPosition = hitInfo.point;
                var highlightPos = lastBreakPos;
                Highlighter.Highlight(highlightPos);
            }
        }

        private bool CastBreakRay(CinemachineVirtualCamera cam, out RaycastHit hit, out Vector3Int blockPosition,
            out Block hitBlock, out Material blockMaterial) {
            var cameraTransform = cam.transform;
            var dir = cameraTransform.forward;
            var result = Physics.Raycast(cameraTransform.position, dir, out hit, BreakingParameters.BreakDistance,
                Scripts.World.World.Instance.WorldMask);
            var blockPos = hit.point + -hit.normal * BreakingParameters.BreakRaycastEpsilon;


            blockPosition = new Vector3Int((int) blockPos.x, (int) blockPos.y, (int) blockPos.z);
            if (blockPos.x < 0) {
                blockPosition.x--;
            }

            if (blockPos.z < 0) {
                blockPosition.z--;
            }

            var chunk = result ? hit.collider.GetComponentInParent<Chunk>() : null;
            if (!result || chunk != null && !chunk.MeshGenerationStatus.IsFinished) {
                hitBlock = null;
                blockMaterial = null;
                return false;
            }

            var world = Scripts.World.World.Instance;
            hitBlock = world.GetBlock(blockPosition);
            var r = hit.collider.GetComponent<MeshRenderer>();
            blockMaterial = r.sharedMaterial;
            return true;
        }
    }
}