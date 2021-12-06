Shader "Custom/Planet"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _AnimSpeedX ("Anim Speed (X)", Range(0,20)) = 1.3
        _AnimSpeedY ("Anim Speed (Y)", Range(0,20)) = 2.7
        _AnimScale ("Anim Scale", Range(0,10)) = 0.03
        _AnimTiling ("Anim Tiling", Range(0,20)) = 8
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float2 uv2_WaterTex : TEXCOORD1;
            float4 vcolor : COLOR; // vertex color
            float4 _Time;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _AnimSpeedX;
        float _AnimSpeedY;
        float _AnimScale;
        float _AnimTiling;
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            // fixed4 c = (IN.uv2_WaterTex.x==1)? tex2D (_MainTex, float2(sin((IN.uv_MainTex[0] + IN.uv_MainTex[1]) * _AnimTiling + _Time.y * _AnimSpeedX) * _AnimScale, cos((IN.uv_MainTex[0] - IN.uv_MainTex[1]) * _AnimTiling + _Time.y * _AnimSpeedY) * _AnimScale)) * _Color * IN.vcolor : tex2D (_MainTex, IN.uv_MainTex) * _Color * IN.vcolor;
            // fixed4 c = tex2D (_MainTex, float2(sin((IN.uv_MainTex[0] + IN.uv_MainTex[1]) * _AnimTiling + _Time.y * _AnimSpeedX) * _AnimScale, cos((IN.uv_MainTex[0] - IN.uv_MainTex[1]) * _AnimTiling + _Time.y * _AnimSpeedY) * _AnimScale)) * _Color * IN.vcolor;
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color * IN.vcolor;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
