using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetFace
{
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;
    Noise noise;
    Gradient gradient;

    MeshSettings meshSettings;

    public PlanetFace(MeshSettings meshSettings, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.meshSettings = meshSettings;

        // create basis from up vector
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        Color[] colors = new Color[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        // generate vertices
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                int k = i * resolution + j;

                // perform sphere tesselation based on cube coordinates and normalize
                
                Vector3 pointX = (j / (float)(resolution - 1) - .5f) * 2 * axisA;
                Vector3 pointY = (i / (float)(resolution - 1) - .5f) * 2 * axisB;
                Vector3 point = (localUp + pointX + pointY).normalized;
                vertices[k] = GetElevation(point, ref colors[k]);
            }
        }


        int[] offsets = { 0, resolution + 1, resolution, 0, 1, resolution + 1 };
        // generate triangles
        for (int i = 0, triIndex = 0; i < resolution-1; i++)
        {
            for (int j = 0; j < resolution-1; j++)
            {
                int k = i * resolution + j;
                for(int l = 0; l < 6; l++, triIndex++)
                {
                    triangles[triIndex] = k + offsets[l];

                }

            }
        }


        mesh.Clear();
        // set values for the surface shader
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    Vector3 GetElevation(Vector3 point, ref Color color)
    {   // get elevation based on perlin noise, and assign color gradient value
        float elevation = 0;

        for(int i = 0; i < 5; ++i)
        {
            Vector3 evalPoint = Mathf.Pow(1.4f,i) * point;
            elevation += Mathf.Pow(0.6f, i) * (meshSettings.noises[i+1].Sample3D(evalPoint.x , evalPoint.y, evalPoint.z ) + 1f)/5f;//

        }


        // if certain height, add terraced mountains
        if(elevation > 0.5)
        {
            float steepNess = Mathf.Pow(0.9f, 1) * (meshSettings.noises[7].Sample3D(point.x, point.y, point.z) + 1f) / 5f;
            float intensity = 8f;
            elevation += terracingEffect(steepNess*intensity, 5)/intensity;
        }

        color = meshSettings.planetGradient.Evaluate(elevation);


        return point * (1 + elevation);
    }

    // from https://gamedev.stackexchange.com/questions/116205/terracing-mountain-features
    float terracingEffect(float x, float exp)
    {
        return Mathf.Round(x) + 0.5f * Mathf.Pow((2 * (x - Mathf.Round(x))), exp);
    }



    
}
