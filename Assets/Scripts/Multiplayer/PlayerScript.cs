using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField]
    float speed = 2;
    public float Speed => speed;
    public int id;

    Vector3 previousPosition;
    public Vector3 PreviousPosition => previousPosition;

    public void SetPosition(Vector3 position)
    {
        previousPosition = transform.position;
        transform.position = position;
    }

    public void TranslatePlayer(Vector3 movement)
    {
        previousPosition = transform.position;
        transform.position += movement;
    }

    public void TranslatePlayer(float x, float y, float z)
    {
        previousPosition = transform.position;
        Vector3 movement = new Vector3(x, y, z);
        transform.position += movement;
    }
}
