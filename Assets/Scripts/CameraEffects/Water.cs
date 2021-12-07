using UnityEngine;
using System.Collections;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Water : MonoBehaviour
{

    [SerializeField] public float seaLevel = 1400;

    public Shader waterShader;
    public Transform planetTransform;
    public Texture2D waterTexture;


    Material material;

    private void Awake()
    {
        if (Camera.main.depthTextureMode != DepthTextureMode.Depth) // allow depth buffer
            Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // creates material and passes in variables for water.shader
        material = new Material(waterShader);
        material.SetFloat("planetRadius", planetTransform.localScale.x);
        material.SetVector("planetPos", planetTransform.position);
        material.SetFloat("oceanRadius", seaLevel);
        material.SetTexture("perlinSampler", waterTexture);


        Graphics.Blit(source, destination, material);
    }

}