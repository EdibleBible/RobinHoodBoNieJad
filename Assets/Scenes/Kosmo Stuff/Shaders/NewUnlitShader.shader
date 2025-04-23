Shader "Unlit/NewUnlitShader"
{
    Properties {}
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "ShowShadowMap"
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MyShadowMap);
            SAMPLER(sampler_MyShadowMap);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Tu możesz debugować raw depth:
                float rawDepth = SAMPLE_TEXTURE2D(_MyShadowMap, sampler_MyShadowMap, input.uv).r;

                // Przekształcenie do grayscale dla debugowania
                return half4(rawDepth.xxx, 1.0);
            }

            ENDHLSL
        }
    }
}
