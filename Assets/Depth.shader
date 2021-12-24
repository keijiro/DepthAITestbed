Shader "DepthAI/Depth"
{
    Properties
    {
        _MainTex("Raw16 Depth Map", 2D) = "black" {}
        _DepthMin("Min Depth (mm)", float) = 350
        _DepthMax("Max Depth (mm)", float) = 1500
    }

    CGINCLUDE

#pragma exclude_renderers gles

#include "UnityCG.cginc"

sampler2D _MainTex;
float _DepthMin, _DepthMax;

float3 Viridis(float x)
{
    float3x4 Viridis1 = float3x4
      (0.2777273272234177f, 0.1050930431085774f, -0.3308618287255563f, -4.634230498983486f,
       0.005407344544966578f, 1.404613529898575f, 0.214847559468213f, -5.799100973351585f,
       0.3340998053353061f, 1.384590162594685f, 0.09509516302823659f, -19.33244095627987f);

    float3x3 Viridis2 = float3x3
      (6.228269936347081f, 4.776384997670288f, -5.435455855934631f,
       14.17993336680509f, -13.74514537774601f, 4.645852612178535f,
       56.69055260068105f, -65.35303263337234f, 26.3124352495832f);

    float4 v = float4(1, x, x * x, x * x * x);
    return mul(Viridis1, v) + mul(Viridis2, v.yzw * v.w);
}

void Vertex(float4 position : POSITION,
            float2 texCoord : TEXCOORD0,
            out float4 outPosition : SV_Position,
            out float2 outTexCoord : TEXCOORD0)
{
    outPosition = UnityObjectToClipPos(position);
    outTexCoord = float2(texCoord.x, 1 - texCoord.y);
}

float4 Fragment(float4 position : SV_Position,
                float2 texCoord : TEXCOORD0) : SV_Target
{
    float d = tex2D(_MainTex, texCoord) * 65536;
    bool mask = d > 0;
    float x = saturate(1 - (d - _DepthMin) / (_DepthMax - _DepthMin));
    return float4(mask * Viridis(x), 1);
}

    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}
