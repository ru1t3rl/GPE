using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class GenerateSphere : MonoBehaviour
{
    [SerializeField, Range(3, 100)]
    int segments = 6, rings = 5;
    int previousSegments, previousRings;

    [SerializeField] float radius = 1f;
    float previousRadius;

    Vector3[] vertices;
    int[] triangles;
    bool goingBack = false;

    Mesh mesh;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Sphere";
    }

    void Start()
    {
        GenerateShape();
    }

    void LateUpdate()
    {
#if UNITY_EDITOR
        if (previousRings != rings || previousSegments != segments || previousRadius != radius)
            GenerateShape();

        previousRings = rings;
        previousSegments = segments;
        previousRadius = radius;
#endif
    }

    public void GenerateShape()
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
        //float radiusIncrement = radius / ((rings + 1f) / 2f);
        float tempRadius = 0, x, z;
        float angle;

        vertices = new Vector3[rings * segments + 2];

        vertices[0] = new Vector3(0, radius, 0);

        for (int y = 1, i = 1; y < rings/ - 1; y++)
        {
            // Calculate the angle of the ring compared to the center
            //angle = ((y) / (rings - 2f) * 180f) * Mathf.Deg2Rad;
            angle = y < rings / 2f ? y / (rings / 2f - 1f) * 180f : y > rings / 2f ? y / (rings / 2f - 1f) * 180f * -1f : 90f;
            //.angle = y / (rings/2 - 1f) * 180f;
            angle *= Mathf.Deg2Rad;

            // Calculate the radius using SOS CAS TOA
            tempRadius = Mathf.Cos(angle) * radius;

            for (int j = 0; j < segments; j++, i++)
            {
                x = Mathf.Sin(2 * Mathf.PI / segments * j) * tempRadius;
                z = Mathf.Cos(2 * Mathf.PI / segments * j) * tempRadius;

                if (y < rings / 2)
                    vertices[i] = new Vector3(x, Mathf.Sin(angle) * radius, z);
                else if (y > rings / 2)
                    vertices[i] = new Vector3(x, Mathf.Sin(angle) * -radius, z);
                else
                    vertices[i] = new Vector3(x, Mathf.Sin(angle) * radius, z);
            }
        }

        vertices[vertices.Length - 1] = new Vector3(0, -radius, 0);
    }

    void GenerateTriangles()
    {
        List<int> newTriangles = new List<int>();
        for (int iRing = 1; iRing < rings - 1; iRing++)
        {
            for (int iVertex = 0; iVertex < segments; iVertex++)
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
        }

        triangles = newTriangles.ToArray();
    }

    void OnDrawGizmos()
    {
        if (vertices == null || vertices.Length < 0)
            return;

        vertices.ToList().ForEach(vert => Gizmos.DrawSphere(vert, .1f));
    }
}
