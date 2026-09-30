/// © 2018-2022 Kevin Foley
/// For distribution only on the Unity Asset Store
/// Terms/EULA: https://unity3d.com/legal/as_terms

//Simulates 2-bit color (4-shade grayscale), with optional checker-shading to create 'additional' shades
//for example, if our 4 shades are white, light grey, dark grey, black, we can create a "medium grey" by
//checking light-grey and dark-grey

//TODO: overhaul using https://docs.unity3d.com/ScriptReference/MaterialPropertyDrawer.html
Shader "OneManEscapePlan/RetroGreyscaleCheckeredShader" {
	Properties
	{
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
		[Toggle]_CheckerEnabled("Checker-shading enabled", Float) = 1 
		[IntRange]_CheckerSize("Checker Size", Range(1, 10)) = 1
		_MinBrightness("Min Brightness", Range(0, 1)) = 0
		_MaxBrightness("Max Brightness", Range(0, 1)) = 1
	}
	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" // for UnityObjectToWorldNormal
			#include "Lighting.cginc"
			#include "UnityLightingCommon.cginc" // for _LightColor0
			#pragma multi_compile_fwdbase
			#include "AutoLight.cginc" //needed to receive shadows

			struct v2f
			{
				float2 uv : TEXCOORD0;
				SHADOW_COORDS(1) // store shadows in TEXCOORD1
				fixed4 diff : COLOR0; // diffuse lighting color
				fixed3 ambient : COLOR1;
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD2;
			};
			int _CheckerSize;
			int _CheckerEnabled;
			fixed _MinBrightness;
			fixed _MaxBrightness;

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
				o.ambient = ShadeSH9(half4(worldNormal, 1));
				TRANSFER_SHADOW(o);
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);

				// fade received shadow with distance
				float zDist = dot(_WorldSpaceCameraPos - i.worldPos, UNITY_MATRIX_V[2].xyz);
				float fadeDist = UnityComputeShadowFadeDistance(i.worldPos, zDist);
				half  shadowFade = UnityComputeShadowFade(fadeDist);
				//apply received shadow
				float shadow = saturate(lerp(SHADOW_ATTENUATION(i), 1.0, shadowFade));
				// multiply color by lighting
				fixed3 lighting = i.diff * shadow + i.ambient;
				col.rgb *= lighting;

				//variable representing the closest shade of grey in a 4-bit (16-color) spectrum
				float fourBit = round(col.r * 4) / 4;
				
				//convert the current color to the closest shade of grey in a 2-bit (4-color) spectrum
				col.r = round(col.r * 2) / 2;
				col.g = col.r;
				col.b = col.r;

				//scale checker size for render resolution
				int finalCheckerSize = (int)_ScreenParams.y / 240;
				if (finalCheckerSize < 1) finalCheckerSize = 1;
				finalCheckerSize *= _CheckerSize;

				if (_CheckerEnabled == 1) {
					// checker value will be negative for blocks of pixels in a checkerboard pattern
					float4 screenPos = i.pos;
					screenPos.xy = floor(screenPos.xy * (1 / (float)finalCheckerSize)) * 0.5;
					float checker = -frac(screenPos.r + screenPos.g);

					//this creates additional intermediate shades using checkers. For example, if we have a light-grey shade and a dark-grey shade, we
					//can create a "mid-grey shade" by checkering an area with alternating light-grey and dark-grey pixels
					//It's ugly, but I believe fairly accurately represents how shading would have to work on a 4-color screen.
					
					if (checker < 0) {
						if (col.r < fourBit) {
							col.rgb += .33333333;
						} else if (col.r > fourBit) {
							col.rgb -= .33333333;
						}
					}
				}

				if (col.r < _MinBrightness) {
					col.r = _MinBrightness;
					col.g = _MinBrightness;
					col.b = _MinBrightness;
				}
				else if (col.r > _MaxBrightness) {
					col.r = _MaxBrightness;
					col.g = _MaxBrightness;
					col.b = _MaxBrightness;
				}

				return col;
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
}