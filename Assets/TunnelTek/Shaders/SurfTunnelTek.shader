Shader "SHELTRON/TunnelTek/Surface" {

    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        [HDR] _Color  ("Color1", Color) = (1, 1, 1, 1)
        [HDR] _Color2 ("Color2", Color) = (0.5, 0.5, 0.5, 1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert fullforwardshadows
        #pragma target 3.0
        #pragma multi_compile_fog

        #include "UnityCG.cginc"
        #include "TunnelTekCore.cginc"

        half _Glossiness;
        half _Metallic;

        float4 _Color;
        float4 _Color2;

        struct Input
        {
            float2 texcoord;
             half4 color : COLOR;
        };

        void vert(inout appdata_full v)
        {
            v.vertex.xyz = distortPos(v.vertex.xyz, v.texcoord1);
            v.normal = distortNormal(v.normal);

            v.color.y = 0.5 + 0.5 * sin(v.texcoord1.y * 3.1415 * 2.0 * _Config.y);
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {

            // Albedo comes from a texture tinted by color
            fixed4 c = lerp(_Color, _Color2, IN.color.y);
            o.Albedo = c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = o.Albedo * _Config.z;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
