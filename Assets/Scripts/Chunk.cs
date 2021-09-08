using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ru1t3rl.MeshGen
{

    public class Chunk : MonoBehaviour
    {
        public Vector2Int size { private set; get; }
        public Vector2Int index { private set; get; }

        Texture2D heightmap;
        float heightMultiplier;

        Vector3[] vertices;
        int[] triangles;
        Vector2[] uvs;
        Color[] colors;

        Mesh mesh;

        GenerateGrid parent;

        public void GenerateChunk(GenerateGrid parent, Vector2Int size, Vector2Int index, Material material, Texture2D heightmap, float heightMultiplier = 1)
        {
            this.parent = parent;
            this.size = size;
            this.index = index;
            this.heightmap = heightmap;
            this.heightMultiplier = heightMultiplier;

            GetComponent<MeshRenderer>().materials = new Material[] { material };
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();

            GenerateVertices();
            mesh.vertices = vertices;

            GenerateTriangles();
            mesh.triangles = triangles;

            GenerateUVs();
            mesh.uv = uvs;

            mesh.colors = colors;
            mesh.RecalculateNormals();
        }

        protected virtual void GenerateVertices()
        {
            // Place the vertices
            vertices = new Vector3[(size.x + 1) * (size.y + 1)];
            for (int i = 0, z = 0; z <= size.y; z++)
            {
                for (int x = 0; x <= size.x; x++, i++)
                {
                    vertices[i] = new Vector3(
                        x,
                        Mathf.PerlinNoise(x * .3f, z * .2f) * 3,
                        z);
                }
            }

            if (heightmap != null)
                ApplyHeightMap();
        }

        protected virtual void GenerateTriangles()
        {
            // Create triangles between vertices
            triangles = new int[size.x * size.y * 6];
            for (int ti = 0, vi = 0, y = 0; y < size.y; y++, vi++)
            {
                for (int x = 0; x < size.x; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + size.x + 1;
                    triangles[ti + 5] = vi + size.x + 2;
                }
            }
        }

        protected virtual void GenerateUVs()
        {
            uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            }
        }

        protected virtual void ApplyHeightMap()
        {
            colors = new Color[vertices.Length];

            // Set vertex height based on pixel grayscale and the heightMultiplier
            float pixelHeight = 0;
            for (int i = 0, z = 0; z <= size.y; z++)
            {
                for (int x = 0; x <= size.x; x++, i++)
                {
                    pixelHeight = heightmap.GetPixel(x * (heightmap.width / size.x), z * (heightmap.width / size.y))
                        .grayscale;
                    vertices[i] = new Vector3(x, pixelHeight * heightMultiplier, z);
                    colors[i] = GetColor(pixelHeight * heightMultiplier);
                }
            }
        }

        protected Color GetColor(float height)
        {
            int bestLayer = -1;
            for (var iLayer = 0; iLayer < parent.LayerColors.Length; iLayer++)
            {
                if (bestLayer <= -1 ||
                    (parent.LayerColors[iLayer].height <= height &&
                     parent.LayerColors[bestLayer].height < parent.LayerColors[iLayer].height))
                {
                    bestLayer = iLayer;
                }
            }

            return parent.LayerColors[bestLayer].color;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}