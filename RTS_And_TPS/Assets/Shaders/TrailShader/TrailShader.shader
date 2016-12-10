Shader "Unlit/TrailShader"
{
	Properties
	{
		_Color("Color",Color) = (1.0, 1.0,1.0, 1.0)
	}
	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent - 5" }

		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				// sample the texture
				fixed4 col = float4(_Color.r,_Color.g,_Color.b,i.color.a * _Color.a);
				return col;
			}
			ENDCG
		}
	}
}
