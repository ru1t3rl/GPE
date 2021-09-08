using System.Collections;
using System.Collections.Generic;
using Ru1t3rl.MeshGen;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Ru1t3rl.MeshGen
{
    public class GenerateGridWithChunks : GenerateGrid
    {
        Chunk[,] chunks;
        [SerializeField] Vector2Int maxChunkSize;
        Vector2Int previousMaxChunkSize;
        Vector2Int heightmapChunkSize;

        [SerializeField] Material material;
        Material previousMaterial;


        protected override void LateUpdate()
        {
            base.LateUpdate();

#if UNITY_EDITOR
            if (previousMaxChunkSize != maxChunkSize || previousMaterial != material)
                Generate();

            previousMaxChunkSize = maxChunkSize;
            previousMaterial = material;
#endif
        }

        protected override void Generate()
        {
            if (chunks != null && chunks.Length > 0)
                DestroyChunks();

            // Max Size per chunk is 256, this has to do with a max amount of vertices
            chunks = new Chunk[Mathf.CeilToInt(1f * gridSize.x / maxChunkSize.x), Mathf.CeilToInt(1f * gridSize.y / maxChunkSize.y)];
            heightmapChunkSize = new Vector2Int(heightmap.width / chunks.GetLength(1), heightmap.height / chunks.GetLength(0));

            for (int y = 0; y < chunks.GetLength(0); y++)
            {
                for (int x = 0; x < chunks.GetLength(1); x++)
                {
                    GameObject chunkObj = new GameObject($"Chunk [{x}, {y}]", typeof(MeshRenderer), typeof(MeshFilter));
                    chunkObj.transform.parent = transform;
                    chunkObj.transform.localPosition = new Vector3(x * gridSize.x / (chunks.GetLength(0) * 1f), 0, y * gridSize.y / (chunks.GetLength(1) * 1f));
                    chunkObj.hideFlags = HideFlags.HideInHierarchy;

                    Chunk chunk = chunkObj.AddComponent<Chunk>();
                    chunk.GenerateChunk(
                        this,
                        (x < chunks.GetLength(1) - 1 && y < chunks.Length - 1) || (gridSize.x % maxChunkSize.x == 0 && gridSize.y % maxChunkSize.y == 0) ? maxChunkSize
                            : new Vector2Int(gridSize.x % maxChunkSize.x, gridSize.y % maxChunkSize.y),
                        new Vector2Int(x, y),
                        heightmapChunkSize,
                        material,
                        heightmap,
                        heightMultiplier);

                    chunks[x, y] = chunk;
                }
            }
        }

        void DestroyChunks()
        {
            for (int y = 0; y < chunks.GetLength(0); y++)
            {
                for (int x = 0; x < chunks.GetLength(1); x++)
                {
                    try
                    {
                        chunks[x, y].Destroy();
                    }
                    catch (System.NullReferenceException) { }
                }
            }
        }
    }
}