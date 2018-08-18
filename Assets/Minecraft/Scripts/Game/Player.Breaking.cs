using System;
using Cinemachine;
using Minecraft.Scripts.Game.World;
using Minecraft.Scripts.World.Blocks;
using UnityEngine;

namespace Minecraft.Scripts.Game {
    [Serializable]
    public class PlayerBreakingParameters {
        public float BreakDistance = 2;
        public float BreakRaycastEpsilon = .1F;
        public LayerMask BreakableLayerMask;
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
            var db = Scripts.World.World.Instance.BlockDatabase;
            Scripts.World.World.Instance.SetBlock(lastBreakPos, db.Air);
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
            var normal = Vector3.zero;
            var hit = false;
            if (Breaker.Breaking) {
                if (!playerPressingBreak) {
                    Breaker.StopBreaking();
                }

                Vector3Int currentBlockPos;
                hit = CastBreakRay(cam, out hitInfo, out currentBlockPos, out hitBlock, out blockMaterial, out normal);
                if (hit) {
                    if (currentBlockPos != lastBreakPos) {
                        Breaker.SetBreaking(currentBlockPos, hitBlock, blockMaterial);
                    }

                    lastBreakPos = currentBlockPos;
                }
            } else {
                hit = CastBreakRay(cam, out hitInfo, out lastBreakPos, out hitBlock, out blockMaterial, out normal);
                if (playerPressingBreak && hit) {
                    Breaker.SetBreaking(lastBreakPos, hitBlock, blockMaterial);
                }
            }


            if (!hit) {
                Highlighter.Deselect();
            } else {
                Breaker.LastHitPosition = hitInfo.point;
                var hightlightPos = lastBreakPos + (normal * BreakingParameters.BreakRaycastEpsilon);
                var hightlightPosInt = new Vector3Int((int) hightlightPos.x, (int) hightlightPos.y, (int) hightlightPos.z);
                Highlighter.Highlight(hightlightPosInt);
            }
        }

        private bool CastBreakRay(CinemachineVirtualCamera cam, out RaycastHit hit, out Vector3Int blockPosition, out Block hitBlock, out Material blockMaterial, out Vector3 normal) {
            var cameraTransform = cam.transform;
            var dir = cameraTransform.forward;
            var result = Physics.Raycast(cameraTransform.position, dir, out hit, BreakingParameters.BreakDistance, BreakingParameters.BreakableLayerMask);
            var blockPos = hit.point + dir * BreakingParameters.BreakRaycastEpsilon;
            blockPosition = new Vector3Int((int) blockPos.x, (int) blockPos.y, (int) blockPos.z);
            normal = hit.normal;
            if (!result) {
                hitBlock = null;
                blockMaterial = null;
                return false;
            }

            var world = Scripts.World.World.Instance;
            hitBlock = world.GetBlock(blockPosition);
            blockMaterial = hit.collider.GetComponent<MeshRenderer>().sharedMaterial;
            return true;
        }
    }
}