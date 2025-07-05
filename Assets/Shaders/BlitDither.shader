Shader "Custom/Screenspace/BlitBlueNoiseDither"
{
	HLSLINCLUDE

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

	#pragma multi_compile_fragment _ _PS1_DITHER
	#pragma multi_compile_fragment _ _PS1_QUANTIZE
	// #pragma shader_feature _POINTSAMPLE_FIX_ON

	// #define DITHER_OR_QUANT (defined(_PS1_DITHER) || defined(_PS1_QUANTIZE))

	// uniform sampler2D _BlitTexture;
	// uniform float4 _BlitTexture_ST;
	// uniform float4 _BlitTexture_TexelSize;

	// SamplerState SmpClampPoint;

	uniform sampler2D _NoiseTex;
	uniform float4 _NoiseTex_ST;
	uniform float4 _NoiseTex_TexelSize;

#if _PS1_DITHER
	uniform float _DitherTune;
#endif
#if (_PS1_DITHER || _PS1_QUANTIZE) // DITHER_OR_QUANT
	uniform float _ColorDepth;
#endif

#if _PS1_DITHER
	// Blue-Noise Dither
	inline float3 DitherPattern(float3 col, float2 uv)
	{
		// Calculate tiling based on texture dimensions
		// _NoiseTex_TexelSize.xy contains (1/W, 1/H)
		// _BlitTexture_TexelSize.xy contains (1/W, 1/H)
		float2 autoTiling = _BlitTexture_TexelSize.xy / _NoiseTex_TexelSize.xy;

		// Apply tiling
		float2 noiseUV = uv / autoTiling + _NoiseTex_ST.zw;
		// Fix blue noise aspect ratio
		noiseUV *= _ScreenParams.x / _ScreenParams.y;

		// Sample noise
		float3 noise = tex2D(_NoiseTex, noiseUV).rgb;

		// compute threshold:
		// multiplier = 2^(8 - bits) – 1
		float mult = pow(2.0, 8.0 - _ColorDepth) - 1.0;

		// center noise around zero and scale
		float3 thresh = (noise - 0.5) * ((mult + _DitherTune) / 255.0);

		return col + thresh; // noise;
	}
#endif

#if _PS1_QUANTIZE
	// Quantize to N-bit per channel
	inline float3 ColorDepthReduction(float3 col)
	{
		float levels = pow(2.0, _ColorDepth) - 1.0;
		col *= levels;
		col = floor(col) + step(0.5, frac(col));

		return col / levels;
	}
#endif

	float4 FragPass1(Varyings i) : SV_Target
	{
		// sample original
		float2 uv = i.texcoord;

	// #if _POINTSAMPLE_FIX_ON
		// float4 col = _BlitTexture.Sample(SmpClampPoint, uv);
	// #else
		float4 col = FragNearest(i);
	// #endif

	#if _PS1_DITHER
		// apply blue-noise dither
		col = float4(DitherPattern(col.rgb, uv), col.a);
	#endif

	#if _PS1_QUANTIZE
		// reduce to N-bit
		col = float4(ColorDepthReduction(col.rgb), col.a);
	#endif

		return col;
	}

	ENDHLSL

	Properties
	{
		[MainTexture] _BlitTexture ("Source (RGB)", 2D) = "white" {}
		_NoiseTex ("Blue Noise Tex", 2D) = "white" {}
		[Toggle(_PS1_DITHER)] _Dither ("Dithering", Float) = 1
		_DitherTune ("Dither Tune", Range(-64, 64)) = 0
		[Toggle(_PS1_QUANTIZE)] _Quantize ("Color Quantization", Float) = 1
		_ColorDepth ("Color Depth (bits)", Range(1, 8)) = 6
		// [Toggle] _POINTSAMPLE_FIX ("Point Sample Fix", Float) = 0
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"RenderPipeline" = "UniversalPipeline"
		}

		ZWrite Off
		Cull Off

		Pass
		{
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragNearest

			ENDHLSL
		}

		Pass
		{
			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment FragPass1

			ENDHLSL
		}
	}
}
