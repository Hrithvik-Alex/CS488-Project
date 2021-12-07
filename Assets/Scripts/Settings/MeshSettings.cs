using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class MeshSettings : ScriptableObject
{
    // pre-create perlin noises of different frequencies to use for terrain generation
    public PerlinNoise[] noises = { new PerlinNoise(0, 1),
        new PerlinNoise(10, 2),
        new PerlinNoise(20, 3),
        new PerlinNoise(30, 4),
        new PerlinNoise(40, 5),
        new PerlinNoise(50, 6),
        new PerlinNoise(70, 7),
        new PerlinNoise(80, 8),
        new PerlinNoise(90, 9),
        new PerlinNoise(100, 10),
    };

    // high freq perlin noise for water texture
    public PerlinNoise waterNoise = new PerlinNoise(0, 9);

    public Gradient planetGradient;
    public Gradient oceanGradient;

}
