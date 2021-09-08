using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ru1t3rl.MeshGen
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class GenerateCylinder : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 10f)] int radius;
        int previousRadius;

        [SerializeField, Range(6, 100)] int nVertices;
        int lastVertexCount;
        Vector3[] vertices;
        int[] triangles;
        Vector2[] uvs;

        Mesh mesh;

        void Awake()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Cylinder";

            GenerateMesh();
        }


        void Update()
        {
            if (lastVertexCount != nVertices || previousRadius != radius)
                GenerateMesh();

            lastVertexCount = nVertices;
            previousRadius = radius;
        }

        void GenerateMesh()
        {
            mesh.Clear();

            GenerateVertices();
            GenerateTriangles();
            GenerateUVs();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
        }

        /// <summary>
        /// Place the vertices in a circle using Sine and Cosine
        /// </summary>
        void GenerateVertices()
        {
            float x, z;

            nVertices -= nVertices % 2;

            vertices = new Vector3[nVertices + 2];

            vertices[0] = new Vector3(0, -radius / 2f, 0);

            for (int i = 0; i < (nVertices) / 2f; i++)
            {
                x = Mathf.Sin(2 * Mathf.PI / (nVertices / 2f) * i) * radius;
                z = Mathf.Cos(2 * Mathf.PI / (nVertices / 2f) * i) * radius;

                vertices[i + 1] = new Vector3(x, -radius / 2f, z);
                vertices[i + vertices.Length / 2] = new Vector3(x, radius / 2f, z);
            }

            vertices[vertices.Length - 1] = new Vector3(0, radius / 2f, 0);
        }

        /// <summary>
        /// Generate Triangles best on the vertices
        /// </summary>
        void GenerateTriangles()
        {
            List<int> newTriangles = new List<int>();
            triangles = new int[(vertices.Length) * 3];

            for (int i = 0; i < (vertices.Length - 2) / 2; i++)
            {
                #region Draw Bottom

                // Center Vertex
                triangles[i * 3] = 0;

                // First Vertex
                triangles[i * 3 + 2] = i + 1;

                // Second Vertex
                if (i + 2 >= vertices.Length / 2)
                    triangles[i * 3 + 1] = 1;
                else
                    triangles[i * 3 + 1] = i + 2;

                #endregion

                #region Draw Top

                //Center Vertex
                triangles[(vertices.Length) / 2 * 3 + i * 3] = vertices.Length - 1;

                // 1st vertex
                triangles[(vertices.Length) / 2 * 3 + i * 3 + 1] = vertices.Length / 2 + i;

                // last vertex check if still in vertices range
                if (vertices.Length / 2 + i + 2 < vertices.Length)
                {
                    triangles[(vertices.Length) / 2 * 3 + i * 3 + 2] = vertices.Length / 2 + i + 1;
                }
                else
                {
                    triangles[(vertices.Length) / 2 * 3 + i * 3 + 2] = vertices.Length / 2;
                }

                #endregion
            }

            newTriangles.AddRange(triangles);

            #region Draw Side

            // -2 since I don't need the center vertices
            for (int iVertex = 0; iVertex < (vertices.Length - 2) / 2; iVertex++)
            {

                #region Triangle 1

                if (iVertex + 2 < vertices.Length / 2)
                    newTriangles.Add(iVertex + 2);
                else
                {
                    newTriangles.Add(1 + (iVertex + 2) % (vertices.Length / 2));
                }

                if (vertices.Length / 2 + iVertex + 2 < vertices.Length - 1)
                    newTriangles.Add(vertices.Length / 2 + iVertex + 2);
                else
                {
                    newTriangles.Add(vertices.Length / 2 + (vertices.Length / 2 + iVertex + 2) % (vertices.Length - 1));
                }

                if (vertices.Length / 2 + iVertex + 1 < vertices.Length - 1)
                    newTriangles.Add(vertices.Length / 2 + iVertex + 1);
                else
                {
                    newTriangles.Add(vertices.Length / 2 + (vertices.Length / 2 + iVertex + 1) % (vertices.Length - 1));
                }


                #endregion

                #region Triangle 2

                newTriangles.Add(vertices.Length / 2 + iVertex + 1 < vertices.Length - 1
                    ? vertices.Length / 2 + iVertex + 1
                    : vertices.Length / 2);

                newTriangles.Add(iVertex + 1 < vertices.Length / 2 ? iVertex + 1 : 1 + (iVertex + 1) % (vertices.Length / 2));

                newTriangles.Add(iVertex + 2 < vertices.Length / 2 ? iVertex + 2 : 1 + (iVertex + 2) % (vertices.Length / 2));

                #endregion

            }

            #endregion

            triangles = newTriangles.ToArray();
        }

        void GenerateUVs()
        {
            uvs = new Vector2[vertices.Length];

            for (var iVertex = 0; iVertex < uvs.Length; iVertex++)
            {
                uvs[iVertex] = new Vector2(vertices[iVertex].x, vertices[iVertex].z);
            }
        }

        /// <summary>
        /// Draw gizmo's mainly for debug purpose
        /// </summary>
#if DEBUG
        void OnDrawGizmos()
        {
            if (vertices == null || vertices.Length == 0)
                return;

            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], .1f);
            }
        }
#endif
    }
}