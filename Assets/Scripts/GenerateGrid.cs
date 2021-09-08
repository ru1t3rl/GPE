using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ru1t3rl.MeshGen
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class GenerateGrid : MonoBehaviour
    {
        [SerializeField] Texture2D heightmap;
        Texture2D previousHeightmap;
        [SerializeField] float heightMultiplier = 2f;
        [SerializeField] LayerColor[] layerColors;
        Color[] colors;

        [SerializeField] Vector2Int gridSize;
        Vector2Int previousGridSize;

        Vector3[] vertices;
        Vector2[] uvs;
        int[] triangles;
        Mesh mesh;

        void Awake()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Procedural Grid";

            Generate();
        }

        void LateUpdate()
        {
#if UNITY_EDITOR
            if (previousGridSize != gridSize || previousHeightmap != heightmap)
            {
                mesh.Clear();
                Generate();
            }

            previousGridSize = gridSize;
            previousHeightmap = heightmap;
#endif
        }

        void Generate()
        {
            // Place the vertices
            vertices = new Vector3[(gridSize.x + 1) * (gridSize.y + 1)];
            for (int i = 0, y = 0; y <= gridSize.y; y++)
            {
                for (int x = 0; x <= gridSize.x; x++, i++)
                {
                    vertices[i] = new Vector3(x, Mathf.PerlinNoise(x * .3f, y * .2f) * 3, y);
                }
            }

            if (heightmap != null)
                ApplyHeightMap();

            // Create triangles between vertices
            triangles = new int[gridSize.x * gridSize.y * 6];
            for (int ti = 0, vi = 0, y = 0; y < gridSize.y; y++, vi++)
            {
                for (int x = 0; x < gridSize.x; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + gridSize.x + 1;
                    triangles[ti + 5] = vi + gridSize.x + 2;
                }
            }

            uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            }

            // Link the vertices and triangles to the mesh
            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        void ApplyHeightMap()
        {
            colors = new Color[vertices.Length];

            // Set vertex height based on pixel grayscale and the heightMultiplier
            float pixelHeight = 0;
            for (int i = 0, y = 0; y <= gridSize.y; y++)
            {
                for (int x = 0; x <= gridSize.x; x++, i++)
                {
                    pixelHeight = heightmap.GetPixel(x * (heightmap.width / gridSize.x), y * (heightmap.width / gridSize.y)).grayscale;
                    vertices[i] = new Vector3(x, pixelHeight * heightMultiplier, y);
                    colors[i] = GetColor(pixelHeight * heightMultiplier);
                }
            }
        }

        Color GetColor(float height)
        {
            int bestLayer = -1;
            for (var iLayer = 0; iLayer < layerColors.Length; iLayer++)
            {
                if (bestLayer <= -1 ||
                    (layerColors[iLayer].height <= height && layerColors[bestLayer].height < layerColors[iLayer].height))
                {
                    bestLayer = iLayer;
                }
            }

            return layerColors[bestLayer].color;
        }

        void OnDrawGizmos()
        {
            if (vertices == null || vertices.Length == 0)
                return;

            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], .1f);
            }
        }
    }

    [System.Serializable]
    public class LayerColor
    {
        public string name;
        public float height;
        public Color color;
    }
}