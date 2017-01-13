
//Shader "Unlit/FocusFieldTower"
//{
//	Properties 
//	{
//        _Color ("Main Color", Color)				= (1.0, 1.0, 1.0, 1.0)
//        _MainTex ("Base (RGB) Alpha (A)", 2D)		= "white" {}
//		_EffectColor("EffectColor", Color)			= (1.0, 1.0, 1.0, 1.0)
//		_HorizontalStripes("HorizontalStripes", 2D) = "white" {}
//		_UVAnimationY("UVAnimationY", float)		= 0.0
//	}
//    SubShader 
//	{
//        Pass 
//		{
//			Tags {"Queue" = "Geometry" "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #pragma multi_compile_fwdbase
//
//            #include "UnityCG.cginc"
//            #include "AutoLight.cginc"
//
//            struct vertex_input 
//			{
//                float4 vertex	: POSITION;
//                float3 normal	: NORMAL;
//                float4 texcoord : TEXCOORD0;
//            };
//
//            struct vertex_output 
//			{
//                float4 pos				: SV_POSITION;
//                float2 uv				: TEXCOORD0;
//                float3 lightDir			: TEXCOORD1;
//                float3 normal			: TEXCOORD2;
//                LIGHTING_COORDS(3, 4)
//                float3 vertexLighting	: TEXCOORD5;
//            };
//
//            sampler2D _MainTex;
//            float4 _MainTex_ST;
//            fixed4 _Color;
//            fixed4 _LightColor0;
//			
//            vertex_output vert(vertex_input v) 
//			{
//                vertex_output o;
//
//                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//                o.uv = v.texcoord.xy;
//                o.lightDir = ObjSpaceLightDir(v.vertex);
//                o.normal = v.normal;
//                TRANSFER_VERTEX_TO_FRAGMENT(o);
//
//                o.vertexLighting = float3(0.0, 0.0, 0.0);
//
//                #ifdef VERTEXLIGHT_ON
//
//                float3 worldN	= mul((float3x3)_Object2World, SCALED_NORMAL);
//                float4 worldPos = mul(_Object2World, v.vertex);
//
//                for (int index = 0; index < 4; index++) 
//				{    
//                   float4 lightPosition = float4(unity_4LightPosX0[index], unity_4LightPosY0[index], unity_4LightPosZ0[index], 1.0);
//                   float3 vertexToLightSource = float3(lightPosition - worldPos);        
//                   float3 lightDirection = normalize(vertexToLightSource);
//                   float squaredDistance = dot(vertexToLightSource, vertexToLightSource);
//                   float attenuation = 1.0 / (1.0  + unity_4LightAtten0[index] * squaredDistance);
//                   float3 diffuseReflection = attenuation * float3(unity_LightColor[index]) * float3(_Color) * max(0.0, dot(worldN, lightDirection));         
//                   o.vertexLighting = o.vertexLighting + diffuseReflection * 2;
//                }
//
//                #endif
//
//                return o;
//            }
//
//            half4 frag(vertex_output i) : COLOR 
//			{
//                i.lightDir = normalize(i.lightDir);
//                fixed atten = LIGHT_ATTENUATION(i);
//
//                fixed4 tex = tex2D(_MainTex, i.uv);
//                tex *= _Color + fixed4(i.vertexLighting, 1.0);
//
//                fixed diff = saturate(dot(i.normal, i.lightDir));
//
//                fixed4 c;
//                c.rgb = UNITY_LIGHTMODEL_AMBIENT.rgb * 2 * tex.rgb;
//                c.a = tex.a + _LightColor0.a * atten;
//
//                return c;
//            }
//
//            ENDCG
//        }
//
//		Pass
//		{
//			Name "X-Ray"
//			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
//			LOD 200
//
//			CGPROGRAM
//#pragma vertex   vert
//#pragma fragment frag
//#define UNITY_PASS_FORWARDBASE
//#include "UnityCG.cginc"
//
//			uniform float4		_Color;
//			uniform float4		_EffectColor;
//			uniform sampler2D	_HorizontalStripes;
//			uniform float4		_HorizontalStripes_ST;
//			uniform float		_UVAnimationY;
//
//			struct VertexInput {
//				float4 vertex		: POSITION;
//				float3 normal		: NORMAL;
//				float2 texcoord		: TEXCOORD0;
//			};
//			struct VertexOutput {
//				float4 pos			: SV_POSITION;
//				float2 uv			: TEXCOORD0;
//				float3 viewNormal	: TEXCOORD1;
//				float4 viewPos		: TEXCOORD2;
//				float4 vertex		: TEXCOORD3;
//			};
//			
//			VertexOutput vert(VertexInput v) 
//			{
//				VertexOutput o = (VertexOutput)0;
//
//				o.pos			= mul(UNITY_MATRIX_MVP, v.vertex);
//				o.uv			= v.texcoord;
//				o.viewNormal	= mul(UNITY_MATRIX_MV,  v.normal);
//				o.viewPos		= mul(UNITY_MATRIX_MV, v.vertex);
//				o.vertex		= v.vertex;
//
//				return o;
//			}
//			float4 frag(VertexOutput i) : COLOR
//			{
//				float4 outColor = float4(0, 0, 0, 1);
//
//				float4 effect = tex2D(_HorizontalStripes, TRANSFORM_TEX(float2(i.vertex.x, i.vertex.y*5 + _UVAnimationY), _HorizontalStripes));
//
//				float opacity = dot(normalize(i.viewNormal), normalize(-i.viewPos));
//				opacity = clamp( pow(abs(opacity*1.5), 9), 0.1, 0.9 );
//
//				outColor.rgb  = _EffectColor * effect;
//				outColor.a    = effect.a     * opacity;
//
//				return outColor;
//
//			}
//			ENDCG
//		}
//    }
//    Fallback "VertexLit"
//}

