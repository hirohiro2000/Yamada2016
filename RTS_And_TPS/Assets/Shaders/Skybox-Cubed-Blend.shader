Shader "Skybox/CubemapBlend" {
Properties {
	_Tint ("Tint Color", Color) = (.5, .5, .5, .5)
	[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
	_Rotation ("Rotation", Range(0, 360)) = 0
	[NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "grey" {}
	[NoScaleOffset] _Tex2 ("Cubemap2   (HDR)", Cube) = "grey" {}
	_SkyBlend("Blend", Range(0.0, 1.0)) = 0.0
}

SubShader {
	Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
	Cull Off ZWrite Off

	Pass {
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		samplerCUBE _Tex;
		samplerCUBE _Tex2;
		float _SkyBlend;
		half4 _Tex_HDR;
		half4 _Tint;
		half _Exposure;
		float _Rotation;

		float4 RotateAroundYInDegrees (float4 vertex, float degrees)
		{
			float alpha = degrees * UNITY_PI / 180.0;
			float sina, cosa;
			sincos(alpha, sina, cosa);
			float2x2 m = float2x2(cosa, -sina, sina, cosa);
			return float4(mul(m, vertex.xz), vertex.yw).xzyw;
		}
		
		struct appdata_t {
			float4 vertex : POSITION;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float3 texcoord : TEXCOORD0;
		};

		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, RotateAroundYInDegrees(v.vertex, _Rotation));
			o.texcoord = v.vertex.xyz;
			return o;
		}

		fixed4 frag (v2f i) : SV_Target
		{
			float4 tex = texCUBE (_Tex, i.texcoord);
			float4 tex2 = texCUBE(_Tex2, i.texcoord);
			float4 color = tex2 * _SkyBlend + tex * (1 - _SkyBlend);
			float3 c = color.rgb;
			c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
			c *= _Exposure;
			return float4(c, 1);
		}
		ENDCG 
	}
} 	


Fallback Off

}
