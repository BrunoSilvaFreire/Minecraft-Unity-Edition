using Minecraft.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

#endif

namespace Minecraft.Scripts.Debug {
    public class ChunkDebugger : Debugger {
        public Text ChunkInfoLabel;
        public Text PlayerInfoLabel;

        protected override void Debug(Player player, World.World world) {
            var chunk = world.GetChunkAt(player.CurrentEntity.transform.position, false);
            if (chunk == null) {
                return;
            }

            var size = world.ChunkSize;
            var entity = player.CurrentEntity;
            PlayerInfoLabel.text = entity == null ? "No Entity" : $"Position: {entity.transform.position}";
            ChunkInfoLabel.text = $"Current Chunk: {chunk.name} - {size}/{world.ChunkHeight}";
        }

        protected override void DrawGizmos(Player player, World.World world) {
            var chunk = world.GetChunkAt(player.CurrentEntity.transform.position, false);
            if (chunk == null) {
                return;
            }

            var size = world.ChunkSize;
            var width = (float) size / 2;
            var center = chunk.transform.position + new Vector3(width, 0, width);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, new Vector3(size, world.ChunkHeight, size));
        }
    }
}