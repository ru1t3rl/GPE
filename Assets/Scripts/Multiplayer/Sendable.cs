using UnityEngine;

[System.Serializable]
public class Sendable
{
    public int id;
    public float x, y;
    public float previousX, previousY;
    public int frame;
    public int ping;

    public Vector2 moveDirection
    {
        get
        {
            return new Vector2(x + previousX, y + previousY).normalized;
        }
    }
}
