using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateIcoSphere : MonoBehaviour
{
    public Material icoSphereMaterial;
    public float icoSphereSize = 1f;
    GameObject icoSphere;
    Mesh icoSphereMesh;
    Vector3[] icoSphereVertices;
    int[] icoSPhereTriangles;
    MeshRenderer icoSphereMeshRenderer;
    MeshFilter icoSphereMeshFilter;
    MeshCollider icoSphereMeshCollider;

    public void CreateIcoSphere()
    {
        CreateIcoSphereGameObject();
        //do whatever else you need to do with the sphere mesh
        RecalculateMesh();
    }

    void CreateIcoSphereGameObject()
    {
        icoSphere = new GameObject();
        icoSphereMeshFilter = icoSphere.AddComponent<MeshFilter>();
        icoSphereMesh = icoSphereMeshFilter.mesh;
        icoSphereMeshRenderer = icoSphere.AddComponent<MeshRenderer>();
        //need to set the material up top
        icoSphereMeshRenderer.material = icoSphereMaterial;
        icoSphere.transform.localScale = new Vector3(icoSphereSize, icoSphereSize, icoSphereSize);
        IcoSphere.Create(icoSphere);
    }

    void RecalculateMesh()
    {
        icoSphereMesh.RecalculateBounds();
        icoSphereMesh.RecalculateTangents();
        icoSphereMesh.RecalculateNormals();
    }
}