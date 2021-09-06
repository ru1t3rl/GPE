using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTest : MonoBehaviour
{
    float radius = 1;
    float vertices = 10;

    float angle = 0;
    [SerializeField] float drag;

    Vector3 pos;

    void Awake()
    {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        angle++;

        pos.x = radius / 2 * Mathf.Sin(angle/drag);
        pos.y = radius / 2 * Mathf.Cos(angle/drag);

        transform.position = pos;
    }
}
