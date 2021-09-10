using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    public static GridPoint[,,] grid;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uv = new List<Vector2>();
}

public struct GridPoint
{
    public GridPoint(Vector3 position)
    {
        Position = position;
        On = false;
    }

    public Vector3 Position { get; set; }
    private bool On { get; set; }

    public override string ToString() => $"{Position}, On: {On}";
}

public static class Points
{
    /*      E ------ F
     *      |        |
     *      | A ------- B
     *      | |      |  |
     *      G | ---- H  |
     *        |         |
     *        C ------- D
    */

    // CORNERS
    public static GridPoint A;
    public static GridPoint B;
    public static GridPoint C;
    public static GridPoint D;
    public static GridPoint E;
    public static GridPoint F;
    public static GridPoint G;
    public static GridPoint H;

    // HALF-WAY POINTS
    public static Vector3 ab
    {
        get { return C.Position + new Vector3(0.5f, 1f, 0f); }
    }
    public static Vector3 ba
    {
        get { return C.Position + new Vector3(0.5f, 1f, 0f); }
    }
    public static Vector3 bd
    {
        get { return C.Position + new Vector3(1f, 0.5f, 0f); }
    }
    public static Vector3 db
    {
        get { return C.Position + new Vector3(1f, 0.5f, 0f); }
    }
    public static Vector3 dc
    {
        get { return C.Position + new Vector3(0.5f, 0f, 0f); }
    }
    public static Vector3 cd
    {
        get { return C.Position + new Vector3(0.5f, 0f, 0f); }
    }
    public static Vector3 ca
    {
        get { return C.Position + new Vector3(0f, 0.5f, 0f); }
    }
    public static Vector3 ac
    {
        get { return C.Position + new Vector3(0f, 0.5f, 0f); }
    }

    public static Vector3 ef
    {
        get { return C.Position + new Vector3(0.5f, 1f, 1f); }
    }
    public static Vector3 fe
    {
        get { return C.Position + new Vector3(0.5f, 1f, 1f); }
    }
    public static Vector3 fh
    {
        get { return C.Position + new Vector3(1f, 0.5f, 1f); }
    }
    public static Vector3 hf
    {
        get { return C.Position + new Vector3(1f, 0.5f, 1f); }
    }
    public static Vector3 hg
    {
        get { return C.Position + new Vector3(0.5f, 0f, 1f); }
    }
    public static Vector3 gh
    {
        get { return C.Position + new Vector3(0.5f, 0f, 1f); }
    }
    public static Vector3 ge
    {
        get { return C.Position + new Vector3(0f, 0.5f, 1f); }
    }
    public static Vector3 eg
    {
        get { return C.Position + new Vector3(0f, 0.5f, 1f); }
    }

    public static Vector3 fb
    {
        get { return C.Position + new Vector3(1f, 1f, 0.5f); }
    }
    public static Vector3 bf
    {
        get { return C.Position + new Vector3(1f, 1f, 0.5f); }
    }
    public static Vector3 ae
    {
        get { return C.Position + new Vector3(0f, 1f, 0.5f); }
    }
    public static Vector3 ea
    {
        get { return C.Position + new Vector3(0f, 1f, 0.5f); }
    }
    public static Vector3 hd
    {
        get { return C.Position + new Vector3(1f, 0f, 0.5f); }
    }
    public static Vector3 dh
    {
        get { return C.Position + new Vector3(1f, 0f, 0.5f); }
    }
    public static Vector3 cg
    {
        get { return C.Position + new Vector3(0f, 0f, 0.5f); }
    }
    public static Vector3 gc
    {
        get { return C.Position + new Vector3(0f, 0f, 0.5f); }
    }
}