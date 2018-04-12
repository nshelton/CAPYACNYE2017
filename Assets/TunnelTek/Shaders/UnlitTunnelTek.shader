// TunnelTek
//

Shader "SHELTRON/TunnelTek/Unlit"
{
    Properties
    {
        _MainTex ("-", 2D) = "white"{}

        [HDR] _Color  ("Color1", Color) = (1, 1, 1, 1)
        [HDR] _Color2 ("Color2", Color) = (0.5, 0.5, 0.5, 1)

        _Thickness ("LineWidth", Float) = 0.1 
    }

    CGINCLUDE

    #pragma multi_compile_fog

    #include "UnityCG.cginc"
    #include "TunnelTekCore.cginc"

    float _Thickness;

    sampler2D _MainTex;
    float4 _MainTex_ST;

    struct appdata
    {
        float4 vertex : POSITION;
        float2 texcoord0 : TEXCOORD0;
        float2 texcoord1 : TEXCOORD1;
        float3 normal : NORMAL;
    };

    struct v2f
    {
        float4 position : SV_POSITION;
        float3 normal : NORMAL;
        float2 texcoord : TEXCOORD;
        half4 color : COLOR;
        UNITY_FOG_COORDS(1)
    };

    float4 _Color;
    float4 _Color2;

    v2f vert(appdata v)
    {
        v.vertex.xyz = distortPos(v.vertex, v.texcoord1);

        v2f o;
        o.position = UnityObjectToClipPos(v.vertex);
        o.texcoord = TRANSFORM_TEX(v.texcoord0, _MainTex);
        o.normal = v.normal;
        o.color.xy = v.texcoord0.xy;
        UNITY_TRANSFER_FOG(o, o.position);

        return o;
    }

    half4 frag(v2f i) : SV_Target
    {
        half4 c = lerp(_Color, _Color2, i.color.y) * _Config.z ;
        UNITY_APPLY_FOG_COLOR(i.fogCoord, c, (half4)0);
        return c;
    }

    ENDCG

    SubShader
    {

        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Pass
        {
            
            Blend One One
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}