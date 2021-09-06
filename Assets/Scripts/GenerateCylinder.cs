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

        for (int i = 0; i < (nVertices) / 2f; i++)
        {
            x = Mathf.Sin(2 * MathF.PI / (nVertices / 2f) * i) * radius;
            z = Mathf.Cos(2 * MathF.PI / (nVertices / 2f) * i) * radius;

            vertices[i + 1] = new Vector3(x, -.5f, z);
            vertices[i + vertices.Length / 2] = new Vector3(x, .5f, z);
        }

        vertices[vertices.Length - 1] = new Vector3(0, .5f, 0);
    }

    void GenerateTriangles()
    {
        triangles = new int[(vertices.Length - 1) * 3];

        for (int i = 0; i < (vertices.Length - 2) / 2; i++)
        {
            #region DrawBottom
            triangles[i * 3] = 0;

            triangles[i * 3 + 1] = i + 1;

            if (i + 2 >= vertices.Length / 2)
                triangles[i * 3 + 2] = 1;
            else
                triangles[i * 3 + 2] = i + 2;
            #endregion

            #region Draw Top
            triangles[(vertices.Length - 2) / 2 + i * 3] = vertices.Length - 1;

            triangles[(vertices.Length - 2) / 2 + i * 3 + 1] = ((vertices.Length - 2) / 2 + 1) + i + 1;

            if (i + 2 >= vertices.Length / 2)
                triangles[(vertices.Length - 2) / 2 + i * 3 + 2] = ((vertices.Length - 2) / 2 + 1);
            else
                triangles[(vertices.Length - 2) / 2 + i * 3 + 2] = ((vertices.Length - 2) / 2 - 2 + 1) + i + 2;
            #endregion
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
