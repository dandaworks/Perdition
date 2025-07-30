// #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

inline void PS1_InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
{
	inputData = (InputData)0;

	inputData.positionWS = input.positionWS;
#if defined(DEBUG_DISPLAY)
	inputData.positionCS = input.positionCS;
#endif

#ifdef _NORMALMAP
	// IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
	// float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
	// float3 bitangent = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);

	half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
	inputData.tangentToWorld = half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz);
	inputData.normalWS = TransformTangentToWorld(normalTS, inputData.tangentToWorld);
#else
	half3 viewDirWS = GetWorldSpaceNormalizeViewDir(inputData.positionWS);
	inputData.normalWS = input.normalWS;
#endif

	inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
	viewDirWS = SafeNormalize(viewDirWS);

	inputData.viewDirectionWS = viewDirWS;

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
	inputData.shadowCoord = input.shadowCoord;
#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
	inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
#else
	inputData.shadowCoord = float4(0, 0, 0, 0);
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
	inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactorAndVertexLight.x);
	inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
#else
	inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactor);
	inputData.vertexLighting = half3(0, 0, 0);
#endif

	inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

#if defined(DEBUG_DISPLAY)
#if defined(DYNAMICLIGHTMAP_ON)
	inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
#endif
#if defined(LIGHTMAP_ON)
	inputData.staticLightmapUV = input.staticLightmapUV;
#else
	inputData.vertexSH = input.vertexSH;
#endif
#if defined(USE_APV_PROBE_OCCLUSION)
	inputData.probeOcclusion = input.probeOcclusion;
#endif
#endif
}

inline void PS1_InitializeStandardLitSurfaceData(float2 uv, inout SurfaceData surf)
{
	half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
	surf.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);
	surf.alpha = AlphaDiscard(surf.alpha, _Cutoff);

	// half4 specGloss = SampleMetallicSpecGloss(uv, albedoAlpha.a);
	surf.albedo = albedoAlpha.rgb * _BaseColor.rgb;
	surf.albedo = AlphaModulate(surf.albedo, surf.alpha);

// #if _SPECULAR_SETUP
// 	surf.metallic = half(1.0);
// 	surf.specular = specGloss.rgb;
// #else
	// surf.metallic = specGloss.r;
	// surf.specular = half3(0.0, 0.0, 0.0);
// #endif

	surf.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, _PS1_SAMPLER_BUMP), _BumpScale);
	surf.emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, _PS1_SAMPLER_EMISSION)) * _EmissionColor.a;

	surf.smoothness = 0.0;
	surf.metallic = 0.0;
	surf.specular = half3(0.0, 0.0, 0.0);
	surf.occlusion = 1.0;

	// surf.occlusion = SampleOcclusion(uv);

// #if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
// 	half2 clearCoat = SampleClearCoat(uv);
// 	surf.clearCoatMask       = clearCoat.r;
// 	surf.clearCoatSmoothness = clearCoat.g;
// #else
// 	surf.clearCoatMask       = half(0.0);
// 	surf.clearCoatSmoothness = half(0.0);
// #endif

// #if defined(_DETAIL)
// 	half detailMask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, uv).a;
// 	float2 detailUv = uv * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
// 	surf.albedo = ApplyDetailAlbedo(detailUv, surf.albedo, detailMask);
// 	surf.normalTS = ApplyDetailNormal(detailUv, surf.normalTS, detailMask);
// #endif
}

inline void PS1_InitializeBakedGIData(Varyings input, inout InputData inputData)
{
	#if defined(DYNAMICLIGHTMAP_ON)
	inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV, input.vertexSH, inputData.normalWS);
	inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
	#elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
	inputData.bakedGI = SAMPLE_GI(input.vertexSH,
		GetAbsolutePositionWS(inputData.positionWS),
		inputData.normalWS,
		inputData.viewDirectionWS,
		input.positionCS.xy,
		input.probeOcclusion,
		inputData.shadowMask);
	#else
	inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, inputData.normalWS);
	inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
	#endif
}
