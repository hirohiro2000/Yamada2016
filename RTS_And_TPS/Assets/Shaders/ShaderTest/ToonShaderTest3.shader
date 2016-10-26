// url:http://qiita.com/kamasu/items/08231953c8dd233de62c

Shader "Custom/ToonShaderTest3" 
{
	Properties{
		_Color("Color", Color)				= ( 1.0, 1.0, 1.0, 1.0 )
		_MainTex("Base Color", 2D)			= "white" {}
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth("Outline Width", Range( 0, 0.2) ) = 0.05
	}
	SubShader{

		Pass{

			Name "STENCIL"
			Tags{ "LightMode" = "Always" }

			Stencil{
				Ref		2
				Comp	always
				Pass	replace
				ZFail	replace
			}

			Lighting	Off
			Cull		Off
			ColorMask	0
			ZTest		Off
			ZWrite		Off

			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform sampler2D	_MainTex;
			uniform float4		_MainTex_ST;
			uniform float		_OutlineWidth;

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
				VertexOutput o;
				
				float3		n = normalize(v.normal);
				float4		p = v.vertex;
				p.xyz += n*_OutlineWidth;

				o.pos		= mul(UNITY_MATRIX_MVP, p);
				o.uv		= v.texcoord;
				o.normalDir = mul(UNITY_MATRIX_MVP, p);
								
				return o;
			}
			float4 frag(VertexOutput i) : COLOR{
				return (float4)0;
			}

			ENDCG

		}


		Pass{

			Name "MAIN"
			Tags{ 
				"LightMode"		= "ForwardBase"
				"RenderType"	= "Opaque"
				"Queue"			= "Opaque"
			}
			
			Stencil{
				Ref		3
				Comp	always
				Pass	replace
				Fail	replace
				ZFail	replace
			}
				
			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform sampler2D	_MainTex;
			uniform float4		_MainTex_ST;
			uniform float4		_Color;

			struct VertexInput {
				float4 vertex		: POSITION;
				float3 normal		: NORMAL;
				float2 texcoord		: TEXCOORD0;
			};
			struct VertexOutput {
				float4 pos			: SV_POSITION;
				float2 uv			: TEXCOORD0;
			};

			VertexOutput vert(VertexInput v) {
				VertexOutput o;
				
				o.pos		= mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv		= v.texcoord;
								
				return o;
			}
			float4 frag(VertexOutput i) : COLOR{

				float4 albedo = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex));
				float3 diffuseColor = albedo.rgb*_Color.rgb;

				float4 outColor = (float4)0;
				outColor.rgb	= diffuseColor.rgb;
				outColor.a		= albedo.a;
				
				return outColor;
			
			}
			ENDCG
		}

		Pass{

			Name "OUTLINE"

			Stencil{
				Ref 2
				Comp equal
			}

			Lighting	Off
			Cull		Front
			ZTest		Off

			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			uniform float		_OutlineWidth;
			uniform float4		_OutlineColor;

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
				VertexOutput o;
				
				float3		n = normalize(v.normal);
				float4		p = v.vertex;
				p.xyz += n*_OutlineWidth;

				o.pos = mul(UNITY_MATRIX_MVP, p);
				o.uv = v.texcoord;
				o.normalDir = mul(UNITY_MATRIX_MVP, p);

				return o;
			}
			float4 frag(VertexOutput i) : COLOR{

				return _OutlineColor;
			
			}
			ENDCG

		}

	}

	FallBack "Diffuse"

}
