using UnityEngine;
using UnityEditor;

public class NoiseTexture : MonoBehaviour
{

    [MenuItem("Create/ShapeTexture")]
    static void getShapeTexture() // creates shape texture for clouds
    {
        Texture3D texture = new Texture3D(128, 128, 128, TextureFormat.ARGB32, false);
        texture.wrapMode = TextureWrapMode.Repeat;

        WorleyNoise worleyNoiseLF = new WorleyNoise(0, 2, 2);
        WorleyNoise worleyNoiseMF = new WorleyNoise(0, 5, 2);
        WorleyNoise worleyNoiseHF = new WorleyNoise(0, 8, 2);
        WorleyNoise worleyNoiseHHF = new WorleyNoise(0, 10, 2);

        for (int i = 0; i < 128; i++)
        {
            for (int j = 0; j < 128; j++)
            {
                for (int k = 0; k < 128; k++)
                {
                    float valR = worleyNoiseLF.Sample3D(i / 128f, j / 128f, k / 128f);//
                    float valG = worleyNoiseMF.Sample3D(i / 128f, j / 128f, k / 128f);
                    float valB = worleyNoiseHF.Sample3D(i / 128f, j / 128f, k / 128f);
                    float valA = worleyNoiseHHF.Sample3D(i / 128f, j / 128f, k / 128f);

                    texture.SetPixel(i, j, k, new Color(valR, valG, valB, valA));
                }
            }
        }
        texture.Apply();
        AssetDatabase.CreateAsset(texture, "Assets/CloudShape.asset");
    }

    [MenuItem("Create/DetailTexture")]
    static void getDetailTexture() // creates detail texture for clouds
    {
        Texture3D texture = new Texture3D(32, 32, 32, TextureFormat.ARGB32, false);
        texture.wrapMode = TextureWrapMode.Repeat;

        WorleyNoise worleyNoiseLF = new WorleyNoise(0, 2, 2);
        WorleyNoise worleyNoiseMF = new WorleyNoise(0, 5, 2);
        WorleyNoise worleyNoiseHF = new WorleyNoise(0, 8, 2);
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                for (int k = 0; k < 32; k++)
                {
                    float valR = worleyNoiseLF.Sample3D(i/32f, j/32f, k/32f);
                    float valG = worleyNoiseMF.Sample3D(i / 32f, j / 32f, k / 32f);
                    float valB = worleyNoiseHF.Sample3D(i / 32f, j / 32f, k / 32f);
                    texture.SetPixel(i, j, k, new Color(valR, valG, valB));
                }
            }
        }
        texture.Apply();
        AssetDatabase.CreateAsset(texture, "Assets/CloudDetail.asset");
    }

    [MenuItem("Create/WMTexture")]
    static void getWeatherMapTexture() // creates weather map texture for clouds
    {
        Texture2D texture = new Texture2D(512, 512);
        texture.wrapMode = TextureWrapMode.Repeat;

        PerlinNoise perlinNoiseLow = new PerlinNoise(0,1);
        PerlinNoise perlinNoiseHigh = new PerlinNoise(32,4);

        for (int i = 0; i < 512; i++)
        {
            for (int j = 0; j < 512; j++)
            {
             
                float valr = (perlinNoiseLow.Sample2D(i/512f,j/512f)+1f)/2f;
                float valg = (perlinNoiseHigh.Sample2D(i/512f,j/512f)+1f)/2f;
                float valb = 1;
                float vala = 0.5f;

                texture.SetPixel(i, j, new Color(valr, valg, valb, vala));
            }
        }

        texture.Apply();
        AssetDatabase.CreateAsset(texture, "Assets/WeatherMap.asset");
    }

    [MenuItem("Create/WaterTexture")]
    static void getWaterTexture() // creates perlin water texture for water
    {

        PerlinNoise noise = new PerlinNoise(16, 8);
        Texture2D texture = new Texture2D(128, 128);
        texture.wrapMode = TextureWrapMode.Repeat;

        Vector3[] powers = new Vector3[10];
        for (int k = 0; k < 10; k++)
        {
            powers[k] = new Vector3(Mathf.Pow(2, k), Mathf.Pow(0.9f, k), Mathf.Pow(0.5f, k));
           

        }
        for (int i = 0; i < 256; i++)
        {
            for (int j = 0; j < 256; j++)
            {
                Vector2 point = new Vector2(i, j);

                // apply different frequencies/amplitudes per channel

                float totalX = 0.0f;
                for (int k = 0; k < 10; k++)
                {
                    Vector2 evalPoint = powers[k].x * point;
                    totalX += powers[k].y * noise.Sample2D(evalPoint.x/256f,evalPoint.y/256f);

                }

                float totalY = 0.0f;
                for (int k = 0; k < 8; k++)
                {
                    Vector2 evalPoint = powers[k].x * point;
                    totalY += powers[k].z * noise.Sample2D(evalPoint.x / 256f, evalPoint.y / 256f);

                }

                float totalZ = 0.0f;
                for (int k = 0; k < 6; k++)
                {
                    Vector2 evalPoint = powers[k].x * point;
                    totalZ += powers[k].z * noise.Sample2D(evalPoint.x / 256f, evalPoint.y / 256f);

                }

                texture.SetPixel(i, j, new Color(totalX, totalY, totalZ));
            }
        }
       

        texture.Apply();
        AssetDatabase.CreateAsset(texture, "Assets/WaterTexture.asset");
    }

}