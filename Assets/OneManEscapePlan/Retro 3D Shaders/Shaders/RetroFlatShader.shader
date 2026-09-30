/// © 2018-2022 Kevin Foley
/// For distribution only on the Unity Asset Store
/// Terms/EULA: https://unity3d.com/legal/as_terms

Shader "OneManEscapePlan/RetroFlatShader" {
	Properties{
		_MainTex("Color Scheme", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Detail("Detail", 2D) = "gray" {}
	}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

		#pragma surface surf Lambert fullforwardshadows vertex:vert
		#pragma target 3.0

		struct Input {
			float3 color : COLOR;
			float4 screenPos;
		};

		sampler2D _MainTex;
		sampler2D _Detail;
		fixed4 _Color;

		void vert(inout appdata_full v) {
			v.color = tex2Dlod(_MainTex, v.texcoord) * _Color;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.color;

			float bright = IN.color.r * 0.2126 + IN.color.g * .7152 + IN.color.b * .0722;

			if (bright < 2) {
				float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
				screenUV *= float2(6, 6);
				o.Albedo *= tex2D(_Detail, screenUV).rgb * 2;
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
