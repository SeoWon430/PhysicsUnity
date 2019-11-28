Shader "Custom/ToonShader"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_RampTex("Ramp (RGB)", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Toon

		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _RampTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
		}

		fixed4 LightingToon(SurfaceOutput s, fixed3 lightDir, fixed atten) {
			half NdotL = dot(s.Normal, lightDir);
			half degree;
			//NdotL = tex2D(_RampTex, fixed2(NdotL, 0.5));
			if (NdotL > 0.5) {
				degree = 1;
			}
			else if (NdotL > 0) {
				degree = 0.66;
			}
			else if (NdotL > -0.5) {
				degree = 0.33;
			}
			else {
				degree = 0;
			}

			half4 color;
			color.rgb = s.Albedo * _LightColor0.rgb * (degree * atten);
			color.a = s.Alpha;

			return color;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
