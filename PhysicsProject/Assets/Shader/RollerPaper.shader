Shader "Custom/RollerPaper"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_ScrollXSpeed("X speed", Range(0, 200)) = 0
	}
		SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		fixed _ScrollXSpeed;
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutputStandard o)
			{
			fixed2 scrolledUV = IN.uv_MainTex;

			fixed xValue = _ScrollXSpeed * _Time;

			scrolledUV += fixed2(xValue, 0);

			fixed4 c = tex2D(_MainTex, scrolledUV) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
