using System;
using System.Collections.Generic;
using Minecraft.Scripts.Utility;
using Minecraft.Scripts.World.Blocks;
using Minecraft.Scripts.World.Chunks;
using Minecraft.Scripts.World.Utilities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Minecraft.Scripts.World.Jobs {
    public struct BlockShape {
        private BlockCompositionType compositionType;
        public bool IsVisible => compositionType != BlockCompositionType.Invisible;

        public BlockShape(BlockCompositionType compositionType) {
            this.compositionType = compositionType;
        }

        public BlockCompositionType CompositionType => compositionType;
    }

    public struct ChunkBlock {
        private BlockShape shape;
        private BlockMaterial material;

        public ChunkBlock(BlockShape shape, BlockMaterial material) {
            this.shape = shape;
            this.material = material;
        }

        public BlockShape Shape => shape;

        public BlockMaterial Material => material;
    }

    public struct ChunkNeighbor : IDisposable {
        [ReadOnly]
        private NativeArray<BlockShape> wallShape;

        private byte wallWidth;

        public ChunkNeighbor(BlockShape[] wallShape, byte wallWidth) {
            this.wallShape = new NativeArray<BlockShape>(wallShape, Allocator.TempJob);
            this.wallWidth = wallWidth;
        }

        public NativeArray<BlockShape> WallShape => wallShape;

        public byte WallWidth => wallWidth;

        public BlockShape this[byte x, byte y] => wallShape[y * wallWidth + x];

        public void Dispose() {
            wallShape.Dispose();
        }
    }

    /// <summary>
    /// Memory cost:
    /// ((Neightboor wall) * 4 wall) + (Chunk Volume * BlockMemorySize)
    /// ((1 * 16 * 256 ) * 4) + ((16 * 16 * 256) * 1) = 81920 bytes = Cost of generating one chunk mesh
    /// Chunk * 12 = 983040 bytes 
    /// </summary>
    public struct GenerateMeshJobData : IDisposable {
        private ChunkNeighbor northNeighbor, southNeighbor, eastNeighbor, westNeighbor;

        [ReadOnly]
        private NativeArray<ChunkBlock> chunkShape;

        private byte chunkWidth, chunkHeight;


        public GenerateMeshJobData(ChunkNeighbor northNeighbor, ChunkNeighbor southNeighbor, ChunkNeighbor eastNeighbor, ChunkNeighbor westNeighbor, ChunkBlock[] chunkShape, byte chunkWidth, byte chunkHeight) {
            this.northNeighbor = northNeighbor;
            this.southNeighbor = southNeighbor;
            this.eastNeighbor = eastNeighbor;
            this.westNeighbor = westNeighbor;
            this.chunkShape = new NativeArray<ChunkBlock>(chunkShape, Allocator.TempJob);
            this.chunkWidth = chunkWidth;
            this.chunkHeight = chunkHeight;
        }

        public ChunkNeighbor NorthNeighbor => northNeighbor;

        public ChunkNeighbor SouthNeighbor => southNeighbor;

        public ChunkNeighbor EastNeighbor => eastNeighbor;

        public ChunkNeighbor WestNeighbor => westNeighbor;

        public NativeArray<ChunkBlock> ChunkShape => chunkShape;

        public byte ChunkWidth => chunkWidth;

        public byte ChunkHeight => chunkHeight;

        public ChunkBlock this[byte x, byte y, byte z] => chunkShape[IndexingUtility.IndexOf(x, y, z, chunkWidth, chunkHeight)];

        // ReSharper disable PossibleInvalidOperationException
        public BlockShape GetBlock(byte x, byte y, byte z, Vector3Int dir) {
            var horizontalBorder = (byte) (chunkWidth - 1);
            BlockShape? shape;
            if (CheckBorder(x, horizontalBorder, dir.x, z, y, ref eastNeighbor, ref westNeighbor, out shape)) {
                return shape.Value;
            }

            if (CheckBorder(z, horizontalBorder, dir.z, x, y, ref northNeighbor, ref southNeighbor, out shape)) {
                return shape.Value;
            }

            return this[(byte) (x + dir.x), (byte) (y + dir.y), (byte) (z + dir.z)].Shape;
        }

        private static bool CheckBorder(byte valueToCheck, byte limit, int dirX, byte wallX, byte wallY, ref ChunkNeighbor positiveNeighbor, ref ChunkNeighbor negativeNeighbor, out BlockShape? shape) {
            if (valueToCheck == 0 && dirX < 0) {
                shape = negativeNeighbor[wallX, wallY];
                return true;
            }

            if (valueToCheck == limit && dirX > 0) {
                shape = positiveNeighbor[wallX, wallY];
                return true;
            }

            shape = null;
            return false;
        }

        private static ChunkNeighbor LoadNeighbor(Chunk chunk, Vector3Byte origin, Vector3Byte limit) {
            var originX = origin.x;
            var originY = origin.y;
            var originZ = origin.z;

            var data = chunk.ChunkData;
            var totalBlocks = data.ChunkSize * data.ChunkHeight;
            var buffer = new BlockShape[totalBlocks];
            var currentBlock = 0;
            for (var x = originX; x < limit.x; x++) {
                for (var y = originY; y < limit.y; y++) {
                    for (var z = originZ; z < limit.z; z++) {
                        var block = data[x, y, z];
                        buffer[currentBlock++] = new BlockShape(block.Composition);
                    }
                }
            }

            return new ChunkNeighbor(buffer, data.ChunkSize);
        }

        public static GenerateMeshJobData From(Chunk chunk, World world) {
            var data = chunk.ChunkData;
            var width = data.ChunkSize;
            var height = data.ChunkHeight;
            var end = width * height * width;
            var shape = new ChunkBlock[end];
            for (uint i = 0; i < end; i++) {
                var block = data[i];
                shape[i] = new ChunkBlock(new BlockShape(block.Composition), block.Material);
            }

            var chunkPos = chunk.ChunkPosition;
            var finalIndex = (byte) (width - 1);
            var chunkX = chunkPos.x;
            var chunkY = chunkPos.y;
            var north = LoadNeighbor(world.GetChunkAt(chunkX, chunkY + 1), Vector3Byte.zero, new Vector3Byte(1, height, width));
            var south = LoadNeighbor(world.GetChunkAt(chunkX, chunkY - 1), new Vector3Byte(0, 0, finalIndex), new Vector3Byte(1, height, width));
            var west = LoadNeighbor(world.GetChunkAt(chunkX - 1, chunkY), new Vector3Byte(finalIndex, 0, 0), new Vector3Byte(width, height, 1));
            var east = LoadNeighbor(world.GetChunkAt(chunkX - 1, chunkY), Vector3Byte.zero, new Vector3Byte(width, height, 1));
            return new GenerateMeshJobData(north, south, east, west, shape, width, height);
        }

        public void Dispose() {
            chunkShape.Dispose();
            northNeighbor.Dispose();
            southNeighbor.Dispose();
            westNeighbor.Dispose();
            eastNeighbor.Dispose();
        }
    }

    public struct ChunkMesh : IDisposable {
        private NativeArray<Vector3> vertices;
        private NativeArray<int> indices;
        private NativeArray<Vector2> uvs;

        public ChunkMesh(MeshBuilder builder) {
            vertices = new NativeArray<Vector3>(builder.Vertices.ToArray(), Allocator.TempJob);
            indices = new NativeArray<int>(builder.Indices.ToArray(), Allocator.TempJob);
            uvs = new NativeArray<Vector2>(builder.Uvs.ToArray(), Allocator.TempJob);
        }

        public void Dispose() {
            vertices.Dispose();
            indices.Dispose();
            uvs.Dispose();
        }

        public NativeArray<Vector3> Vertices => vertices;

        public NativeArray<int> Indices => indices;

        public NativeArray<Vector2> Uvs => uvs;

        public Mesh Build() {
            var mesh = new Mesh {
                vertices = Vertices.ToArray(),
                triangles = Indices.ToArray(),
                uv = Uvs.ToArray()
            };
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }
    }

    public struct GenerateSubMeshJob : IJob, IDisposable {
        private GenerateMeshJobData chunkData;

        private BlockMaterial material;
        private NativeList<Vector3> vertices;
        private NativeList<int> indices;
        private NativeList<Vector2> uvs;

        public GenerateSubMeshJob(GenerateMeshJobData chunkData, BlockMaterial material) : this() {
            this.chunkData = chunkData;
            this.material = material;
            vertices = new NativeList<Vector3>(Allocator.TempJob);
            indices = new NativeList<int>(Allocator.TempJob);
            uvs = new NativeList<Vector2>(Allocator.TempJob);
        }

        public void Execute() {
            var chunkSize = chunkData.ChunkWidth;
            var chunkHeight = chunkData.ChunkHeight;
            var builder = new MeshBuilder();
            for (byte x = 0; x < chunkSize; x++) {
                for (byte y = 0; y < chunkHeight; y++) {
                    for (byte z = 0; z < chunkSize; z++) {
                        var currentBlock = chunkData[x, y, z];
                        if (!currentBlock.Shape.IsVisible) {
                            continue;
                        }

                        var currentMaterial = currentBlock.Material;

                        if (currentMaterial != material) {
                            continue;
                        }

                        foreach (var face in BlockFaces.Faces) {
                            var dir = face.ToDirection();
                            var isTopOrBottom = y == 0 && dir.y < 0 || y == chunkHeight - 1 && dir.y > 0;
                            if (isTopOrBottom || chunkData.GetBlock(x, y, z, dir).IsVisible) {
                                continue;
                            }

                            builder.AddFace(x, y, z, face);
                        }
                    }
                }
            }

            foreach (var vertex in builder.Vertices) {
                vertices.Add(vertex);
            }

            foreach (var builderIndex in builder.Indices) {
                indices.Add(builderIndex);
            }

            foreach (var builderUv in builder.Uvs) {
                uvs.Add(builderUv);
            }
        }

        public void Dispose() {
            vertices.Dispose();
            indices.Dispose();
            uvs.Dispose();
        }

        public Mesh Build() {
            var mesh = new Mesh {
                vertices = vertices.ToArray(),
                triangles = indices.ToArray(),
                uv = uvs.ToArray()
            };
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }
    }
    
}