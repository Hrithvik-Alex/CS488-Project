Shader "Unlit/Water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Ocean Radius", Range(0,1)) = 0.2
        _DeepCol ("Deep Color", COLOR) = (0,0.07,0.449,1) // dark blue
        _ShallowCol ("Shallow Color", COLOR) = (0,0.816,1,1) // light blue
        _Smooth ("Smooth factor", Range(0,1)) = 0.5
        _SpecCol ("Specular Highlight Color", COLOR) = (0.9,0.9,0.9,1)
        _AnimSpeedX ("Anim Speed (X)", Range(0,4)) = 0.8
        _AnimSpeedY ("Anim Speed (Y)", Range(0,4)) = 0.1
        _AnimScale ("Anim Scale", Range(0,10)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "CommonFuncs.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
         
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 viewRay : TEXCOORD1;
                float4 vertex : SV_POSITION;
  
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float4 _MainTex_ST;
            float _Alpha;
            float4 _DeepCol;
            float4 _ShallowCol;
            float4 _SpecCol;
            float _Smooth;
            float _AnimSpeedX;
            float _AnimSpeedY;
            float _AnimScale;

            sampler2D perlinSampler;

            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // from https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html
                float3 viewRay = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewRay = mul(unity_CameraToWorld, float4(viewRay,0));
                return o;
            }

            
            float3 planetPos;
            float oceanRadius;
            float planetRadius;

            // from medium.com/@bgolus/normal-mapping-for-a-triplanar-shader-10bf39dca05a

            float3 rnmBlendUnpacked(float3 n1, float3 n2)
            {
                n1 += float3( 0,  0, 1);
                n2 *= float3(-1, -1, 1);
                return n1*dot(n1, n2)/n1.z - n2;
            }

        
            // from medium.com/@bgolus/normal-mapping-for-a-triplanar-shader-10bf39dca05a
            float3 getTriplanarNormal(float3 worldPos, float3 worldNormal, float scale, float2 offset, sampler2D noiseSample) {

                // Reoriented Normal Mapping blend
                // Triplanar uvs
                float2 uvX = worldPos.zy*scale + offset; // x facing plane
                float2 uvY = worldPos.xz*scale + offset; // y facing plane
                float2 uvZ = worldPos.xy*scale + offset; // z facing plane
                // Tangent space normal maps
                half3 tnormalX = UnpackNormal(tex2D(noiseSample, uvX));
                half3 tnormalY = UnpackNormal(tex2D(noiseSample, uvY));
                half3 tnormalZ = UnpackNormal(tex2D(noiseSample, uvZ));
                // Get absolute value of normal to ensure positive tangent "z" for blend
                half3 absVertNormal = abs(worldNormal);
                // Swizzle world normals to match tangent space and apply RNM blend
                tnormalX = rnmBlendUnpacked(half3(worldNormal.zy, absVertNormal.x), tnormalX);
                tnormalY = rnmBlendUnpacked(half3(worldNormal.xz, absVertNormal.y), tnormalY);
                tnormalZ = rnmBlendUnpacked(half3(worldNormal.xy, absVertNormal.z), tnormalZ);
                // Get the sign (-1 or 1) of the surface normal
                half3 axisSign = sign(worldNormal);
                // Reapply sign to Z
                tnormalX.z *= axisSign.x;
                tnormalY.z *= axisSign.y;
                tnormalZ.z *= axisSign.z;
                // Triblend normals and add to world normal
                float3 blend = saturate(pow(worldNormal, 4));
                blend /= dot(blend,1);
                half3 worldNormalFinal = normalize(
                    tnormalX.xyz * blend.x +
                    tnormalY.xyz * blend.y +
                    tnormalZ.xyz * blend.z +
                    worldNormal
                    );
	            return worldNormalFinal;
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float3 rayO = _WorldSpaceCameraPos;
                float3 rayD = i.viewRay / length(i.viewRay);

                // get value from depth buffer to check intersection distance 
                float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)) * length(i.viewRay);

                float2 oceanIntersect = ray_sphere_intersect(rayO, rayD, planetPos, oceanRadius);
                float oceanDepth = min(oceanIntersect.y, depth - oceanIntersect.x);

                
                if(oceanDepth > 0) { // ocean has been hit
                    float3 hitPoint = rayO + rayD * oceanIntersect.x;

                    //pick ocean color based on depth
                    float4 oceanColor = lerp(_ShallowCol, _DeepCol, saturate(Remap(oceanDepth, 0, 200, 0, 1)));
                    // get light direction normal vector
                    float3 lightSrc = (_WorldSpaceLightPos0.xyz - hitPoint) / length(_WorldSpaceLightPos0.xyz - hitPoint);

                    // change offset to swing back and forth over time
                    float2 waveSwing = float2(sin(_Time.y * _AnimSpeedY) * _AnimScale, sin(_Time.y * _AnimSpeedX) * _AnimScale);

                    // use perlin noise texture to get normal map
                    float3 waterNorm = getTriplanarNormal(hitPoint, normalize(hitPoint), 0.001, waveSwing, perlinSampler);
                    
                    // formula from https://en.wikipedia.org/wiki/Specular_highlight for specular highlight
				    float sExp = acos(dot(normalize(lightSrc - rayD), waterNorm)) / _Smooth;
				    float specularHighlight = inverseExp(pow(sExp,2));

                    oceanColor *= saturate(dot(normalize(hitPoint), lightSrc)); // diffuse lighting

                    oceanColor += specularHighlight * _SpecCol;

                    return col*_Alpha + oceanColor*(1-_Alpha);
                    
                } else {
                    return col;
                }
                
            }
            ENDCG
        }
    }
}
