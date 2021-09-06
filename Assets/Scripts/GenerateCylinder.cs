using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenerateCylinder : MonoBehaviour
{
    [SerializeField] int radius;
    [SerializeField, Range(6, 100)]
    int nVertices;
    int lastVertexCount;
    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Cylinder";

        GenerateMesh();
    }


    void Update()
    {
        if (lastVertexCount != nVertices)
            GenerateMesh();

        lastVertexCount = nVertices;
    }

    void GenerateMesh()
    {
        mesh.Clear();

        GenerateVertices();
        GenerateTriangles();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void GenerateVertices()
    {
        float x, z;

        nVertices -= nVertices % 2;

        vertices = new Vector3[nVertices + 2];

        vertices[0] = new Vector3(0, -.5f, 0);

        for (int i = 1; i < nVertices / 2; i++)
        {
            x = Mathf.Sin((i - 1) / (vertices.Length / 2f) * 360f * Mathf.Deg2Rad + radius / 2f);
            z = Mathf.Cos((i - 1) / (vertices.Length / 2f) * 360f * Mathf.Deg2Rad + radius / 2f);

            vertices[i] = new Vector3(x, -.5f, z);
            vertices[i + vertices.Length / 2] = new Vector3(x, .5f, z);
        }

        vertices[vertices.Length - 1] = new Vector3(0, .5f, 0);
    }

    void GenerateTriangles()
    {
        triangles = new int[vertices.Length + vertices.Length / 2];

        for (int i = 0; i < vertices.Length / 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
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
