using UnityEngine;
using UnityEditor;

public class NoiseTexture : MonoBehaviour
{

    [MenuItem("Create/ShapeTexture")]
    static void getShapeTexture()
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
    static void getDetailTexture()
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

    static float Step(float v, float threshold)
    {
        if (v < threshold)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    [MenuItem("Create/WMTexture")]
    static void getWeatherMapTexture()
    {
        Texture2D texture = new Texture2D(512, 512);
        //texture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;//
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

    //private void NormalizeArray(float[,] arr)
    //{

    //    float min = float.PositiveInfinity;
    //    float max = float.NegativeInfinity;

    //    for (int y = 0; y < height; y++)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {

    //            float v = arr[x, y];
    //            if (v < min) min = v;
    //            if (v > max) max = v;

    //        }
    //    }

    //    for (int y = 0; y < height; y++)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            float v = arr[x, y];
    //            arr[x, y] = (v - min) / (max - min);
    //        }
    //    }

    //}
}