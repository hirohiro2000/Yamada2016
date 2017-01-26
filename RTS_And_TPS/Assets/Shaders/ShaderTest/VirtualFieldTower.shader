Shader "Unlit/VirtualFieldTower"
{
	Properties
	{
		_MainTex ("Albedo", 2D)						= "white" {}
		_MainColor("MainColor", Color)				= (1.0, 1.0, 1.0, 1.0)
		_EffectColor("EffectColor", Color)			= (1.0, 1.0, 1.0, 1.0)
		_EffectMul("Mul",float)						= 1.0
		_EffectPow("Pow",float)						= 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex	: POSITION;
				float2 uv		: TEXCOORD0;
				float3 normal	: NORMAL;
			};

			struct v2f
			{
				float4 pos			: SV_POSITION;
				float2 uv			: TEXCOORD0;
				float3 viewNormal	: TEXCOORD1;
				float4 viewPos		: TEXCOORD2;
				float4 vertex		: TEXCOORD3;
				UNITY_FOG_COORDS(4)
			};

			sampler2D	_MainTex;
			float4		_MainTex_ST;
			float3		_MainColor;
			float3		_EffectColor;
			float 		_EffectMul;
			float 		_EffectPow;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos			= mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv			= TRANSFORM_TEX(v.uv, _MainTex);
				o.viewNormal	= mul(UNITY_MATRIX_MV, v.normal);
				o.viewPos		= mul(UNITY_MATRIX_MV, v.vertex);
				o.vertex		= v.vertex;
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target                            
			{                                                                                                      
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv)*0.3;

				float opacity = dot(normalize(i.viewNormal), normalize(-i.viewPos));
				opacity  = clamp(pow(abs(opacity)*_EffectMul, _EffectPow), 0, 1);
				col.rgb  += _EffectColor*opacity;
				col.a    += 1.0;

				opacity = dot(normalize(i.viewNormal), normalize(-i.viewPos));
				opacity = 1.0f - clamp(pow(abs(opacity)*2.0, 8), 0, 1);
				col.rgb += _MainColor*opacity;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}                                 
			ENDCG                                        
		}
	}
}
