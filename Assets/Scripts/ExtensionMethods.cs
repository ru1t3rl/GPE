using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool Contains(this MeshRenderer[] array, MeshRenderer renderer)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == renderer)
            {
                return true;
            }
        }

        return false;
    }

    public static bool EqualMaterials(this List<MeshRenderer> array, MeshRenderer renderer)
    {

        for (int i = 0; i < array.Count; i++)
        {
            if (array[i].sharedMaterials.Length == renderer.sharedMaterials.Length)
            {
                bool equal = true;
                for (int j = 0; j < array[i].sharedMaterials.Length; j++)
                {
                    if (array[i].sharedMaterials[j] != renderer.sharedMaterials[j])
                    {
                        equal = false;
                        break;
                    }
                }

                if (equal)
                    return true;
            }
        }

        return false;
    }
}
