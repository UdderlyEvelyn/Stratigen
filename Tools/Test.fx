float4x4 mWVP;
float4x4 mWL;
float4x4 mW;
Texture2D Texture : register(t0);
sampler TexSampler : register(s0);
float3 LightPosition;
float3 CameraPosition;
float ShadowDepthBias;
Texture2D ShadowMapA : register(t1);
sampler ShadowSamplerA : register(s1);
Texture2D ShadowMapB : register(t2);
sampler ShadowSamplerB : register(s2);

struct LitTextureVertex
{
	float3 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD1;
	float AmbientOcclusion : TEXCOORD2;
	float Reflectivity : TEXCOORD3;
};

struct RenderVTP
{
	float4 InternalPosition : POSITION0;
	float3 Normal : TEXCOORD0;
	float2 TexCoord	: TEXCOORD1;
	float3 Position2D : TEXCOORD2;
	float4 Position3D : TEXCOORD3;
	float4 PositionL : TEXCOORD4;
	float AmbientOcclusion : TEXCOORD5;
	float Reflectivity : TEXCOORD6;
};

struct DepthVTP
{
	float4 InternalPosition : POSITION0;
	float4 Position : TEXCOORD0;
};

struct CrepVTP
{
	float4 InternalPosition : POSITION0;
	float4 Position : TEXCOORD0;
};

struct BlurVTP
{
	float4 InternalPosition : POSITION0;
	float4 Position : TEXCOORD0;
};

/*RenderVTP vRender(LitTextureVertex vtx)
{
	RenderVTP vo = (RenderVTP)0;
	float4 pos4 = float4(vtx.Position, 1);
	vo.InternalPosition = mul(pos4, mWVP);
	vo.Position2D = vtx.Position;
	vo.Position3D = mul(vtx.Position, mW);
	vo.Normal = mul(vtx.Normal, mW).xyz;
	vo.PositionL = mul(float4(vtx.Position + (.099 * vtx.Normal), 1), mWL);
	//Passing values down the pipeline from vertex-level to pixel-level...
	vo.TexCoord = vtx.TexCoord;
	vo.AmbientOcclusion = vtx.AmbientOcclusion;
	vo.Reflectivity = vtx.Reflectivity;
	return vo;
}*/

RenderVTP vRender(LitTextureVertex vtx)
{
	RenderVTP vo = (RenderVTP)0;
	float4 pos4 = float4(vtx.Position, 1);
		vo.InternalPosition = mul(pos4, mWVP);
	vo.Position2D = vtx.Position;
	vo.Position3D = mul(vtx.Position, mW);
	vo.Normal = mul(vtx.Normal, (float3x3)mW);
	vo.PositionL = mul(pos4, mWL);
	//Passing values down the pipeline from vertex-level to pixel-level...
	vo.TexCoord = vtx.TexCoord;
	vo.AmbientOcclusion = vtx.AmbientOcclusion;
	vo.Reflectivity = vtx.Reflectivity;
	return vo;
}

//Phong-Blinn w/ Shadow Rendering And Some Custom Tweaking
/*float4 pRender(RenderVTP pxl) : COLOR0
{
	float ambient = .1;
	float diffuse = 0;
	float specular = 0;
	float4 texColor = tex2D(TexSampler, pxl.TexCoord);
	float2 shadowTexCoord = pxl.PositionL.xy / pxl.PositionL.w / 2 + .5;
	shadowTexCoord.y = 1 - shadowTexCoord.y;

	float sampledDepth = tex2D(ShadowSampler, shadowTexCoord).x;
	float actualDepth = 1 - pxl.PositionL.z / pxl.PositionL.w;
	if (actualDepth >= 0 && actualDepth > sampledDepth)
	{
		float3 normal = normalize(pxl.Normal);
		float3 lightDir = normalize(pxl.Position3D.xyz - LightPosition);
		diffuse = saturate(dot(normal, -lightDir));
		if (pxl.Reflectivity > 0) 
		{
			float3 h = normalize(normalize(CameraPosition - pxl.Position3D.xyz) - lightDir);
			specular = pow(saturate(dot(h, normal)), 1 + pxl.Reflectivity);
		}
	}

	return float4(texColor.xyz * (ambient + diffuse + specular), 1);
}*/

float4 pRender(RenderVTP pxl) : COLOR0
{
	float ambient = 1;
	float4 texColor = tex2D(TexSampler, pxl.TexCoord);
		float2 shadowTexCoord = pxl.PositionL.xy / pxl.PositionL.w / 2 + .5;
		shadowTexCoord.y = 1 - shadowTexCoord.y;
	float sampledDepth = (tex2D(ShadowSamplerA, shadowTexCoord).x + tex2D(ShadowSamplerB, shadowTexCoord).x) / 2;
	float actualDepth = 1 - pxl.PositionL.z / pxl.PositionL.w;
	float diff = max(0, (sampledDepth - actualDepth) * 100);
	float4 shadowedTexColor = texColor - (texColor * diff);
		float3 normal = normalize(pxl.Normal);
		float3 lightDir = -normalize(pxl.PositionL.xyz);
		//float diffuse = length(pxl.PositionL.xyz);
		float diffuse = (shadowedTexColor < texColor) ? 0 : saturate(dot(normal, lightDir));
	float specular = 0;
	if (pxl.Reflectivity > 0)
	{
		float3 h = normalize(normalize(CameraPosition - pxl.Position3D.xyz) + lightDir);
			specular = saturate(dot(h, normal) * pxl.Reflectivity);
	}
	return float4(shadowedTexColor.xyz * ambient * .7 + texColor.xyz * diffuse * .2 + shadowedTexColor.xyz * specular * .1 - pxl.AmbientOcclusion, texColor.w);
}

DepthVTP vDepth(LitTextureVertex vtx)
{
	DepthVTP vo = (DepthVTP)0;
	vo.InternalPosition = mul(float4(vtx.Position, 1), mWL);
	vo.Position = vo.InternalPosition;
	return vo;
}

float4 pDepth(DepthVTP pxl) : COLOR0
{
	return 1 - pxl.Position.z / pxl.Position.w;
}

CrepVTP vCrep(LitTextureVertex vtx)
{
	CrepVTP vo = (CrepVTP)0;
	vo.InternalPosition = mul(vtx.Position, mWVP);
	vo.Position = vo.InternalPosition;
	return vo;
}

float4 pCrep(CrepVTP pxl) : COLOR0
{
	return 1 - pxl.Position.z / 10;
}

BlurVTP vBlur(LitTextureVertex vtx)
{
	BlurVTP vo = (BlurVTP)0;
	vo.InternalPosition = mul(vtx.Position, mWVP);
	vo.Position = vo.InternalPosition;
	return vo;
}

float4 pBlur(BlurVTP pxl) : COLOR0
{
	return pxl.Position.z;
}

//Vertex shader must write out POSITION error? Check compile declarations for "vs" where "ps" should be.
technique Pre
{
	pass Depth
	{
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
    	VertexShader = compile vs_2_0 vDepth();
        PixelShader  = compile ps_2_0 pDepth();
	}
}

technique Render
{
    pass Opaque
    {  
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
    	VertexShader = compile vs_2_0 vRender();
        PixelShader  = compile ps_2_0 pRender();
    }
}

technique Post
{
	pass Crep
	{
		VertexShader = compile vs_2_0 vCrep();
		PixelShader	 = compile ps_2_0 pCrep();
	}
	pass Blur
	{
		VertexShader = compile vs_2_0 vBlur();
		PixelShader  = compile ps_2_0 pBlur();
	}
}
