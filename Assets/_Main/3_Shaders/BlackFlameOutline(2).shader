Shader "Custom/BlackFlameOutline"
{
    Properties
    {
        [Header(Base Body)]
        _BaseColor ("Body Color", Color) = (1,1,1,1)
        _MainTex ("Base Texture (optional)", 2D) = "white" {}

        [Header(Flame Look)]
        _FlameColor ("Flame Color", Color) = (0,0,0,1)
        _EmberColor ("Ember Glow Color", Color) = (0.15,0.02,0,0)
        _EmberIntensity ("Ember Glow Intensity", Range(0,5)) = 0

        [Header(Outline Shape)]
        _OutlineWidth ("Base Rim Width", Range(0.0, 0.05)) = 0.01
        _FlameLength ("Flame Tongue Length", Range(0.0, 1.0)) = 0.3
        _FlameSharpness ("Flame Tongue Sharpness", Range(1, 8)) = 3
        _FlameTongueScale ("Flame Tongue Scale", Range(0.5, 10)) = 2.5
        _FlameFlickerSpeed ("Flame Flicker / Rise Speed", Range(0, 5)) = 1.2

        [Header(Flame Pattern)]
        _NoiseScale ("Noise Scale", Range(1, 30)) = 8
        _FlameSpeed ("Flame Rise Speed", Range(0,5)) = 1.2
        _FlickerSpeed ("Flicker Speed", Range(0,10)) = 3
        _EdgeSoftness ("Edge Jaggedness", Range(0.01, 1)) = 0.35
        _Cutoff ("Flame Cutoff", Range(0,1)) = 0.4
    }

    SubShader
    {
        // Self-contained: Pass 1 draws the body, Pass 2 draws the flame outline shell.
        // Just assign this shader as your character's one and only material.
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline" }

        // ---------------- PASS 1: the actual character body ----------------
        // This must render first and occlude the outline shell everywhere
        // except at the silhouette edge. Without this pass (or an equivalent
        // opaque body material), the outline shell has nothing to hide behind
        // and the flame pattern covers the whole model instead of just the rim.
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

        // ---------------- PASS 2: the black flame outline shell ----------------
        Pass
        {
            Name "BlackFlameOutline"
            Tags { "LightMode"="UniversalForward" }

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
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS   : SV_POSITION;
                float2 uv            : TEXCOORD0;
                float3 positionOSraw : TEXCOORD1;
                float  flameStrength : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _FlameColor;
                float4 _EmberColor;
                float  _EmberIntensity;
                float  _OutlineWidth;
                float  _FlameLength;
                float  _FlameSharpness;
                float  _FlameTongueScale;
                float  _FlameFlickerSpeed;
                float  _NoiseScale;
                float  _FlameSpeed;
                float  _FlickerSpeed;
                float  _EdgeSoftness;
                float  _Cutoff;
            CBUFFER_END

            // ---------- cheap value noise / fbm, no textures needed ----------
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float valueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 p)
            {
                float total = 0.0;
                float amp = 0.5;
                [unroll]
                for (int i = 0; i < 4; i++)
                {
                    total += valueNoise(p) * amp;
                    p *= 2.02;
                    amp *= 0.5;
                }
                return total;
            }
            // -------------------------------------------------------------

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 nrm = normalize(IN.normalOS);

                // Sample a lower-frequency noise field per-vertex to decide how far
                // each point on the surface should shoot outward. Most of the field
                // stays low (near the base rim) with occasional peaks (long tongues).
                float2 seed = float2(IN.positionOS.x + IN.positionOS.z, IN.positionOS.y) * _FlameTongueScale;
                seed.y -= _Time.y * _FlameFlickerSpeed; // tongues drift/rise over time

                float n = fbm(seed);
                n = pow(saturate(n), _FlameSharpness); // bias toward a few sharp spikes, mostly near 0

                float extrude = _OutlineWidth + n * _FlameLength;
                float3 displacedPos = IN.positionOS.xyz + nrm * extrude;

                OUT.positionOSraw = IN.positionOS.xyz;
                OUT.uv = IN.uv;
                OUT.flameStrength = n;
                OUT.positionHCS = TransformObjectToHClip(displacedPos);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Sample flame pattern in object space so it wraps consistently
                // regardless of the mesh's UV layout, and rises along local +Y.
                float2 flameUV = float2(IN.positionOSraw.x + IN.positionOSraw.z, IN.positionOSraw.y);
                flameUV *= _NoiseScale;
                flameUV.y -= _Time.y * _FlameSpeed;

                float n = fbm(flameUV);

                // Animate the cutoff threshold per-pixel for a flicker effect.
                float flicker = (valueNoise(float2(IN.uv.x * 10.0, _Time.y * _FlickerSpeed)) - 0.5) * 0.3;
                float threshold = _Cutoff + flicker;

                float edge = smoothstep(threshold - _EdgeSoftness, threshold + _EdgeSoftness, n);

                // Fray the tips: the farther a point was pushed out (higher flameStrength),
                // the harder it is to stay solid, so tongues thin out toward their ends.
                float tipFade = saturate(1.25 - IN.flameStrength * 0.9);
                edge *= tipFade;

                clip(edge - 0.5); // jagged flame-tip silhouette instead of a smooth line

                half3 col = _FlameColor.rgb + _EmberColor.rgb * _EmberIntensity * n;
                return half4(col, _FlameColor.a);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
