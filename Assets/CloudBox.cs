using UnityEngine;
using System.Collections;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CloudBox : MonoBehaviour
{
    [SerializeField] CloudRenderSettings cloudSettings;
    public Shader cloudShader;
    public Transform trans;
    Material material;
    public Texture shapeTexture;
    public Texture detailTexture;
    public Texture weatherMapTexture;


    private void Awake()
    {
        if (Camera.main.depthTextureMode != DepthTextureMode.Depth)
            Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material = new Material(cloudShader);
        material.SetVector("minB", trans.position - trans.localScale/2);
        material.SetVector("maxB", trans.position + trans.localScale/2);

        material.SetFloat("gc", cloudSettings.gc);
        material.SetFloat("gd", cloudSettings.gd);

        material.SetVector("lightCol", new Vector3(253f/256f, 184f/256f, 19f/256f));

        material.SetTexture("Shape", shapeTexture);
        material.SetTexture("Detail", detailTexture);
        material.SetTexture("WeatherMap", weatherMapTexture);


        Graphics.Blit(source, destination, material);
    }

    

}
