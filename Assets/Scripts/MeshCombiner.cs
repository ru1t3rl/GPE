using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ru1t3rl.Rendering
{
    public class MeshCombiner : MonoBehaviour
    {
        [SerializeField] bool combineChildren;
        [SerializeField] bool combineList;
        [SerializeField] CombineInstance[] instances;

        private void Awake()
        {
            if (combineChildren)
            {
                CombineChildren();
            }
            else if (combineList)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i].Combine();
                }
            }
        }

        void CombineChildren()
        {
            MeshRenderer[] renderers = transform.GetComponentsInChildren<MeshRenderer>();
            MeshFilter[] meshFilters = transform.GetComponentsInChildren<MeshFilter>();

            List<MeshRenderer> differentTypes = new List<MeshRenderer>();
            List<List<MeshFilter>> meshes = new List<List<MeshFilter>>();

            // Loop through all the renderers
            for (int i = 0; i < renderers.Length; i++)
            {
                // Check if an object with the same material(s) is already in the list
                if (!differentTypes.EqualMaterials(renderers[i]))
                {
                    differentTypes.Add(renderers[i]);

                    meshes.Add(new List<MeshFilter>());
                    meshes[meshes.Count - 1].Add(meshFilters[i]);
                }
                else
                {
                    // Get the corresponding index and add the mesh to the same category
                    for (int j = 0; j < differentTypes.Count; j++)
                    {
                        if (differentTypes[j].sharedMaterials.Length == renderers[i].sharedMaterials.Length)
                        {
                            bool equal = true;

                            for (int k = 0; k < differentTypes[j].sharedMaterials.Length; k++)
                            {
                                if (differentTypes[j].sharedMaterials[k] != renderers[i].sharedMaterials[k])
                                {
                                    equal = false;
                                    break;
                                }
                            }

                            if (equal)
                            {
                                meshes[j].Add(meshFilters[i]);
                                break;
                            }
                        }
                    }
                }
            }

            // Combine the different meshes
            for (int i = 0; i < differentTypes.Count; i++)
            {
                new CombineInstance(differentTypes[i].gameObject.name, meshes[i].ToArray(), differentTypes[i].material).Combine();
            }
        }
    }

    [System.Serializable]
    public class CombineInstance
    {
        public string name;
        public MeshFilter[] meshes;
        UnityEngine.CombineInstance[] combineInstances;
        public Material material;

        MeshFilter finalMeshFilter;

        public CombineInstance(string name, MeshFilter[] meshes, Material material)
        {
            this.meshes = meshes;
            this.material = material;
            this.name = name;
        }

        public void Combine()
        {
            combineInstances = new UnityEngine.CombineInstance[meshes.Length];

            // Instantiate an empty gameobject
            GameObject combinedInstance = new GameObject();
            combinedInstance.name = "[Combined] " + name;

            // Add a meshfilter and -renderer to the object
            finalMeshFilter = combinedInstance.AddComponent<MeshFilter>();
            combinedInstance.AddComponent<MeshRenderer>().material = material;

            for (int iMesh = 0; iMesh < meshes.Length; iMesh++)
            {
                combineInstances[iMesh] = new UnityEngine.CombineInstance();
                combineInstances[iMesh].mesh = meshes[iMesh].sharedMesh;
                combineInstances[iMesh].transform = meshes[iMesh].transform.localToWorldMatrix;

                Object.Destroy(meshes[iMesh].gameObject);
            }

            // Create the combined mesh
            finalMeshFilter.mesh = new Mesh();
            finalMeshFilter.mesh.CombineMeshes(combineInstances);
        }
    }
}
