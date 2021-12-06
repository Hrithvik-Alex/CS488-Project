using UnityEngine;
using System.Collections;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Atmosphere : MonoBehaviour
{
    [SerializeField] public float atmosphereRadius = 2500;
    [SerializeField] public float seaLevel = 1000;
    [SerializeField] public float colorWLr = 440; 
    [SerializeField] public float colorWLg = 530; 
    [SerializeField] public float colorWLb = 700;
    [SerializeField] public float HDRexposure = 1;

    public Shader atmosphereShader;
    public Transform planetTransform;

    Material material;

    private void Awake()
    {
        if (Camera.main.depthTextureMode != DepthTextureMode.Depth)
            Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material = new Material(atmosphereShader);
        material.SetFloat("planetRadius", planetTransform.localScale.x);
        material.SetVector("planetPos", planetTransform.position);
        material.SetFloat("atmosphereRadius", atmosphereRadius);
        material.SetFloat("seaLevel", seaLevel);
        material.SetFloat("HDRexposure", HDRexposure);



        Vector3 scatter = new Vector3(Mathf.Pow(100/colorWLr,4),Mathf.Pow(100/colorWLg,4),Mathf.Pow(100/colorWLb,4));
        material.SetVector("scatterRatios", scatter);

        Graphics.Blit(source, destination, material);
    }

    //void Start()
    //{
    //    material = new Material(atmosphereShader);
    //    material.SetFloat("planetRadius", planetTransform.localScale.x);
    //    material.SetVector("planetPos", planetTransform.position);
    //    material.SetFloat("atmosphereRadius", atmosphereRadius);

    //}

}
