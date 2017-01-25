Shader "Unlit/PermeateFieldTower"
{
	Properties
	{
		_Color("Color", Color)						= (1.0, 1.0, 1.0, 1.0)
		_EffectColor("EffectColor", Color)			= (1.0, 1.0, 1.0, 1.0)
		_MainTex("Albedo", 2D)						= "white" {}
		_HorizontalStripes("HorizontalStripes", 2D) = "white" {}
		_UVAnimationY("UVAnimationY", float)		= 0.0
	}
	SubShader
	{
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
	
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			//user defined
			uniform sampler2D	_MainTex;
			uniform sampler2D	_NormalMap;
			uniform sampler2D	_SmoothMap;
			uniform sampler2D	_MetalMap;
			uniform sampler2D	_RSRM;
			uniform float4		_MainTex_ST;
			uniform float4		_NormalMap_ST;
			uniform float4		_SmoothMap_ST;
			uniform float4		_MetalMap_ST;
			uniform float4 		_MainColor;
			uniform float 		_Smoothness;
			uniform float		_Wrap;
			uniform float		_BumpDepth;
			uniform float		_Metallicity;
		
			//unity defined
			uniform float4 	_LightColor0;
		
			//base input struct
			struct vertexInput
			{
				float4 vertex	: POSITION;
				float3 normal	: NORMAL;
				float4 texcoord : TEXCOORD0;
				float4 tangent	: TANGENT;
			};
		
			struct vertexOutput
			{
				float4 pos			 : SV_POSITION;
				float4 texcoord		 : TEXCOORD0;
				float4 posWorld		 : TEXCOORD1;
				float3 normalWorld	 : TEXCOORD2;
				float3 tangentWorld	 : TEXCOORD3;
				float3 binormalWorld : TEXCOORD4;
		
				LIGHTING_COORDS(5,6)
			};
		
			//vertex function
			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;
		
				float4x4 modelMatrix		= _Object2World;
				float4x4 modelMatrixInverse = _World2Object;
		
				o.normalWorld	= normalize( mul(_World2Object, float4(v.normal, 0.0)     ).xyz );
				o.tangentWorld  = normalize( mul(_Object2World, float4(v.tangent.xyz, 0.0))     );
				o.binormalWorld = normalize( cross(o.normalWorld, o.tangentWorld) * v.tangent.w );
		
				o.posWorld	= mul( _Object2World,    v.vertex );
				o.pos		= mul( UNITY_MATRIX_MVP, v.vertex );
				o.texcoord  = v.texcoord;
		
				TRANSFER_VERTEX_TO_FRAGMENT(o); // for shadows
		
				return o;
		
			}
		
			//take a -1 to 1 range and fit it 0 to 1
			float clamp01(float toBeNormalized)
			{
				return toBeNormalized * 0.5 + 0.5;
			}
		
			float3 calculateAmbientReflection(float3 rsrm , float texM)
			{
				float  mask = (rsrm.x + rsrm.y + rsrm.z) * 0.33;
				float3 amb = UNITY_LIGHTMODEL_AMBIENT.xyz;
				return  float3 (1.5 * rsrm * amb + amb * 0.5 * texM);
			}
		
			float4 frag(vertexOutput i) : COLOR
			{
				float shadAtten = LIGHT_ATTENUATION(i);
		
				float4 tex = tex2D(_MainTex,    i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				tex = tex  * _MainColor;
				float  texS = tex2D(_SmoothMap, i.texcoord.xy * _SmoothMap_ST.xy + _SmoothMap_ST.zw);
				texS = texS * _Smoothness;
				float  texM = tex2D(_MetalMap,  i.texcoord.xy * _MetalMap_ST.xy + _MetalMap_ST.zw);
				texM = texM * _Metallicity;
				float4 texN = tex2D(_NormalMap, i.texcoord.xy * _NormalMap_ST.xy + _NormalMap_ST.zw);
				float nDepth = 8 / (_BumpDepth * 8);
			
				//Unpack Normal
				float3 localCoords = float3(2.0 * texN.ag - float2(1.0, 1.0), 0.0);
				localCoords.z = nDepth;
			
				//normal transpose matrix
				float3x3 local2WorldTranspose = float3x3
				(
					i.tangentWorld,
					i.binormalWorld,
					i.normalWorld
				);
			
				//Calculate normal direction
				float3 normalDir = normalize( mul(localCoords, local2WorldTranspose) );
			
				float3 N = normalize(normalDir);
				float3 V = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				float3 fragmentToLight = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
				float  distanceToLight = length(fragmentToLight);
				float  atten = pow(2, -0.1 * distanceToLight * distanceToLight) * _WorldSpaceLightPos0.w + 1 - _WorldSpaceLightPos0.w; // (-0.1x^2)^2 for pointlights 1 for dirlights
				float3 L = (normalize(fragmentToLight)) * _WorldSpaceLightPos0.w + normalize(_WorldSpaceLightPos0.xyz) * (1 - _WorldSpaceLightPos0.w);
				float3 H = normalize(V + L);
				float3 worldReflect = reflect(V,N);
			
				//lighting
				float NdotL = dot(N,L);
				float NdotV = 1 - max(0.0, dot(N,V));
				float NdotH = clamp(dot(N,H), 0, 1);
				float VdotL = clamp01(dot(V,L));
				float wrap = clamp(_Wrap, -0.25, 1.0);
			
				float4 texdesat = dot(tex.rgb, float3(0.3, 0.59, 0.11));
			
				float3 difftex = lerp(tex, float4(0,0,0,0), pow(texM, 1)).xyz;
				float3 spectex = lerp(texdesat, tex, texM).xyz;
			
				VdotL = pow(VdotL, 0.85);
				float smooth = 4 * pow(1.8, texS - 2) + 1.5;
				float rim = texM + (pow(NdotV, 1 + texS / 6)) * (1 - texM);
				float bellclamp = (1 / (1 + pow(0.65 * acos(dot(N,L)), 16)));
			
				float3 rsrm = tex2D(_RSRM, float2((1 - (texS - 1) * 0.09), 1 - clamp01(worldReflect.y)));
				float3 rsrmDiff = tex2D(_RSRM, float2(1, N.y));
				float3 ambReflect = calculateAmbientReflection(rsrm    , texM);
				float3 ambReflectDiff = calculateAmbientReflection(rsrmDiff, texM);
			
				float3 spec = NdotH;
				spec = pow(spec, smooth * VdotL) * log(smooth*(VdotL + 1)) * bellclamp * texS * (1 / texS) * 0.5;
				spec *= shadAtten * atten * spectex.xyz * _LightColor0.rgb * (2 + texM) * spectex.xyz;
				spec += ambReflect * spectex.rgb * rim * 2;
			
				float3 diff = max(0, (pow(max(0, (NdotL * (1 - wrap) + wrap)), (2 * wrap + 1))));
				diff *= lerp(shadAtten, 1, wrap) * atten * difftex.xyz * _LightColor0.rgb * 2 * _LightColor0.rgb * difftex.xyz;
				diff += ambReflect * difftex.xyz * rim + ambReflectDiff * 2 * difftex.xyz;
			
				return float4 (atan(clamp(spec + diff, 0, 2)), 1); //this is used to round off values above one and give better color reproduction in bright scenes
		
			}
			ENDCG
		}
		Pass
		{
			Name "X-Ray"
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
			
			Blend  OneMinusDstColor One
			ZTest  Always
			ZWrite Off

			CGPROGRAM
		#pragma vertex   vert
		#pragma fragment frag
		#define UNITY_PASS_FORWARDBASE
		#include "UnityCG.cginc"

			uniform float4		_EffectColor;
			uniform float		_UVAnimationY;

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

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.viewNormal = mul(UNITY_MATRIX_MV,  v.normal);
				o.viewPos = mul(UNITY_MATRIX_MV,  v.vertex);

				return o;
			}
			float4 frag(VertexOutput i) : COLOR{

				float4 outColor = float4(0, 0, 0, 1);

				float opacity = dot(normalize(i.viewNormal), normalize(-i.viewPos));
				opacity = 1.0 - clamp(pow(abs(opacity), 5), 0, 1);
				outColor    = opacity * _EffectColor;
				outColor.a  = 0.0f;

				return outColor;

			}
			ENDCG
		}
	}

	FallBack "Diffuse"

}
