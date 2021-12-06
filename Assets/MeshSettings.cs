using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class MeshSettings : ScriptableObject
{
 
    public PerlinNoise noise = new PerlinNoise(0,3);
    public PerlinNoise waterNoise = new PerlinNoise(0, 9);

    public Gradient planetGradient;
    public Gradient oceanGradient;
    public int waveOctaves;
    public float waveAmp;

}
