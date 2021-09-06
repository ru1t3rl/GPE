using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateGrid : MonoBehaviour
{
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

    void Update()
    {
        if (previousGridSize != gridSize)
        {
            mesh.Clear();
            Generate();
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = Random.Range(0f, 1f);
        }

        mesh.vertices = vertices;

        previousGridSize = gridSize;
    }

    void Generate()
    {
        // Place the vertices
        vertices = new Vector3[(gridSize.x + 1) * (gridSize.y + 1)];
        for (int i = 0, y = 0; y <= gridSize.y; y++)
        {
            for (int x = 0; x <= gridSize.x; x++, i++)
            {
                vertices[i] = new Vector3(x, 0, y);
            }
        }

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
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
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
