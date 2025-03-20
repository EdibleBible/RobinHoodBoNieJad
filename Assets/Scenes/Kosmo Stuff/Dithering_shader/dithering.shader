Shader "Custom/dithering"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "Dithered Light Pass"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv         : TEXCOORD0;
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            // Macierz Bayer 4x4 (dla ditheringu)
            static const float4x4 bayerMatrix = float4x4(
                0.0,  8.0,  2.0, 10.0,
                12.0, 4.0, 14.0,  6.0,
                3.0, 11.0,  1.0,  9.0,
                15.0, 7.0, 13.0,  5.0
            ) / 16.0; // Normalizacja 0-1

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS);
                OUT.normalWS   = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv         = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Pobierz światło główne
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);
                float lightIntensity = saturate(dot(IN.normalWS, lightDir));

                // Pobranie pozycji piksela i ditheringu
                int2 pixelPos = int2(IN.positionCS.xy) % 4; // Modulo 4 dla macierzy Bayera
                float threshold = bayerMatrix[pixelPos.x][pixelPos.y];

                // Dithering: jeśli intensywność światła < threshold → czarny
                float ditheredLight = lightIntensity > threshold ? 1.0 : 0.0;

                return half4(ditheredLight.xxx, 1.0); // Wynik w skali szarości
            }
            ENDHLSL
        }
    }
}