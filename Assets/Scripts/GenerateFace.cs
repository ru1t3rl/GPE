using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFace : MonoBehaviour
{
    Mesh mesh;
    Vector3[] verts;
    int[] triangles;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        for (int i = 0; i < 6; i++)
        {

        }
    }
}
