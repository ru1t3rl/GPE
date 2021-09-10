using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMesh : MonoBehaviour
{
    [SerializeField] Vector3 gridSize = new Vector3(10, 10, 10);
    [SerializeField] Material material;
    [SerializeField] float zoom = 1f;
    [SerializeField] float noiselimit = .5f;

    Mesh mesh;
}
