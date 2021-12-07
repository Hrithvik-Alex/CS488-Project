using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{

    [Range(2, 512)]
    public int resolution = 50;

    public MeshFilter[] meshFilters;
    PlanetFace[] planetFaces;
    bool meshesCreated = true;
    
    [SerializeField] MeshSettings meshSettings;

    private void OnValidate()
    {
        if (!meshesCreated)
        {
            meshFilters = new MeshFilter[6];
        }
        planetFaces = new PlanetFace[6];

        Vector3[] dirs = { // initialize up vectors for each face of cube-sphere
            new Vector3(0,1,0),
            new Vector3(0,-1,0),
            new Vector3(-1,0,0),
            new Vector3(1,0,0),
            new Vector3(0,0,1),
            new Vector3(0,0,-1),
        };

        for (int i = 0; i < 6; i++) // create 6 meshes for the sphere, so each mesh isn't too large
        {
            if (!meshesCreated)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;
                meshObj.AddComponent<MeshRenderer>();

                List<Material> materialList = new List<Material>();
                materialList.Add(new Material(Shader.Find("Custom/Planet")));

                meshObj.GetComponent<MeshRenderer>().sharedMaterials = materialList.ToArray();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            planetFaces[i] = new PlanetFace(meshSettings, meshFilters[i].sharedMesh, resolution, dirs[i]);
        }

        meshesCreated = true;

        foreach (PlanetFace face in planetFaces)
        {
            face.ConstructMesh();
        }

    }
  
}