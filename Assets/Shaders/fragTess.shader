// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BasicTessellation" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Offset ("Offset", Float) = 10
        _Tess ("Tessellation", Float) = 2
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
       
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma hull tessBase
            #pragma domain basicDomain
            #pragma target 5.0
            #include "UnityCG.cginc"
            #define INTERNAL_DATA
   
            sampler2D _MainTex;
           
            float4 _MainTex_ST;
            float _Offset;
            float _Tess;
           
            struct v2f{
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };
           
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
           
            struct inputControlPoint{
                float4 position : WORLDPOS;
                float4 texcoord : TEXCOORD0;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };
           
            struct outputControlPoint{
                float3 position : BEZIERPOS;            
            };
           
            struct outputPatchConstant{
                float edges[3]        : SV_TessFactor;
                float inside        : SV_InsideTessFactor;
               
                float3 vTangent[4]    : TANGENT;
                float2 vUV[4]         : TEXCOORD;
                float3 vTanUCorner[4] : TANUCORNER;
                float3 vTanVCorner[4] : TANVCORNER;
                float4 vCWts          : TANWEIGHTS;
            };
           
           
            outputPatchConstant patchConstantThing(InputPatch<inputControlPoint, 3> v){
                outputPatchConstant o;
               
                o.edges[0] = _Tess;
                o.edges[1] = _Tess;
                o.edges[2] = _Tess;
                o.inside = _Tess;
               
                return o;
            }
           
            // tessellation hull shader
            [domain("tri")]
            [partitioning("fractional_odd")]
            [outputtopology("triangle_cw")]
            [patchconstantfunc("patchConstantThing")]
            [outputcontrolpoints(3)]
            inputControlPoint tessBase (InputPatch<inputControlPoint,3> v, uint id : SV_OutputControlPointID) {
                return v[id];
            }
           
            #endif // UNITY_CAN_COMPILE_TESSELLATION
           
            v2f vert (appdata_tan v){
                v2f o;
               
                o.texcoord = v.texcoord;
                o.pos = v.vertex;
               
                return o;
            }
           
            v2f displace (appdata_tan v){
                v2f o;        
               
                o.texcoord = TRANSFORM_TEX (v.texcoord, _MainTex);
               
                float localTex = tex2Dlod(_MainTex, float4(o.texcoord,0,0)).r;
                v.vertex.y += localTex.r * _Offset;
               
                o.pos = UnityObjectToClipPos(v.vertex);
               
                return o;
            }
           
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
           
            // tessellation domain shader
            [domain("tri")]
            v2f basicDomain (outputPatchConstant tessFactors, const OutputPatch<inputControlPoint,3> vi, float3 bary : SV_DomainLocation) {
                appdata_tan v;
                v.vertex = vi[0].position*bary.x + vi[1].position*bary.y + vi[2].position*bary.z;
                v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                v.texcoord = vi[0].texcoord*bary.x + vi[1].texcoord*bary.y + vi[2].texcoord*bary.z;
                v2f o = displace( v);
//                v2f o = vert_surf (v);
                return o;
            }
           
            #endif // UNITY_CAN_COMPILE_TESSELLATION
           
           
   
            float4 frag(in v2f IN):COLOR{
                return tex2D (_MainTex, IN.texcoord);
            }
           
            ENDCG
        }
    }
}
 