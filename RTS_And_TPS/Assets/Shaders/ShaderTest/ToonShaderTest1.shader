// url:http://qiita.com/kamasu/items/08231953c8dd233de62c

Shader "Custom/ToonShaderTest1" 
{
	Properties{
		_Color("Color", Color)				= ( 1.0, 1.0, 1.0, 1.0 )
		_MainTex("Base Color", 2D)			= "white" {}
		_ShadeColor("ShadeColor", Color)	= (1.0, 1.0, 1.0, 1.0)
		_Emission("Emission", Color)		= (0.0, 0.0, 0.0, 0.0)
	}
	SubShader{
		Tags{
			"RenderType" = "Opaque"
		}

		Pass{

			Name "FORWARD"
			Tags{ "LightMode" = "ForwardBase" }

			Stencil{
				Ref   2
				Comp  always
				Pass  replace
				Fail  replace
				ZFail keep
			}


			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			uniform float4		_Color;
			uniform float4		_Emission;
			uniform sampler2D	_MainTex; 
			uniform float4		_MainTex_ST;
			struct VertexInput {
				float4 vertex		: POSITION;
				float3 normal		: NORMAL;
				float2 texcoord		: TEXCOORD0;
			};
			struct VertexOutput {
				float4 pos			: SV_POSITION;
				float2 uv			: TEXCOORD0;
				float3 normalDir	: TEXCOORD1;
			};

			VertexOutput vert(VertexInput v) {
				VertexOutput o = (VertexOutput)0;
	
				o.pos		= mul(UNITY_MATRIX_MVP, v.vertex);
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.uv = v.texcoord;
				return o;

			}
			float4 frag(VertexOutput i) : COLOR{

				float4 albedo = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				float3 diffuseColor = albedo.rgb*_Color.rgb;


				float4 outColor = (float4)0;
				outColor.rgb	= diffuseColor.rgb;
				outColor.a		= albedo.a;

				outColor.rgb	+= _Emission.rgb * _Emission.a;

				return outColor;
			
			}

			ENDCG

		}


		Pass{
			Name "X-Ray"
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
//			Tags{ "LightMode" = "ForwardBase" }
			
//			Blend SrcAlpha OneMinusSrcAlpha
			Blend  OneMinusDstColor One
			ZTest  Always
			ZWrite Off

			Stencil{
				Ref 2
				Comp notequal
			}

			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"

			uniform float4		_ShadeColor;

			struct VertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			struct VertexOutput {
				float4 pos			: SV_POSITION;
				float3 viewNormal	: TEXCOORD0;
				float4 viewPos		: TEXCOORD1;
			};
			VertexOutput vert(VertexInput v) {
				VertexOutput o = (VertexOutput)0;

				o.pos		 = mul( UNITY_MATRIX_MVP, v.vertex  );
				o.viewNormal = mul( UNITY_MATRIX_MV,  v.normal	);
				o.viewPos	 = mul( UNITY_MATRIX_MV,  v.vertex	);

				return o;
			}
			float4 frag(VertexOutput i) : COLOR{

				float4 outColor = float4( 0, 0, 0, 1 );

				float opacity = dot(normalize(i.viewNormal), normalize(-i.viewPos));
				opacity		= 1.0 - clamp(pow(abs(opacity), 5), 0, 1);
				outColor	= opacity * _ShadeColor;

				return outColor;

//				return _ShadeColor;

			}
			ENDCG
		}
	}

	FallBack "Diffuse"

}
