float4x4 WVP;

struct VertexToPixel
{
	float4 InternalPosition		: POSITION0;
	float  Depth				: TEXCOORD0;
};

struct 

VertexToPixel vShader(float4 inPos: POSITION0, float3 inNormal : NORMAL0, float2 inTexCo : TEXCOORD1, float inLight : TEXCOORD2)
{
	VertexToPixel Output = (VertexToPixel)0;	
	
	float4 pos = mul(inPos, WVP);
	Output.InternalPosition = pos;
	Output.Depth = 1 - pos.z / pos.w;

	return Output;
}

float4 pShader(VertexToPixel PSIn) : COLOR0
{
	return PSIn.Depth;
}

technique Map
{
    pass Pass0
	{
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		AlphaBlendEnable = FALSE;
		FillMode = Solid;
		CullMode = None;
    	VertexShader = compile vs_2_0 vShader();
        PixelShader  = compile ps_2_0 pShader();
    }
}
