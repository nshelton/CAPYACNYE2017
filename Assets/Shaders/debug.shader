Shader "Custom/Debug Blit"
{
    Properties
    {
        _MainTex("-", 2D) = ""{}
        _HistoryOffset("-", float) = 0
    }

    CGINCLUDE

    sampler2D _MainTex;
    float _HistoryOffset;

    #include "UnityCG.cginc"

    float4 frag(v2f_img i) : SV_Target 
    {
        float2 newUV = frac(i.uv + float2(_HistoryOffset, 0));
        float3 c = tex2D(_MainTex, newUV).xyz;

        return float4(c, 1);
    }

    ENDCG

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            ENDCG
        }
    }
}
