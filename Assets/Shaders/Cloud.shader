Shader "Unlit/Cloud"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // no depth testing since transparent shader
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
                float4 vertex : SV_POSITION;
                float3 viewRay : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float4 _MainTex_ST;

            float3 minB;
            float3 maxB;

            float gc;
            float gd;

            Texture3D<float4> Shape;
            Texture3D<float4> Detail;
            Texture2D<float4> WeatherMap;
            SamplerState samplerShape;
            SamplerState samplerDetail;
            SamplerState samplerWeatherMap;

            float4 _LightColor0;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                //get view vector from camera
                float3 viewRay = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewRay = mul(unity_CameraToWorld, float4(viewRay,0));
                return o;
            }


            // created with equation derived from Haggstrom's paper

            float cloudDensity(float3 pos) {

                float3 cloudspace = (pos+0.25*(maxB-minB)*_Time.y)*0.001;
                float4 shapeSample = Shape.Sample(samplerShape, cloudspace, 0);
                float4 detailSample = Detail.Sample(samplerDetail, cloudspace, 0);
                float4 weatherMapSample = WeatherMap.Sample(samplerWeatherMap, cloudspace, 0);

                float WM_c = max(weatherMapSample.r, SAT(gc - 0.5)*weatherMapSample.g*2);
                float ph =(pos.y - minB.y)/(maxB - minB).y;
    
                float SR_b = SAT(Remap(ph,0,0.07,0,1));
                float SR_t = SAT(Remap(ph,0.5,1,1,0));
                float SA = SR_b * SR_t;

                float DR_b = ph*SAT(Remap(ph,0,0.15,0,1));
                float DR_t = SAT(Remap(ph,0.9,1.0,1,0));
                float DA = gd*DR_b*DR_t*weatherMapSample.a*2;

                float SN_sample = Remap(shapeSample.r,(shapeSample.g*0.625 + shapeSample.b*0.25 + shapeSample.a*0.125)-1,1,0,1);
                float SN_nd = SAT(Remap(SN_sample*SA,1-gc*WM_c,1,0,1));

                float DN_fbm = detailSample.r*0.625 + detailSample.g*0.25 + detailSample.b * 0.125;
                float DN_mod = 0.35*inverseExp(gc*0.75)*L_i(DN_fbm, 1-DN_fbm,SAT(ph*5));

                return SAT(Remap(SN_nd, DN_mod,1,0,1))*DA;
            }   

            // returns total density by sampling points in the ray
            float2 rayMarch(float3 rayO, float3 rayD, float2 intersect, float depth) {
                float totalDens = 0;
                float totalTrans = 1;
                [unroll]
                for(float travel = 0, i = 0; travel < min(intersect.y, depth - intersect.x) && i < 15; travel += intersect.y/10, i++) {
                    float3 currentPos = rayO + rayD*(intersect.x+travel);
                    float density = cloudDensity(currentPos);
                    if(density>0) {

                        // do similar ray marching towards light source to calculate how much light has been scattered
                        float3 pos = currentPos;
                        // normal vector towards light source
                        float3 lightSrc = (_WorldSpaceLightPos0.xyz - pos) / length(_WorldSpaceLightPos0.xyz - pos);
                        float2 lightIntersect = rayBoxInt(pos, lightSrc, minB, maxB);
                        float lightDens = 0;
                        for(int travelLight = 0; travelLight < 10; travelLight ++) {
                            pos += lightSrc*lightIntersect.y/10;
                            lightDens += cloudDensity(pos)*lightIntersect.y/10;
                        }

                        // calculate total density at point along with light transmittance   
                        totalDens += density*intersect.y/10*totalTrans*(0.5*inverseExp(lightDens*0.5) + 0.5);
                        totalTrans *= inverseExp(density*intersect.y/10*0.25);
                    }
                    
                }
                return float2(totalDens, totalTrans);
            }



            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float3 col = tex2D(_MainTex, i.uv);
                float3 rayO = _WorldSpaceCameraPos;
                float3 rayD = i.viewRay / length(i.viewRay);

                // get value from depth buffer to check intersection distance 
                float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)) * length(i.viewRay);
                float2 intersect = rayBoxInt(rayO, rayD, minB, maxB);
                
                float2 marchResults = rayMarch(rayO, rayD, intersect, depth);

                float3 lightSrc = (_WorldSpaceLightPos0.xyz - rayO+rayD*intersect.x) / length(_WorldSpaceLightPos0.xyz - rayO+rayD*intersect.x);
                marchResults.x *= dot(rayD, lightSrc);

                float3 finalCol = col*marchResults.y + marchResults.x*_LightColor0.xyz;
                return float4(finalCol, 0);
            

            }
            ENDCG
        }
    }
}
