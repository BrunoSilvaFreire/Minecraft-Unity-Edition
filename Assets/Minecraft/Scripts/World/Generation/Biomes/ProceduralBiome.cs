using System;
using System.Collections.Generic;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using Minecraft.Scripts.World.Structures;
using UnityEngine;
using UnityUtilities;
using Random = UnityEngine.Random;

namespace Minecraft.Scripts.World.Generation.Biomes {
    [Serializable]
    public sealed class OreCluster {
        public Block Block;
        public UInt8Range Amount;
        public UInt8Range MaxWidth, MaxHeight, MaxDepth;
        public float Probability = 50;

        public Vector3Int GetMaxSize() {
            return new Vector3Int(
                MaxWidth.Evaluate(),
                MaxHeight.Evaluate(),
                MaxDepth.Evaluate()
            );
        }

        public void Place(World world, Vector3Int origin) {
            var amount = Amount.Evaluate();
            var maxSize = GetMaxSize();
            var possibleSizes = new List<Vector3Int>();
            for (byte x = 0; x < maxSize.x; x++) {
                for (byte y = 0; y < maxSize.y; y++) {
                    for (byte z = 0; z < maxSize.z; z++) {
                        var volume = x * y * z;
                        if (volume < amount || volume > amount * 2) {
                            //Not big enough to fit or too big
                            continue;
                        }

                        possibleSizes.Add(new Vector3Int(x, y, z));
                    }
                }
            }

            var dimension = possibleSizes.RandomElement();
            var size = dimension.x * dimension.y * dimension.z;
            byte totalPlacedOres = 0;
            var blocks = new Block[size];
            while (totalPlacedOres != amount) {
                var index = Random.Range(0, size);
                var b = blocks[index];
                if (b != null) {
                    continue;
                }

                blocks[index] = Block;
                totalPlacedOres++;
            }

            Structure.PlaceOnto(world, origin, dimension, blocks);
        }
    }

    [Serializable]
    public sealed class CrustElement {
        public byte Height;
        public Block Block;
    }

    [Serializable]
    public sealed class CrustConfig {
        public byte MinimumHeight = 8;
        public CrustElement[] CrustElements;

        public Block GetCrustBlockFor(byte offsetFromSurface) {
            var heightLeft = offsetFromSurface;
            foreach (var element in CrustElements) {
                var height = element.Height;
                if (heightLeft > height) {
                    heightLeft -= height;
                } else {
                    return element.Block;
                }
            }

            return null;
        }
    }

    [Serializable]
    public sealed class SubterraneanConfig {
        public Block DefaultBlock;
        public OreCluster[] Clusters;
    }

    [CreateAssetMenu(menuName = "Minecraft/World/Biomes/ProceduralBiome")]
    public class ProceduralBiome : Biome {
        public CrustConfig Crust;
        public SubterraneanConfig Subterrain;
        public float PerlinScale = 5;

        public override void Populate(World world, ref ChunkData data, Vector2Int chunkPosition) {
            var worldSize = world.ChunkSize;
            var worldHeight = world.ChunkHeight;
            var minHeight = Crust.MinimumHeight;
            var surfaceHeightBuffer = new byte[worldSize, worldSize];
            for (byte x = 0; x < worldSize; x++) {
                for (byte z = 0; z < worldSize; z++) {
                    float pX = chunkPosition.x * world.ChunkSize + x;
                    float pZ = chunkPosition.y * world.ChunkSize + z;
                    var noise = Mathf.PerlinNoise(pZ / worldSize, pX / worldSize) * PerlinScale;
                    var height = (byte) (noise + minHeight);
                    surfaceHeightBuffer[x, z] = height;
                    for (byte y = 0; y < worldHeight; y++) {
                        Block block;
                        if (y > height) {
                            block = world.BlockDatabase.Air;
                        } else {
                            var offsetFromSurface = (byte) (height - y);
                            block = Crust.GetCrustBlockFor(offsetFromSurface);
                            if (block == null) {
                                block = Subterrain.DefaultBlock;
                            }
                        }

                        data[x, y, z] = block;
                    }
                }
            }

            foreach (var oreCluster in Subterrain.Clusters) {
                while (oreCluster.Probability / 100 > Random.value) {
                    var x = Random.Range(0, worldSize);
                    var z = Random.Range(0, worldSize);
                    var origin = new Vector3Int(
                        x,
                        Random.Range(0, worldHeight - surfaceHeightBuffer[x, z]),
                        z
                    );
                    oreCluster.Place(world, origin);
                }
            }
        }
    }
}