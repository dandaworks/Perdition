inline Varyings PreVertex(Attributes i, float jitterGridScale, float2 screenSize, out VertexPositionInputs vertexInput)
{
	Varyings o;

	UNITY_TRANSFER_INSTANCE_ID(i, o);

	// VertexPositionInputs vInputs = GetPS1VertexPositionInputs(i.positionOS);
	vertexInput = GetPS1VertexPositionInputs(i.positionOS, jitterGridScale, screenSize.xy);

	o.positionCS = vertexInput.positionCS;
#ifndef _PS1_PREVERTEX_NO_POSITION_WS
	o.positionWS = vertexInput.positionWS;
#endif
#ifndef _PS1_PREVERTEX_NO_NORMAL_WS
	#ifdef _NORMALMAP
		o.normalWS = half4(TransformObjectToWorldNormal(i.normalOS), 0.0);
	#else
		o.normalWS = (half3)TransformObjectToWorldNormal(i.normalOS);
	#endif
#endif

	return o;
}

inline Varyings PreVertex(Attributes i, float jitterGridScale, float2 screenSize)
{
	VertexPositionInputs _ = (VertexPositionInputs)0;
	return PreVertex(i, jitterGridScale, screenSize, _);
}
