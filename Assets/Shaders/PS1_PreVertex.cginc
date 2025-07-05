inline Varyings PreVertex(Attributes i, float jitterGridScale, float2 screenSize)
{
	Varyings o;

	// VertexPositionInputs vInputs = GetPS1VertexPositionInputs(i.positionOS);
	VertexPositionInputs vInputs = GetPS1VertexPositionInputs(i.positionOS, jitterGridScale, screenSize.xy);

	o.positionCS = vInputs.positionCS;
#ifndef _PS1_PREVERTEX_NO_POSITION_WS
	o.positionWS = vInputs.positionWS;
#endif
#ifndef _PS1_PREVERTEX_NO_NORMAL_WS
	o.normalWS = TransformObjectToWorldNormal(i.normalOS);
#endif

	return o;
}
