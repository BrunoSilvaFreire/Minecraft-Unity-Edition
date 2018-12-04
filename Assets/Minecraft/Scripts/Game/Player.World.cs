using Minecraft.Scripts.World.Chunks;
using UnityEngine;

namespace Minecraft.Scripts.Game {
    public partial class Player {
        public byte ViewDistance = 5;
        public bool AutomaticChunkHandling = true;
        private void UpdateWorld() {
            if (CurrentEntity == null || !AutomaticChunkHandling) {
                return;
            }

            var world = Scripts.World.World.Instance;
            // TODO: Disgusting, I Know
            world.MeshGenerator.PriorityPosition = currentEntity.transform.position;
            CheckUnloadedChunks(world);
            CheckLoadedChunks(world);
        }

        private void CheckUnloadedChunks(Scripts.World.World world) {
            var entityPos = CurrentEntity.transform.position;
            var pos = world.GetChunkPositionAt((int) entityPos.x, (int) entityPos.z);
            for (var x = -ViewDistance; x <= ViewDistance; x++) {
                for (var y = -ViewDistance; y <= ViewDistance; y++) {
                    var xPos = x + pos.x;
                    var yPos = y + pos.y;
                    var chunk = world.GetChunk(xPos, yPos);
                    if (!chunk.IsMeshGenerated) {
                        chunk.GenerateMesh(world, false);
                    }
                }
            }
        }

        private void CheckLoadedChunks(Scripts.World.World world) {
            foreach (var chunk in world.LoadedChunks.ToArray()) {
                if (!IsWithinViewDistance(world, chunk)) {
                    world.UnloadChunk(chunk);
                }
            }
        }

        private bool IsWithinViewDistance(Scripts.World.World world, Chunk chunk) {
            var entityPos = CurrentEntity.transform.position;
            var pos = world.GetChunkPositionAt((int) entityPos.x, (int) entityPos.z);
            var cPos = chunk.ChunkPosition;
            var dX = Mathf.Abs(pos.x - cPos.x);
            var dY = Mathf.Abs(pos.y - cPos.y);
            //Debug.Log($"Checking chunk {cPos}");
            //Debug.Log($"{nameof(dX)} = {dX} / {ViewDistance}");
            //Debug.Log($"{nameof(dY)} = {dY}");
            return dX <= ViewDistance && dY <= ViewDistance;
        }

        private void DrawWorldGizmos() {
            var c = Color.red;
            c.a = 0.5F;
            var w = Scripts.World.World.Instance;
            Gizmos.color = c;
            var size = new Vector3(ViewDistance * w.ChunkSize, ViewDistance * w.ChunkHeight, ViewDistance * w.ChunkSize);
            Gizmos.DrawWireCube(CurrentEntity.transform.position, size);
        }
    }
}