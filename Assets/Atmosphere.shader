Shader "Unlit/Atmosphere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {
        "RenderType"="Transparent"
        "Queue"= "Transparent"
        }
        Cull Off ZWrite Off ZTest Always

        Pass
        {

            Blend One Zero
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewVec : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _CameraDepthTexture;

            float2 quadraticRoots( float A, float B, float C)
			{
				// from A4 polyroots.cpp

				float D;
				float q;

				if( A == 0 ) {
					if( B == 0 ) {
						return float2(-1,-1);
					} else {	
						return float2(-C/B,-1);
					}
				} else {
					float2 roots;
					/*  Compute the discrimanant D=b^2 - 4ac */
					D = B*B - 4*A*C;
					if( D < 0 ) {
						return float2(-1,-1);
					} else {
						/* Two real roots */
						q = -( B + sign(B)*sqrt(D) ) / 2.0;
						roots.x = q / A;
						if( q != 0 ) {
							roots.y = C / q;
						} else {
							roots.y = roots.x;
						}
						return roots;
					}
				}
			}

			float3 planetPos;
			float planetRadius;
			float atmosphereRadius;
            float seaLevel;
            float3 scatterRatios;
            float HDRexposure;

            float Remap(float v, float lo, float ho, float ln, float hn) {
                return ln + (v-lo)*(hn-ln)/(ho-lo);
            }

            float SAT(float v)
            {
                return clamp(v, 0, 1);
            }

            float inverseExp(float v) {
                return exp(-v);
            }

            float3 inverseExp(float3 v) {
                return exp(-v);
            }


            float2 ray_sphere_intersect(float3 rayO, float3 rayD, float3 spherePos, float sphereRadius) {
				// modified ray sphere intersect from A4

	            float2 roots = quadraticRoots(dot(rayD,rayD), dot(2*rayD,rayO-spherePos),
				dot(rayO-spherePos,rayO-spherePos) - sphereRadius*sphereRadius);
	          
	            if(roots[0] > 0 && roots[1] > 0) {
		            float close = min(roots[0],roots[1]);
					float far = max(roots[0], roots[1]);
					return float2(close, far-close);
	            }   else if(roots[0] > 0 || roots[1] > 0) { // inside sphere
					float far = max(roots[0], roots[1]);
					return float2(0, far);
                }   else {
		            return float2(999999,0);
	            }

            }

            float particleDensity(float3 pos) {
                // less density as we go further from the surface
                float dist = length(pos - planetPos);
                float t = Remap(dist, seaLevel, atmosphereRadius, 0, 1);
                return inverseExp(t*10)*(1-t);
            }

            // as described in https://developer.nvidia.com/gpugems/gpugems2/part-ii-shading-lighting-and-shadows/chapter-16-accurate-atmospheric-scattering
            float outScattering(float3 rayO, float3 rayD, float len) {
                float totalLight = 0;
                float3 scatterPoint = rayO;
                for( int i = 0; i < 10; i++, scatterPoint += rayD*(len/10)) {
                    totalLight += particleDensity(scatterPoint) * (len/10);
                }

                return totalLight;
            }

            float3 inScattering(float3 rayO, float3 rayD, float len, float4 col) {
                // integral approximation approach for sampling light scattering at different points

                float3 totalLight = 0;
                float3 scatterPoint = rayO;
                float totalViewDensity = 0;
                for(int i = 0; i < 11; i++, scatterPoint += rayD*(len/10)) {
                    float3 lightSrc = (_WorldSpaceLightPos0.xyz - scatterPoint) / length(_WorldSpaceLightPos0.xyz - scatterPoint);
                    float2 sunIntersect = ray_sphere_intersect(scatterPoint, lightSrc, planetPos, atmosphereRadius );
                    float viewDensity = outScattering(scatterPoint, -rayD,  (len/10)*i);
                    totalViewDensity += viewDensity /10;
                    float3 totalOutScatteringTransmittance = inverseExp(( outScattering(scatterPoint,lightSrc, sunIntersect.y )
                    + viewDensity) * scatterRatios * 400);
                    totalLight += particleDensity(scatterPoint) * (len/10) * totalOutScatteringTransmittance;
                }

                 
                return col*inverseExp(totalViewDensity) + totalLight;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float3 viewVec = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 0, -1));
                o.viewVec = mul(unity_CameraToWorld, float4(viewVec,0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				float2 intersect = ray_sphere_intersect(_WorldSpaceCameraPos, i.viewVec/length(i.viewVec), planetPos, atmosphereRadius);

                float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)) * length(i.viewVec);

				float atmosphereDist = min(intersect.y, depth-intersect.x);
                if(atmosphereDist > 0) {
                    float3 atmosStart = _WorldSpaceCameraPos + (i.viewVec/length(i.viewVec))*intersect.x;
                    float3 lightVal = inScattering(atmosStart, i.viewVec/length(i.viewVec), atmosphereDist, col);

                    // HDR exposure
                    return float4(1.0,1.0,1.0,1.0) - exp(-HDRexposure*float4(lightVal, 0));
                } else {
                    return col;
                }

            }
            ENDCG
        }
    }
}
