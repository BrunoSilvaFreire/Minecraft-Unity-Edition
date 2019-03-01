using Minecraft.Scripts.Game;
using Minecraft.Scripts.World.Jobs;

namespace Minecraft.Scripts.Debug {
    public class MeshGeneratorDebugger : JobSystemDebugger<MeshGenerator, MeshWorker, MeshJob> {
        protected override void Debug(Player player, World.World world) {
            Debug(world.MeshGenerator);
        }
    }
}