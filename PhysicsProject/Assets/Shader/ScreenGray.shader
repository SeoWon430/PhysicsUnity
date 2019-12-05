Shader "Custom/ScreenGray"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Luminosity("_Luminosity", Range(0,1)) = 0.0
		_DepthPower("_DepthPower", Range(0,1)) = 0.0
    }
    SubShader
    {
		Pass{

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			half _Luminosity;
			half _DepthPower;
			fixed4 _Color;

			fixed4 frag(v2f_img i) : COLOR
			{
				if (_DepthPower <= 0) {
					fixed4 renderTex = tex2D(_MainTex, i.uv);

					float luminosity = 0.299 * renderTex.r + 0.587 * renderTex.g + 0.114 * renderTex.b;
					fixed4 finalColor = lerp(renderTex, luminosity, _Luminosity);
					renderTex.rgb = finalColor;

					return renderTex;

				}
				else {

					float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv.xy));
					depth = pow(Linear01Depth(depth), _DepthPower);
					return depth;

				}

			}
			ENDCG
		
		}
        
    }
    FallBack "Diffuse"
}
