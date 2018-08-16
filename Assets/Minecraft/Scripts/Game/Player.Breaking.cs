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
        public PlayerBreakingParameters BreakingParameters;

        private Vector3Int lastBreakPos;

        private void StartBreaking() {
            Breaker.OnBlockBroke.AddListener(OnBlockBroke);
        }

        private void OnBlockBroke() {
            Scripts.World.World.Instance.SetBlock(lastBreakPos, Blocks.Air.Material);
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
            if (Breaker.Breaking) {
                if (!playerPressingBreak) {
                    Breaker.StopBreaking();
                }

                Vector3Int currentBlockPos;
                if (!CastBreakRay(cam, out hitInfo, out currentBlockPos, out hitBlock, out blockMaterial)) {
                    return;
                }

                if (currentBlockPos != lastBreakPos) {
                    Breaker.SetBreaking(currentBlockPos, hitBlock, blockMaterial);
                }

                lastBreakPos = currentBlockPos;
            } else {
                if (!playerPressingBreak) {
                    return;
                }

                if (!CastBreakRay(cam, out hitInfo, out lastBreakPos, out hitBlock, out blockMaterial)) {
                    return;
                }

                Breaker.SetBreaking(lastBreakPos, hitBlock, blockMaterial);
            }
        }

        private bool CastBreakRay(CinemachineVirtualCamera cam, out RaycastHit hit, out Vector3Int blockPosition, out Block hitBlock, out Material blockMaterial) {
            var cameraTransform = cam.transform;
            var dir = cameraTransform.forward;
            var result = Physics.Raycast(cameraTransform.position, dir, out hit, BreakingParameters.BreakDistance, BreakingParameters.BreakableLayerMask);
            var blockPos = hit.point + dir * BreakingParameters.BreakRaycastEpsilon;
            blockPosition = new Vector3Int((int) blockPos.x, (int) blockPos.y, (int) blockPos.z);
            if (!result) {
                hitBlock = null;
                blockMaterial = null;
                return false;
            }

            var world = Scripts.World.World.Instance;
            hitBlock = Blocks.GetBlock(world.GetBlock(blockPosition));
            blockMaterial = hit.collider.GetComponent<MeshRenderer>().sharedMaterial;
            return true;
        }
    }
}