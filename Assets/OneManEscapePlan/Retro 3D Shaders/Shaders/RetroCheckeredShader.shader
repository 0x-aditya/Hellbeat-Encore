/// © 2018-2022 Kevin Foley
/// For distribution only on the Unity Asset Store
/// Terms/EULA: https://unity3d.com/legal/as_terms

Shader "OneManEscapePlan/RetroCheckeredShader" {
	Properties
	{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
		[IntRange]_CheckerSize("Checker Size", Range(1, 10)) = 1
		_CheckerStrength("Checker Strength", Range(0, 1)) = .25
		_BrightnessThreshold("Brightness Threshold", Range(0, 2)) = .85
		_Color("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" // for UnityObjectToWorldNormal, appdata_base
			#include "UnityLightingCommon.cginc" // for _LightColor0
			#pragma multi_compile_fwdbase //needed to receive shadows
			#include "AutoLight.cginc" //for SHADOW_COORDS (to receive shadows)

			struct v2f
			{
				float2 uv : TEXCOORD0;
				SHADOW_COORDS(1) // store shadows in TEXCOORD1
				fixed4 diff : COLOR0; // diffuse lighting color
				fixed3 ambient : COLOR1;
				float4 pos : POSITION;
				float3 worldPos : TEXCOORD2;
			};
			sampler2D _MainTex;
			int _CheckerSize;
			float _CheckerStrength;
			float _BrightnessThreshold;
			fixed4 _Color;

			v2f vert(appdata_base v)
			{
				v2f o;
				o.uv = v.texcoord;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				// get vertex normal in world space
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				// calculate lighting
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0;
				o.ambient = ShadeSH9(half4(worldNormal,1));
				TRANSFER_SHADOW(o);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;

				//scale checker size for render resolution
				int finalCheckerSize = (int)_ScreenParams.y / 240;
				if (finalCheckerSize < 1) finalCheckerSize = 1;
				finalCheckerSize *= _CheckerSize;

				// checker value will be negative for blocks of pixels
				// in a checkerboard pattern
				float4 screenPos = i.pos;
				screenPos.xy = floor(screenPos.xy * (1 / (float)finalCheckerSize)) * 0.5;
				float checker = -frac(screenPos.r + screenPos.g);

				// fade received shadow with distance
				float zDist = dot(_WorldSpaceCameraPos - i.worldPos, UNITY_MATRIX_V[2].xyz);
				float fadeDist = UnityComputeShadowFadeDistance(i.worldPos, zDist);
				half  shadowFade = UnityComputeShadowFade(fadeDist);
				//apply received shadow
				float shadow = saturate(lerp(SHADOW_ATTENUATION(i), 1.0, shadowFade));
				fixed3 lighting = i.diff * shadow + i.ambient;

				//calculate brightness of lighting
				float bright = lighting.r * 0.2126 + lighting.g * .7152 + lighting.b * .0722;
				//only checker shadowed areas
				if (checker < 0 && bright < _BrightnessThreshold) {
					col *= (1 - _CheckerStrength);
				}

				// multiply by lighting
				col.rgb *= lighting;
				return col;
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}