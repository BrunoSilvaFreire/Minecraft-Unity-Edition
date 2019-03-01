using Minecraft.Scripts.Game;
using Minecraft.Scripts.World.Jobs;

namespace Minecraft.Scripts.Debug {
    public class PopulatorDebugger : JobSystemDebugger<Populator, PopulatorWorker, PopulateJob> {
        protected override void Debug(Player player, World.World world) {
            Debug(world.Populator);
        }
    }
}