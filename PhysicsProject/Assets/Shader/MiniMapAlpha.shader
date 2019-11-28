Shader "Custom/MiniMapAlpha"
{
    Properties
    {
		_MainTex("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Alpha ("Aplha",  Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0


		sampler2D _MainTex;
		struct Input {
			float2 uv_MainTex;
		};
        half _Alpha;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
            //o.Alpha = _Alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
