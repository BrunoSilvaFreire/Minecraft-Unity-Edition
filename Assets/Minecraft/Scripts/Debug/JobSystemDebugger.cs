using Minecraft.Scripts.Game;
using Minecraft.Scripts.Utility.Multithreading;
using UnityEngine.UI;

namespace Minecraft.Scripts.Debug {
    public abstract class JobSystemDebugger<T, W, J> : Debugger where T : JobSystem<W, J> where W : AbstractWorker<J> where J : Job<J> {
        public Text Display;
        protected void Debug(T jobSystem) {
            var text = string.Empty;
            text += $"Workers ({jobSystem.TotalWorkers}):\n";
            foreach (var worker in jobSystem.Workers) {
                text += $"* {worker}\n";
                var j = worker.CurrentJob;
                text += $"{new string(' ', 2)}* Currently executing: {(j == null ? "nothing" : j.ToString())}\n";
            }
            Display.text = text;
        }

        protected override void DrawGizmos(Player player, World.World world) { }
    }
}