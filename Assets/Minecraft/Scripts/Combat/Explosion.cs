using System;
using Minecraft.Scripts.World.Selection;
using UnityEngine;

namespace Minecraft.Scripts.Combat {
    [Serializable]
    public class Explosion {
        public float Radius = 3;

        public void Play(World.World world, Vector3Int center) {
            Selections.SphereSelection(world, center, Radius).DeleteAll();
        }
    }
}