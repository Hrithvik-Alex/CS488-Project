using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class CloudRenderSettings : ScriptableObject
{

    public PerlinNoise noise = new PerlinNoise(32, 5);
    public int Octaves;
    public float Amp;
    public float gc;
    public float gd;
    public int height;
    public int width;
    public float ph;
    public float wh;
    public float wd;
}

