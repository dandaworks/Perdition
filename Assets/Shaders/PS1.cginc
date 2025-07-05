// #ifndef __PS1_INC__
// #define __PS1_INC__

/*
	♦ GTE uses fixed-point math on all three axes (X, Y, Z) ♦

	The PS1's Geometry Transformation Engine (GTE) represents every coordinate in a fixed-point format
	(e.g., 1 sign bit, 3 integer bits, 12 fractional bits for each of X, Y, and Z) and carries out all
	transforms in that domain.


	♦ GPU rasterizer only accepts integer (X, Y) screen coordinates ♦

	After projection, the hardware drops the fractional bits from the X and Y components—rounding each
	vertex to the nearest whole pixel. As you pan the camera, vertices "stick" until their true sub-pixel
	position crosses the next integer boundary, then suddenly jump, creating the jitter.


	♦ Z is quantized for draw ordering, not for on-screen placement ♦

	The depth value is scaled and bucketed into the ordering table for painter-style sorting, but it isn't
	used to place pixels. That is, you won't see the same stepping in depth that you see in X and Y on-screen.

	♦ TLDR ♦

	In short, the visible vertex wobble is purely an X/Y sub-pixel artifact; Z‐axis precision plays no part in
	the 2D jitter we observe. That is, assuming it's reproduced accurate to the PlayStation 1.
*/

// #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#if _PS1_JITTER
	// uniform float _JitterGridScale;
	// #define JitterPos(v) round(v * _JitterGridScale) / _JitterGridScale
	#define JitterPos(v, scale) round(v * scale) / scale
#endif

// #if _PS1_PIXELBLIT
	// uniform float4 _PS1_PixelBlitParams;
	// #define _PS1_PIXPARAMS _PS1_PixelBlitParams
// #else
	// #define _PS1_PIXPARAMS _ScreenParams
// #endif

float4 ApplyClipSpaceJitter(float4 posCS, float jitterGridScale, float2 screenSize)
{
#if _PS1_JITTER
	// Sub-pixel grid jitter on X/Y
	// The grid density is controlled by _JitterGridScale
	// posCS.xy = JitterPos(posCS.xy);
	posCS.xy = JitterPos(posCS.xy, jitterGridScale);
#endif

#if _PS1_PIXELSNAP
	// Integer-pixel snap
	float2 ndc = posCS.xy / posCS.w; // Normalized device coords
	float2 pix = floor((ndc * 0.5 + 0.5) * screenSize.xy/*_PS1_PIXPARAMS.xy*/);
	ndc = (pix / screenSize.xy/*_PS1_PIXPARAMS.xy*/ - 0.5) * 2.0;
	posCS.xy = ndc * posCS.w;
#endif

	return posCS;
}

VertexPositionInputs GetPS1VertexPositionInputs(float3 posOS, float jitterGridScale, float2 screenSize)
{
	VertexPositionInputs inputs = GetVertexPositionInputs(posOS);

#if !_PS1_JITTER
	return inputs;
#else

	float4 clip = inputs.positionCS;

	clip = ApplyClipSpaceJitter(clip, jitterGridScale, screenSize.xy);

	inputs.positionCS = clip;

#if _PS1_PIXELSNAP
	// TODO: Should this actually be
	// float4(clip.xy / clip.w, clip.zw) ?
	inputs.positionNDC = float4(clip.xyz / clip.w, clip.w);
	// inputs.positionNDC = float4(clip.xy / clip.w, clip.zw);
#endif

	return inputs;
#endif
}

#undef _PS1_PIXPARAMS

// #endif // __PS1_INC__

/*
inline float4 PS1ObjectToHClip(float4 v)
{
	// In URP, we use TransformObjectToHClip to go from object to clip space
	float4 clip = TransformObjectToHClip(v); // v.xyz

#if _PS1_JITTER
	// Sub-pixel grid jitter on X/Y
	// The grid density is controlled by _JitterGridScale
	clip.xy = JitterPos(clip.xy);
#endif

#if _PS1_PIXELSNAP
	// Integer-pixel snap
	float2 ndc = clip.xy / clip.w; // Normalized device coords
	float2 pix = floor((ndc * 0.5 + 0.5) * _ScreenParams.xy);
	ndc = (pix / _ScreenParams.xy - 0.5) * 2.0;
	clip.xy = ndc * clip.w;
#endif

	return clip;
}
*/