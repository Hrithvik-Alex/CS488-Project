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
       
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        Vector2[] waterChecks = new Vector2[resolution * resolution];
        Color[] colors = new Color[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        // generate vertices
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = GetElevation(pointOnUnitSphere, ref colors[i], ref waterChecks[i]);
            }
        }

        // generate triangles
        for (int y = 0, triIndex = 0; y < resolution-1; y++)
        {
            for (int x = 0; x < resolution-1; x++, triIndex+=6)
            {
                int i = x + y * resolution;
                triangles[triIndex] = i;
                triangles[triIndex + 1] = i + resolution + 1;
                triangles[triIndex + 2] = i + resolution;

                triangles[triIndex + 3] = i;
                triangles[triIndex + 4] = i + 1;
                triangles[triIndex + 5] = i + resolution + 1;
          
            }
        }


        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.uv2 = waterChecks;
        mesh.RecalculateNormals();
    }

    Vector3 GetElevation(Vector3 point, ref Color color, ref Vector2 waterCheck)
    {
        float noiseVal = meshSettings.noise.Sample3D(point.x, point.y, point.z);
        float elevation = (noiseVal+1)/2f;
        if(elevation < 0.4f)
        {
            color = (meshSettings.oceanGradient.Evaluate(elevation/0.4f) + 2.0f*meshSettings.oceanGradient.Evaluate(0.2f *waveNoise(point)))/3f;
            waterCheck = new Vector2(1, 0);
        }
        else
        {
            waterCheck = new Vector2(0, 0);
            color = meshSettings.planetGradient.Evaluate(elevation);//
        }

        return point * (1 + Mathf.Max(0.4f,elevation));
    }

    float waveNoise(Vector3 point)
    {
        float total = 0.0f;
        for(int i = 0; i < meshSettings.waveOctaves; i++)
        {
            Vector3 evalPoint = Mathf.Pow(2, i) * point;
            total += Mathf.Pow(meshSettings.waveAmp, i) * (meshSettings.noise.Sample3D(evalPoint.x, evalPoint.y, evalPoint.z) + 1)/2f;

        }

        return total;
    }
}
