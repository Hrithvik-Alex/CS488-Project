using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{

    [Range(2, 512)]
    public int resolution = 50;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    PlanetFace[] planetFaces;

    [SerializeField] MeshSettings meshSettings;

    private void OnValidate()
    {
        Generate();
    }

    void Initialize()
    {
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
            

        }
        planetFaces = new PlanetFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;
                meshObj.AddComponent<MeshRenderer>();

                List<Material> materialList = new List<Material>();
                materialList.Add(new Material(Shader.Find("Custom/Planet")));
                materialList.Add(new Material(Shader.Find("Unlit/Water")));

                meshObj.GetComponent<MeshRenderer>().sharedMaterials = materialList.ToArray();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            planetFaces[i] = new PlanetFace(meshSettings, meshFilters[i].sharedMesh, resolution, directions[i]);
        }

        //GameObject atmosphereObj = new GameObject("atmosphere");
        //atmosphereObj.AddComponent<Atmosphere>();
        //atmosphereObj.GetComponent<Atmosphere>().atmosphereShader = Shader.Find("Unlit/Atmosphere");
    }

    public void Generate()
    {
        Initialize();
        GenerateMesh();
        GetColors();
        GenerateClouds();
        GenerateAtmosphere();
    }

 
    void GenerateMesh()
    {
        foreach (PlanetFace face in planetFaces)
        {
            face.ConstructMesh();
        }
    }

    void GetColors()
    {

    }

    void GenerateClouds()
    {

    }

    void GenerateAtmosphere()
    {

    }
}