Shader "Unlit/FocusFieldTower"
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
			Tags {"Queue" = "Geometry" "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct vertex_input 
			{
                float4 vertex	: POSITION;
                float3 normal	: NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct vertex_output 
			{
                float4 pos				: SV_POSITION;
                float2 uv				: TEXCOORD0;
                float3 lightDir			: TEXCOORD1;
                float3 normal			: TEXCOORD2;
                LIGHTING_COORDS(3, 4)
                float3 vertexLighting	: TEXCOORD5;
				float3 viewNormal		: TEXCOORD6;
				float4 viewPos			: TEXCOORD7;
				float4 vertex			: TEXCOORD8;
			};

			uniform float4		_Color;
            uniform sampler2D	_MainTex;
            uniform float4		_MainTex_ST;
			uniform float4		_EffectColor;
			uniform sampler2D	_HorizontalStripes;
			uniform float4		_HorizontalStripes_ST;
            uniform float4		_LightColor0;
			uniform float		_UVAnimationY;
			
            vertex_output vert(vertex_input v) 
			{
                vertex_output o;

                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.uv = v.texcoord.xy;
                o.lightDir = ObjSpaceLightDir(v.vertex);
                o.normal = v.normal;
                TRANSFER_VERTEX_TO_FRAGMENT(o);

				o.viewNormal	= mul(UNITY_MATRIX_MV, v.normal);
				o.viewPos		= mul(UNITY_MATRIX_MV, v.vertex);
				o.vertex		= v.vertex;

                o.vertexLighting = float3(0.0, 0.0, 0.0);

                #ifdef VERTEXLIGHT_ON

                float3 worldN	= mul((float3x3)_Object2World, SCALED_NORMAL);
                float4 worldPos = mul(_Object2World, v.vertex);

                for (int index = 0; index < 4; index++) 
				{    
                   float4 lightPosition = float4(unity_4LightPosX0[index], unity_4LightPosY0[index], unity_4LightPosZ0[index], 1.0);
                   float3 vertexToLightSource = float3(lightPosition - worldPos);        
                   float3 lightDirection = normalize(vertexToLightSource);
                   float squaredDistance = dot(vertexToLightSource, vertexToLightSource);
                   float attenuation = 1.0 / (1.0  + unity_4LightAtten0[index] * squaredDistance);
                   float3 diffuseReflection = attenuation * float3(unity_LightColor[index]) * float3(_Color) * max(0.0, dot(worldN, lightDirection));         
                   o.vertexLighting = o.vertexLighting + diffuseReflection * 2;
                }

                #endif

                return o;
            }

            half4 frag(vertex_output i) : COLOR 
			{
                i.lightDir = normalize(i.lightDir);
                fixed atten = LIGHT_ATTENUATION(i);

                fixed4 tex = tex2D(_MainTex, i.uv);
                tex *= _Color + fixed4(i.vertexLighting, 1.0);

                fixed diff = saturate(dot(i.normal, i.lightDir));

                fixed4 c;
                c.rgb = UNITY_LIGHTMODEL_AMBIENT.rgb * 2 * tex.rgb;
                c.a = tex.a + _LightColor0.a * atten;


				float3 effect = tex2D(_HorizontalStripes, TRANSFORM_TEX(float2(0.0, i.vertex.y * 4.0 + _UVAnimationY), _HorizontalStripes));

				float opacity = dot(normalize(i.viewNormal), normalize(-i.viewPos));
				
				float opacity1 = clamp(pow(abs(opacity),  2), 0.1, 1.0);

				c.rgb = c.rgb*0.4 + effect*_EffectColor;	
				c.a   = opacity1;							

				return c;
            }

            ENDCG
        }
	}

	FallBack "Diffuse"

}
