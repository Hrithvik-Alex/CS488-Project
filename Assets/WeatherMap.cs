using UnityEngine;
using System.Collections;

public class WeatherMap : MonoBehaviour
{
    [SerializeField] CloudRenderSettings cloudSettings;
    Color[] mapValues;
    public ComputeShader shader;
    public Shader cloudShader;
    ComputeBuffer dataBuffer;
    bool hasRun;
    float[] probabilities;

    void OnValidate()
    {
        mapValues = new Color[cloudSettings.height * cloudSettings.width];
        for(int i = 0; i < cloudSettings.height; i++)
        {
            for(int j = 0; j < cloudSettings.width; j++)
            {
                int k = cloudSettings.width * i + j;
                //mapValues[k].r = Step((cloudSettings.noise.Evaluate(new Vector3(i*i+j,j,i)) + 1f) / 2f, 0.5f);
                //mapValues[k].g = (cloudSettings.noise.Evaluate(new Vector3(i,j,j*j+1)) + 1f)/2f;
                mapValues[k].b = 1;
                mapValues[k].a = 0.5f;
            }
        }
        hasRun = false;
        //runShader();
        probabilities = getWeatherMapCPU();

        Material material = GetComponentInChildren<MeshRenderer>().sharedMaterial;
        Texture2D texture = new Texture2D(cloudSettings.height, cloudSettings.width);

        for (int i = 0; i < cloudSettings.height; i++)
        {
            for (int j = 0; j < cloudSettings.width; j++)
            {
                int k = i * cloudSettings.width + j;
                texture.SetPixel(i,j,new Color(probabilities[k], probabilities[k], probabilities[k]));
            }
        }
        texture.Apply();

        material.SetTexture("_MainTex", texture);

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Material material = new Material(cloudShader);

        Graphics.Blit(source, destination, material);
    }


    void Update()
    {
       
        
    }

    void runShader()
    {
        int kernel = shader.FindKernel("CSMain");

        dataBuffer = new ComputeBuffer(cloudSettings.height * cloudSettings.width, 16);
        dataBuffer.SetData(mapValues);
        shader.SetFloat("gc", cloudSettings.gc);
        shader.SetBuffer(kernel, "Result", dataBuffer);
    }

    public Color[] getWeatherMapGPU()
    {
        if(!hasRun)
        {
            dataBuffer.GetData(mapValues);
            hasRun = true;
            dataBuffer.Dispose();
        }

        return mapValues;
    }

    public float[] getWeatherMapCPU()
    {
        float[] probs = new float[cloudSettings.height * cloudSettings.width];
        for (int i = 0; i < cloudSettings.height; i++)
        {
            for (int j = 0; j < cloudSettings.width; j++)
            {
                int k = i * cloudSettings.width + j;
                probs[k] = weatherProbability(mapValues[k]);
            }
        }
        return probs;
    }



    public float weatherProbability(Color color)
    {
        return Mathf.Max(color.r, SAT(cloudSettings.gc - 0.5f) * color.g * 2);
    }

    float SAT(float v)
    {
        return Mathf.Clamp(v, 0, 1);
    }

    float Step(float v, float threshold)
    {
        if(v < threshold)
        {
            return 0;
        } else
        {
            return 1;
        }
    }
}
