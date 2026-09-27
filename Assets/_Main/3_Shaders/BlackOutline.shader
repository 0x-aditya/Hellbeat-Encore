Shader "Custom/BlackOutline"
{
    Properties
    {
        [Header(Base Body)]
        _BaseColor ("Body Color", Color) = (1,1,1,1)
        _MainTex ("Base Texture (optional)", 2D) = "white" {}

        [Header(Outline)]
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.02
    }

    SubShader
    {
        // Self-contained: Pass 1 draws the body, Pass 2 draws the black outline shell.
        // Just assign this shader as your object's one and only material.
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }

        // ---------------- PASS 1: the object body ----------------
        Pass
        {
            Name "Body"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vertBody
            #pragma fragment fragBody

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct BodyAttributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct BodyVaryings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float2 uv          : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _MainTex_ST;
                float4 _OutlineColor;
                float  _OutlineWidth;
            CBUFFER_END

            BodyVaryings vertBody (BodyAttributes IN)
            {
                BodyVaryings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 fragBody (BodyVaryings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                Light mainLight = GetMainLight();

                half3 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).rgb;
                half3 albedo = tex * _BaseColor.rgb;

                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half3 ambient = SampleSH(normalWS);
                half3 litColor = albedo * (mainLight.color * NdotL + ambient);

                return half4(litColor, 1);
            }
            ENDHLSL
        }

        // ---------------- PASS 2: the black outline shell ----------------
        Pass
        {
            Name "Outline"
            // Different LightMode tag from Pass 1 so URP actually draws both passes
            // instead of only the first one it finds under a given tag.
            Tags { "LightMode"="SRPDefaultUnlit" }

            // Cull Front + extrude along the normal = classic inverted-hull outline.
            Cull Front
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _MainTex_ST;
                float4 _OutlineColor;
                float  _OutlineWidth;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float3 nrm = normalize(IN.normalOS);
                float3 displacedPos = IN.positionOS.xyz + nrm * _OutlineWidth;
                OUT.positionHCS = TransformObjectToHClip(displacedPos);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return half4(_OutlineColor.rgb, 1);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
