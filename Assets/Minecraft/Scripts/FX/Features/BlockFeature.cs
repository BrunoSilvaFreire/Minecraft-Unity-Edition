using Minecraft.Scripts.World.Blocks;
using Shiroi.FX.Features;
using UnityEngine;

namespace Minecraft.Scripts.FX.Features {
	public class BlockFeature : EffectFeature {
		private Block block;
		public BlockFeature(Block block, params PropertyName[] tags) : base(tags) {
			this.block = block;
		}
		public Block Block => block;
	}
	
}