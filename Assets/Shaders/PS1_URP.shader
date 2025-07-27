Shader "Custom/URP/PS1"
{
	Properties
	{
		[MainTexture] _BaseMap("Albedo", 2D) = "white" {}
		[MainColor] _BaseColor("Tint", Color) = (1, 1, 1, 1)

		// Lighting

		_Smoothness("Smoothness", Range(0.0, 1.0)) = 0.1
		_Specular("Specular", Range(0.0, 1.0)) = 0.0

		// PS1

		[Toggle(_PS1_JITTER)] _Jitter("Vertex Jitter", Float) = 1
		_JitterGridScale("Jitter Grid Scale", Range(0.0, 64.0)) = 20.0
		[Toggle(_PS1_PIXELSNAP)] _PixelSnap("Vertex Pixel Snap", Float) = 1

		[Toggle(_PS1_AFFINE)] _Affine("Affine Texture Mapping", Float) = 1
		// [Toggle] _VERT_LIGHTMAPPING("Vertex Shader Lightmapping", Float) = 1

		// [HideInInspector][Toggle] _PS1_PIXELBLIT("Is Pixel Blit Enabled?", Float) = 0
		// [HideInInspector] _PS1_PixelBlitParams("Pixel Blit Params", Vector) = (0, 0, 0, 0)

		// Blending state
		_Surface("__surface", Float) = 0.0
		_Blend("__blend", Float) = 0.0
		_Cull("__cull", Float) = 2.0
		/*[ToggleUI]*/ [Toggle] _AlphaClip("__clip", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _SrcBlendAlpha("__srcA", Float) = 1.0
		[HideInInspector] _DstBlendAlpha("__dstA", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		[HideInInspector] _BlendModePreserveSpecular("_BlendModePreserveSpecular", Float) = 1.0
		[HideInInspector] _AlphaToMask("__alphaToMask", Float) = 0.0
		[HideInInspector] _AddPrecomputedVelocity("_AddPrecomputedVelocity", Float) = 0.0

		[HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	HLSLINCLUDE

	#define _SPECULAR_COLOR_SPECULAR_COLOR

	#pragma shader_feature _FORWARD_PLUS

	#pragma shader_feature_fragment _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
	#pragma shader_feature_fragment _ADDITIONAL_LIGHT_SHADOWS

	#pragma shader_feature _ _PS1_AFFINE

	#pragma shader_feature_vertex _ _PS1_JITTER
	#pragma shader_feature_vertex _ _PS1_PIXELSNAP

	#pragma multi_compile _ _PS1_PIXELBLIT

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "PS1.cginc"

	CBUFFER_START(UnityPerMaterial)
		sampler2D _BaseMap;
		float4 _BaseMap_ST;
		float4 _BaseColor;
		float _Smoothness;
		float _Specular;
		float _JitterGridScale;
		float4 _PS1_PixelBlitParams;
	CBUFFER_END

#if _PS1_PIXELBLIT
	// uniform float4 _PS1_PixelBlitParams;
	#define _PS1_PIXPARAMS _PS1_PixelBlitParams
#else
	#define _PS1_PIXPARAMS _ScreenParams
#endif

	// #undef _PS1_PIXPARAMS

	ENDHLSL

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
			// "UniversalMaterialType" = "SimpleLit"
			"Queue" = "Geometry"
			// "IgnoreProjector" = "True"
		}
		// LOD 300

		Pass
		{
			Name "ForwardPass"
			Tags
			{
				"LightMode" = "UniversalForward"
			}

			// Blend states
			Blend[_SrcBlend][_DstBlend], [_SrcBlendAlpha][_DstBlendAlpha]
			ZWrite[_ZWrite]
			Cull[_Cull]
			AlphaToMask[_AlphaToMask]

			HLSLPROGRAM

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#pragma vertex Vertex
			#pragma fragment Fragment

			struct Attributes
			{
				float3 positionOS    : POSITION;
				float3 normalOS      : NORMAL;
				float2 uv            : TEXCOORD0;
			};

			struct Varyings
			{
				float4 positionCS    : SV_POSITION;
				float3 normalWS      : TEXCOORD0;
				float3 positionWS    : TEXCOORD1;
				float2 uv            : TEXCOORD2;
			};

			#include "PS1_PreVertex.cginc"

			Varyings Vertex(Attributes i)
			{
				Varyings o = PreVertex(i, _JitterGridScale, _PS1_PIXPARAMS.xy);

				o.uv = TRANSFORM_TEX(i.uv, _BaseMap);
			#if _PS1_AFFINE
				o.uv *= o.positionCS.w;
			#endif

				return o;
			}

			half4 Fragment(Varyings v) : SV_Target
			{
				float2 uv = v.uv;
			#if _PS1_AFFINE
				uv /= v.positionCS.w;
			#endif

				// 1) Sample & tint
				half4 tex = tex2D(_BaseMap, uv) * _BaseColor;

				InputData lightingData = (InputData)0;
				lightingData.positionWS = v.positionWS;
				lightingData.normalWS = normalize(v.normalWS);
				lightingData.viewDirectionWS = GetWorldSpaceViewDir(v.positionWS);
				lightingData.shadowCoord = TransformWorldToShadowCoord(v.positionWS);

				SurfaceData surface = (SurfaceData)0;
				surface.albedo = half3(1.0, 1.0, 1.0);
				surface.alpha = 1.0;
				surface.smoothness = _Smoothness;
				surface.specular = _Specular;

				half4 lighting = UniversalFragmentBlinnPhong(lightingData, surface) + unity_AmbientSky;

				half4 col = tex * lighting;

				return tex * col;
			}

			ENDHLSL
		}

		Pass
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			ColorMask 0

			HLSLPROGRAM

			#pragma vertex Vertex
			#pragma fragment Fragment

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			float3 _LightDirection;

			struct Attributes
			{
				float3 positionOS    : POSITION;
				float3 normalOS      : NORMAL;
			};

			struct Varyings
			{
				float4 positionCS    : SV_POSITION;
			};

			// #define _PS1_PREVERTEX_NO_POSITION_WS
			// #define _PS1_PREVERTEX_NO_NORMAL_WS
			// #include "PS1_PreVertex.cginc"

			float4 GetShadowPositionHClip(Attributes i)
			{
				Varyings o;
				VertexPositionInputs vInputs = GetPS1VertexPositionInputs(i.positionOS, _JitterGridScale, _PS1_PIXPARAMS.xy);

				// float3 positionWS = TransformObjectToWorld(i.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(i.normalOS);
				o.positionCS = TransformWorldToHClip(ApplyShadowBias(vInputs.positionWS, normalWS, _LightDirection));

				o.positionCS = ApplyShadowClamping(o.positionCS);

				return o.positionCS;
			}

			Varyings Vertex(Attributes i)
			{
				Varyings o; // = PreVertex(i, _JitterGridScale, _PS1_PIXPARAMS.xy);

				o.positionCS = GetShadowPositionHClip(i);

				return o;
			}

			half4 Fragment(Varyings v) : SV_TARGET
			{
				return 0;
			}

			ENDHLSL
		}
	}

	Fallback "Hidden/Universal Render Pipeline/FallbackError"
	CustomEditor "piqey.PS1.PS1Shader"
}
