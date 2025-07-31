Shader "Custom/URP/PS1"
{
	Properties
	{
		[MainTexture] _BaseMap("Albedo", 2D) = "white" {}
		[MainColor] _BaseColor("Tint", Color) = (1, 1, 1, 1)

		_Cutoff("Alpha Cutoff", Float) = 1.0

		[Normal] _BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 1.0

		_EmissionMap("Emission", 2D) = "white" {}
		[HDR] _EmissionColor("Color", Color) = (0, 0, 0, 0)

		// Lighting

		// _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.1
		// _Specular("Specular", Range(0.0, 1.0)) = 0.0

		// PS1

		[Toggle(_PS1_JITTER)] _Jitter("Vertex Jitter", Float) = 1
		_JitterGridScale("Jitter Grid Scale", Range(0.0, 64.0)) = 20.0
		[Toggle(_PS1_PIXELSNAP)] _PixelSnap("Vertex Pixel Snap", Float) = 1

		[Toggle(_PS1_AFFINE)] _Affine("Affine Texture Mapping", Float) = 1
		// [Toggle] _VERT_LIGHTMAPPING("Vertex Shader Lightmapping", Float) = 1

		_PS1_SAMPLER("Sampling Type", Float) = 2

		// [HideInInspector][Toggle] _PS1_PIXELBLIT("Is Pixel Blit Enabled?", Float) = 0
		// [HideInInspector] _PS1_PixelBlitParams("Pixel Blit Params", Vector) = (0, 0, 0, 0)

		// Blending state
		_Surface("__surface", Float) = 0.0
		_Blend("__blend", Float) = 0.0
		_Cull("__cull", Float) = 2.0
		[ToggleUI] _AlphaClip("__clip", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _SrcBlendAlpha("__srcA", Float) = 1.0
		[HideInInspector] _DstBlendAlpha("__dstA", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		// [HideInInspector] _ZWriteControl("_ZWriteControl", Float) = 0.0
		[HideInInspector] _BlendModePreserveSpecular("_BlendModePreserveSpecular", Float) = 1.0
		[HideInInspector] _AlphaToMask("__alphaToMask", Float) = 0.0
		[HideInInspector] _AddPrecomputedVelocity("_AddPrecomputedVelocity", Float) = 0.0
		[HideInInspector] _CastShadows("CastShadows", Float) = 1
		[HideInInspector] _ReceiveShadows("ReceiveShadows", Float) = 1
		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0

		[HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	HLSLINCLUDE

	#define _SPECULAR_COLOR
	// #define _SPECULAR_COLOR_SPECULAR_COLOR
	#define _SPECULARHIGHLIGHTS_OFF

	#pragma shader_feature _FORWARD_PLUS

	#pragma shader_feature_fragment _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
	#pragma shader_feature_fragment _ADDITIONAL_LIGHT_SHADOWS

	// #pragma shader_feature_local _NORMALMAP
	// #pragma shader_feature_local_fragment _EMISSION

	#pragma shader_feature _ _PS1_AFFINE

	#pragma shader_feature_vertex _ _PS1_JITTER
	#pragma shader_feature_vertex _ _PS1_PIXELSNAP

	// #pragma shader_feature_fragment _ _PS1_POINTCLAMP

	#pragma multi_compile _ _PS1_PIXELBLIT

	// #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
	// #pragma multi_compile _ SHADOWS_SHADOWMASK
	// #pragma multi_compile _ DIRLIGHTMAP_COMBINED
	// #pragma multi_compile _ LIGHTMAP_ON
	// #pragma multi_compile _ DYNAMICLIGHTMAP_ON
	// #pragma multi_compile _ USE_LEGACY_LIGHTMAPS

	// #pragma multi_compile_instancing
	// #pragma instancing_options renderinglayer

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "PS1.cginc"

	CBUFFER_START(UnityPerMaterial)
		// sampler2D _BaseMap;
		float4 _BaseMap_ST;
		half4 _BaseColor;

		half _Cutoff;

		// sampler2D _BumpMap;
		float4 _BumpMap_ST;
		half _BumpScale;

		// sampler2D _EmissionMap;
		half4 _EmissionColor;

		// half _Smoothness;
		// half _Specular;

		half _JitterGridScale;
		// float4 _PS1_PixelBlitParams;

		float _Surface;
	CBUFFER_END

	float4 _PS1_PixelBlitParams;

#if _PS1_PIXELBLIT
	// uniform float4 _PS1_PixelBlitParams;
	#define _PS1_PIXPARAMS _PS1_PixelBlitParams
#else
	#define _PS1_PIXPARAMS _ScreenParams
#endif

	// #undef _PS1_PIXPARAMS

	// #pragma shader_feature_fragment _ _PS1_POINTCLAMP
	#pragma shader_feature_fragment _ _PS1_SAMPLER_POINTCLAMP _PS1_SAMPLER_POINTREPEAT

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

#if _PS1_SAMPLER_POINTCLAMP
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

	#define _PS1_SAMPLER sampler_PointClamp
	#define _PS1_SAMPLER_BUMP sampler_PointClamp
	#define _PS1_SAMPLER_EMISSION sampler_PointClamp
#elif _PS1_SAMPLER_POINTREPEAT
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

	#define _PS1_SAMPLER sampler_PointRepeat
	#define _PS1_SAMPLER_BUMP sampler_PointRepeat
	#define _PS1_SAMPLER_EMISSION sampler_PointRepeat
#else
	#define _PS1_SAMPLER sampler_BaseMap
	#define _PS1_SAMPLER_BUMP sampler_BumpMap
	#define _PS1_SAMPLER_EMISSION sampler_EmissionMap
#endif

	ENDHLSL

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
			// "UniversalMaterialType" = "Lit"
			"Queue" = "Geometry"
			// "IgnoreProjector" = "True"
		}
		// LOD 300

		Pass
		{
			Name "ForwardLit"
			Tags
			{
				"LightMode" = "UniversalForward"
			}

			// Blend states
			Blend[_SrcBlend][_DstBlend], [_SrcBlendAlpha][_DstBlendAlpha]
			ZWrite[_ZWrite]
			// ZWrite On
			Cull[_Cull]
			AlphaToMask[_AlphaToMask]

			HLSLPROGRAM

			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local_fragment _EMISSION

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile _ USE_LEGACY_LIGHTMAPS
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fragment _ DEBUG_DISPLAY

			#pragma multi_compile_fog

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"

			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#pragma vertex Vertex
			#pragma fragment Fragment

		// #if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) || defined(_DETAIL)
		// 	#define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
		// #endif

			struct Attributes
			{
				float3 positionOS    : POSITION;

				float3 normalOS      : NORMAL;
				float4 tangentOS     : TANGENT;

				float2 uv            : TEXCOORD0;

				float2 staticLightmapUV        : TEXCOORD1;
				float2 dynamicLightmapUV       : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS    : SV_POSITION;

				float2 uv            : TEXCOORD0;
				float3 positionWS    : TEXCOORD1;

			#ifdef _NORMALMAP
				half4 normalWS       : TEXCOORD2;
				half4 tangentWS      : TEXCOORD3;
				half4 bitangentWS    : TEXCOORD4;
			#else
				half3 normalWS       : TEXCOORD2;
			#endif

			// #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
			// 	half4 tangentWS      : TEXCOORD3;
			// #endif

			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				half4 fogFactorAndVertexLight : TEXCOORD5; // x: fogFactor, yzw: vertex light
			#else
				half fogFactor       : TEXCOORD5;
			#endif

				DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 6);

			#ifdef DYNAMICLIGHTMAP_ON
				float2 dynamicLightmapUV     : TEXCOORD7;
			#endif

			#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
				half3 viewDirTS      : TEXCOORD8;
			#endif

			#ifdef USE_APV_PROBE_OCCLUSION
				float4 probeOcclusion : TEXCOORD9;
			#endif

			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord   : TEXCOORD10;
			#endif

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#include "PS1_PreVertex.cginc"
			#include "PS1_FragHelper.cginc"

			Varyings Vertex(Attributes i)
			{
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;

				UNITY_SETUP_INSTANCE_ID(i);
				Varyings o = PreVertex(i, _JitterGridScale, _PS1_PIXPARAMS.xy, vertexInput);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.uv = TRANSFORM_TEX(i.uv, _BaseMap);
			#if _PS1_AFFINE
				o.uv *= o.positionCS.w;
			#endif

				// normalWS and tangentWS already normalized.
				// This is required to avoid skewing the direction during interpolation.
				// Also required for per-vertex lighting and SH evaluation
				VertexNormalInputs normalInput = GetVertexNormalInputs(i.normalOS, i.tangentOS);

			#if defined(_FOG_FRAGMENT)
				half fogFactor = 0.0h;
			#else
				fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
			#endif

				half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);

				o.positionWS.xyz = vertexInput.positionWS;
				o.positionCS = vertexInput.positionCS;

			#ifdef _NORMALMAP
				half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
				o.normalWS = half4(normalInput.normalWS, viewDirWS.x);
				o.tangentWS = half4(normalInput.tangentWS, viewDirWS.y);
				o.bitangentWS = half4(normalInput.bitangentWS, viewDirWS.z);
			#else
				o.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);
			#endif

				OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, o.staticLightmapUV);
			#ifdef DYNAMICLIGHTMAP_ON
				o.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
			#endif
				OUTPUT_SH4(vertexInput.positionWS, o.normalWS.xyz, GetWorldSpaceNormalizeViewDir(vertexInput.positionWS), o.vertexSH, o.probeOcclusion);

			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
			#else
				o.fogFactor = fogFactor;
			#endif

			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				o.shadowCoord = GetShadowCoord(vertexInput);
			#endif

			// #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
				// o.positionWS = vertexInput.positionWS;
			// #endif

				return o;
			}

			void Fragment(
				Varyings v
				, out half4 outCol : SV_Target0
			#ifdef _WRITE_RENDERING_LAYERS
				, out float4 outRenderingLayers : SV_Target1
			#endif
			)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(v);

				float2 uv = v.uv;
			#if _PS1_AFFINE
				uv /= v.positionCS.w;
			#endif

				InputData lightData = (InputData)0;

				SurfaceData surface = (SurfaceData)0;

				PS1_InitializeStandardLitSurfaceData(uv, surface);

				PS1_InitializeInputData(v, surface.normalTS, lightData);

				lightData.normalWS = NormalizeNormalPerPixel(lightData.normalWS);
				PS1_InitializeBakedGIData(v, lightData);

				SETUP_DEBUG_TEXTURE_DATA(lightData, UNDO_TRANSFORM_TEX(v.uv, _BaseMap));

			#if defined(_DBUFFER)
				ApplyDecalToSurfaceData(i.positionCS, surfaceData, lightData);
			#endif

			#ifdef LOD_FADE_CROSSFADE
				LODFadeCrossFade(input.positionCS);
			#endif

			#if defined(DEBUG_DISPLAY)
			#if defined(LIGHTMAP_ON)
				lightData.staticLightmapUV = v.staticLightmapUV;
			#else
				lightData.vertexSH = v.vertexSH;
			#endif
			#if defined(DYNAMICLIGHTMAP_ON)
				lightData.dynamicLightmapUV = v.dynamicLightmapUV;
			#endif
			#if defined(USE_APV_PROBE_OCCLUSION)
				inputData.probeOcclusion = input.probeOcclusion;
			#endif
			#endif

				outCol = UniversalFragmentBlinnPhong(lightData, surface) + unity_AmbientSky;
				// half4 outCol = UniversalFragmentPBR(lightData, surface) + unity_AmbientSky;
				outCol.rgb = MixFog(outCol.rgb, lightData.fogCoord);
				outCol.a = OutputAlpha(outCol.a, IsSurfaceTypeTransparent(_Surface));

			// #if defined(LIGHTMAP_ON)
				// return outCol;
			// #else
				// return half4(1.0, 0.0, 0.0, 1.0);
			// #endif

			#ifdef _WRITE_RENDERING_LAYERS
				uint renderingLayers = GetMeshRenderingLayer();
				outRenderingLayers = float4(EncodeMeshRenderingLayer(renderingLayers), 0, 0, 0);
			#endif
			}

			ENDHLSL
		}

		/*
		Pass
		{
			Name "DepthOnly"
			Tags
			{
				"LightMode" = "DepthOnly"
			}

			// -------------------------------------
			// Render State Commands
			ZWrite[_ZWrite]
			ColorMask R
			Cull[_Cull]

			HLSLPROGRAM

			#pragma target 2.0

			// -------------------------------------
			// Shader Stages
			#pragma vertex PS1_DepthOnlyVertex
			#pragma fragment PS1_DepthOnlyVertex

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local _ALPHATEST_ON
			#pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

			// -------------------------------------
			// Unity defined keywords
			#pragma multi_compile _ LOD_FADE_CROSSFADE

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			// -------------------------------------
			// Includes
			// #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#if defined(LOD_FADE_CROSSFADE)
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
		#endif

			struct Attributes
			{
				float3 positionOS    : POSITION;

				float3 normalOS      : NORMAL;
				float4 tangentOS     : TANGENT;

				float2 uv            : TEXCOORD0;

				float2 staticLightmapUV        : TEXCOORD1;
				float2 dynamicLightmapUV       : TEXCOORD2;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS    : SV_POSITION;

				float2 uv            : TEXCOORD0;
				float3 positionWS    : TEXCOORD1;

			#ifdef _NORMALMAP
				half4 normalWS       : TEXCOORD2;
				half4 tangentWS      : TEXCOORD3;
				half4 bitangentWS    : TEXCOORD4;
			#else
				half3 normalWS       : TEXCOORD2;
			#endif

			// #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
			// 	half4 tangentWS      : TEXCOORD3;
			// #endif

			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				half4 fogFactorAndVertexLight : TEXCOORD5; // x: fogFactor, yzw: vertex light
			#else
				half fogFactor       : TEXCOORD5;
			#endif

				DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 6);

			#ifdef DYNAMICLIGHTMAP_ON
				float2 dynamicLightmapUV     : TEXCOORD7;
			#endif

			#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
				half3 viewDirTS      : TEXCOORD8;
			#endif

			#ifdef USE_APV_PROBE_OCCLUSION
				float4 probeOcclusion : TEXCOORD9;
			#endif

			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord   : TEXCOORD10;
			#endif

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#define _PS1_PREVERTEX_NO_POSITION_WS
			#define _PS1_PREVERTEX_NO_NORMAL_WS

			#include "PS1_PreVertex.cginc"
			#include "PS1_FragHelper.cginc"

			Varyings PS1_DepthOnlyVertex(Attributes i)
			{
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;

				UNITY_SETUP_INSTANCE_ID(i);
				Varyings o = PreVertex(i, _JitterGridScale, _PS1_PIXPARAMS.xy, vertexInput);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

			#if defined(_ALPHATEST_ON)
				o.uv = TRANSFORM_TEX(i.texcoord, _BaseMap);
			#endif

				o.positionCS = vertexInput.positionCS;

				return o;
			}

			half PS1_DepthOnlyFragment(Varyings i) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

			#if defined(_ALPHATEST_ON)
				Alpha(SampleAlbedoAlpha(i.uv, TEXTURE2D_ARGS(_BaseMap, _PS1_SAMPLER)).a, _BaseColor, _Cutoff);
			#endif

			#if defined(LOD_FADE_CROSSFADE)
				LODFadeCrossFade(i.positionCS);
			#endif

				return i.positionCS.z;
			}

			#undef _PS1_PREVERTEX_NO_POSITION_WS
			#undef _PS1_PREVERTEX_NO_NORMAL_WS

			ENDHLSL
		}
		*/

		/*

		Pass
		{
			Name "DepthOnly"
			Tags
			{
				"LightMode" = "DepthOnly"
			}

			ZWrite[_ZWrite]
			// ZWrite On
			ColorMask R
			Cull[_Cull]

			HLSLPROGRAM

			#pragma target 2.0

			// -------------------------------------
			// Shader Stages
			#pragma vertex DepthOnlyVertex
			#pragma fragment DepthOnlyFragment

			// -------------------------------------
			// Material Keywords
			#pragma shader_feature_local _ALPHATEST_ON
			#pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

			// -------------------------------------
			// Unity defined keywords
			#pragma multi_compile _ LOD_FADE_CROSSFADE

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			// -------------------------------------
			// Includes
			// #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
			// #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

		#if defined(LOD_FADE_CROSSFADE)
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
		#endif

			struct Attributes
			{
				float4 position     : POSITION;
				float2 texcoord     : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
			#if defined(_ALPHATEST_ON)
				float2 uv           : TEXCOORD0;
			#endif
				float4 positionCS   : SV_POSITION;

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings DepthOnlyVertex(Attributes i)
			{
				Varyings o = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_TRANSFER_INSTANCE_ID(i, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

			#if defined(_ALPHATEST_ON)
				o.uv = TRANSFORM_TEX(i.texcoord, _BaseMap);
			#endif

				o.positionCS = TransformObjectToHClip(i.position.xyz);
				o.positionCS = ApplyClipSpaceJitter(o.positionCS, _JitterGridScale, _PS1_PIXPARAMS.xy);

				return o;
			}

			half DepthOnlyFragment(Varyings i) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				#if defined(_ALPHATEST_ON)
					Alpha(SampleAlbedoAlpha(i.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade(i.positionCS);
				#endif

				return i.positionCS.z;
			}

			ENDHLSL
		}

		*/

		Pass
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			ZWrite On
			ZTest LEqual
			ColorMask 0
			Cull[_Cull]

			HLSLPROGRAM

			/*

			#pragma target 2.0

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

			*/

			#pragma target 2.0

			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment

			#pragma shader_feature_local _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			#pragma multi_compile_instancing
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			// #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"

			ENDHLSL
		}
	}

	Fallback "Hidden/Universal Render Pipeline/FallbackError"
	CustomEditor "piqey.PS1.PS1Shader"
}
