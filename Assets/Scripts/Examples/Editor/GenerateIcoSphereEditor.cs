using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GenerateIcoSphere))]
public class GenerateIcoSphereEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateIcoSphere myScript = (GenerateIcoSphere)target;
        if (GUILayout.Button("Create Sphere"))
        {
            myScript.CreateIcoSphere();
        }
    }
}