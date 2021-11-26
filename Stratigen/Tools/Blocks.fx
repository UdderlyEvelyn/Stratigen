float4x4 mWVP;
float4x4 mWL;
float4x4 mW;
Texture2D Texture : register(t0);
sampler TexSampler : register(s0);
float3 LightPosition;
float3 CameraPosition;
float Sunlight;
float WaveAmplitude;
float WaveFrequency;
float Phase;
float4 SkyColor;
//float4 ChromaKey;
float FarClip;
Texture2D RenderBuffer : register(t1);
sampler RenderSampler : register(s1);
Texture2D ShadowMap : register(t2);
sampler ShadowMapSampler : register(s2);
Texture2D ShadowBuffer : register(t3);
sampler ShadowSampler : register(s3);
Texture2D DepthMap : register(t4);
sampler DepthSampler : register(s4);
Texture2D NearShadowMap : register(t5);
sampler NearShadowMapSampler : register(s5);
Texture2D FarShadowMap : register(t6);
sampler FarShadowMapSampler : register(s6);
int Hour;

struct TextureVertex
{
	float3 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD1;
};

struct LitTextureVertex
{
	float3 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD1;
	float AmbientOcclusion : TEXCOORD2;
	float Reflectivity : TEXCOORD3;
};

struct PostVTP
{
	float4 InternalPosition : POSITION0;
	float2 TexCoord : TEXCOORD1;
};

struct RenderVTP
{
	float4 InternalPosition : POSITION0;
	float3 Normal : TEXCOORD0;
	float2 TexCoord	: TEXCOORD1;
	float AmbientOcclusion : TEXCOORD2;
	float Reflectivity : TEXCOORD3;
	float3 Position2D : TEXCOORD4;
	float4 Position3D : TEXCOORD5;
	float4 PositionL : TEXCOORD6;
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
	float2 Position : TEXCOORD0;
};

PostVTP vPost(TextureVertex vtx)
{
	PostVTP vo = (PostVTP)0;
	vo.InternalPosition = float4(vtx.Position.x, -vtx.Position.y, vtx.Position.z, 1);
	vo.TexCoord = vtx.TexCoord;
	return vo;    
}

float4 pRaw(PostVTP pxl) : COLOR0
{
	return tex2D(RenderSampler, pxl.TexCoord);
}

/*float4 pSSFX(PostVTP pxl) : COLOR0
{
	float4 renderColor = tex2D(RenderSampler, pxl.TexCoord);
	float shadow = tex2D(ShadowSampler, pxl.TexCoord).x;
	return (renderColor == ChromaKey ? SkyColor : float4((renderColor.xyz * shadow), 1));
}*/

float4 pLighting(PostVTP pxl) : COLOR0
{
	float4 renderColor = tex2D(RenderSampler, pxl.TexCoord);
	float shadow = tex2D(ShadowSampler, pxl.TexCoord).x;
	return float4(renderColor.xyz * shadow, 1);
}

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

//Phong-Blinn w/ Some Custom Tweaking
float4 pRender(RenderVTP pxl) : COLOR0
{
	float4 texColor = tex2D(TexSampler, pxl.TexCoord);
	float3 normal = normalize(pxl.Normal);
	float3 lightDir = normalize(LightPosition - pxl.Position3D.xyz);
	float specular = 0;	
	float diffuse = saturate(dot(normal, lightDir));
	if (pxl.Reflectivity > 0)
	{
		float3 h = normalize(normalize(CameraPosition - pxl.Position3D.xyz) - lightDir);
		specular = saturate(dot(h, normal) * pxl.Reflectivity);
	}
	return float4(texColor.xyz * Sunlight * (diffuse + specular + pxl.AmbientOcclusion), texColor.w);
}

//Shadow Renderer
float4 pShadow(RenderVTP pxl) : COLOR0
{
	float2 shadowTexCoord = pxl.PositionL.xy / pxl.PositionL.w / 2 + .5;
	shadowTexCoord.y = 1 - shadowTexCoord.y;
	float actualDepth = 1 - pxl.PositionL.z / pxl.PositionL.w;
	float sampledDepth = tex2D(ShadowMapSampler, shadowTexCoord).x;
	//if (actualDepth <= 1000) sampledDepth = tex2D(NearShadowMapSampler, shadowTexCoord).x;
	//else if (actualDepth > 1000) sampledDepth = tex2D(FarShadowMapSampler, shadowTexCoord).x;
	float light = 1;
	float falloff = length(pxl.PositionL.xyz) / 1500;
	if (actualDepth + .000001 - sampledDepth < 0) light = .7;
	return light - light * falloff;
}

RenderVTP vWaves(LitTextureVertex vtx)
{
	RenderVTP vo = (RenderVTP)0;
	float wA = cos(Phase) * WaveAmplitude;
	float wF = Phase * WaveFrequency;
	vtx.Position.y += -(WaveAmplitude / 2) + (wA * sin(vtx.Position.x - wF) * sin(vtx.Position.z - wF));
	float4 pos4 = float4(vtx.Position, 1);
		vo.InternalPosition = mul(pos4, mWVP);
	vo.Position2D = vtx.Position;
	vo.Position3D = mul(vtx.Position, mW);
	vo.Normal = mul(vtx.Normal, mW).xyz;
	vo.PositionL = mul(float4(vtx.Position /*+ (.7 * vtx.Normal)*/, 1), mWL);
	//Passing values down the pipeline from vertex-level to pixel-level...
	vo.TexCoord = vtx.TexCoord;
	vo.AmbientOcclusion = vtx.AmbientOcclusion;
	vo.Reflectivity = vtx.Reflectivity;
	return vo;
}

//Phong-Blinn w/ Simulated Water Effects
float4 pWaves(RenderVTP pxl) : COLOR0
{
	float4 texColor = tex2D(TexSampler, pxl.TexCoord);
	float3 normal = normalize(pxl.Normal);
	float3 lightDir = normalize(LightPosition - pxl.Position3D.xyz);
	float specular = 0;
	float highlight = 0;
	float diffuse = saturate(dot(normal, lightDir));
	if (pxl.Reflectivity > 0)
	{
		float3 h = normalize(normalize(CameraPosition - pxl.Position3D.xyz) - lightDir);
		float3 h2 = normalize(normalize(CameraPosition - pxl.Position3D.xyz) + lightDir);
		specular = saturate(dot(h, normal) * pxl.Reflectivity);
		highlight = saturate(dot(h2, normal) * pxl.Reflectivity);
	}
	float antiReflectivity = 1 - pxl.Reflectivity;
	return float4(Sunlight * ((texColor.xyz * diffuse * antiReflectivity) + (SkyColor.xyz * specular) + (SkyColor.xyz * highlight)), texColor.w);
}

DepthVTP vLightDepth(LitTextureVertex vtx)
{
	DepthVTP vo = (DepthVTP)0;
	vo.InternalPosition = mul(float4(vtx.Position, 1), mWL);
	vo.Position = vo.InternalPosition;
	return vo;
}

float4 pLightDepth(DepthVTP pxl) : COLOR0
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
	//http://xnauk-randomchaosblogarchive.blogspot.com/2012/07/crepuscular-god-rays-and-web-ui-sample.html
	return 1 - pxl.Position.z / 10;
}

float4 pBlur(PostVTP pxl) : COLOR0
{
	float blurRange = .015;
	return float4(
		tex2D(RenderSampler, float2(pxl.TexCoord.x + blurRange, pxl.TexCoord.y + blurRange)).xyz * .25 +
		tex2D(RenderSampler, float2(pxl.TexCoord.x - blurRange, pxl.TexCoord.y - blurRange)).xyz * .25 +
		tex2D(RenderSampler, float2(pxl.TexCoord.x + blurRange, pxl.TexCoord.y - blurRange)).xyz * .25 +
		tex2D(RenderSampler, float2(pxl.TexCoord.x - blurRange, pxl.TexCoord.y + blurRange)).xyz * .25
		, 1);
}

float4 pShadowBlur(PostVTP pxl) : COLOR0
{
	float blurRange = .015;
	return float4(
		tex2D(ShadowSampler, float2(pxl.TexCoord.x + blurRange, pxl.TexCoord.y + blurRange)).xyz * .25 +
		tex2D(ShadowSampler, float2(pxl.TexCoord.x - blurRange, pxl.TexCoord.y - blurRange)).xyz * .25 +
		tex2D(ShadowSampler, float2(pxl.TexCoord.x + blurRange, pxl.TexCoord.y - blurRange)).xyz * .25 +
		tex2D(ShadowSampler, float2(pxl.TexCoord.x - blurRange, pxl.TexCoord.y + blurRange)).xyz * .25, 1);
}

float4 pFog(PostVTP pxl) : COLOR0
{
	float4 existingColor = tex2D(RenderSampler, pxl.TexCoord);
	float depth = tex2D(DepthSampler, pxl.TexCoord).x;
	return float4(existingColor.xyz * depth + SkyColor.xyz * (1 - depth), existingColor.w);
}

//Vertex shader must write out POSITION error? Check compile declarations for "vs" where "ps" should be.
technique Pre
{
	pass LightDepth
	{
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		VertexShader = compile vs_2_0 vLightDepth();
		PixelShader = compile ps_2_0 pLightDepth();
	}
}

technique Render
{
	pass Opaque
	{
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		VertexShader = compile vs_2_0 vRender();
		PixelShader = compile ps_2_0 pRender();
	}

	pass Transparent
	{
		BlendOp = ADD;
		AlphaBlendEnable = TRUE;
		SrcBlend = SRCALPHA;
		DestBlend = INVSRCALPHA;
		ZEnable = TRUE;
		ZWriteEnable = FALSE;
		VertexShader = compile vs_2_0 vRender();
		PixelShader = compile ps_2_0 pRender();
	}

	pass Liquid
	{
		BlendOp = ADD;
		AlphaBlendEnable = TRUE;
		SrcBlend = SRCALPHA;
		DestBlend = INVSRCALPHA;
		ZEnable = TRUE;
		ZWriteEnable = FALSE;
		VertexShader = compile vs_2_0 vWaves();
		PixelShader = compile ps_2_0 pWaves();
	}

	pass Shadow
	{
		BlendOp = MIN;
		AlphaBlendEnable = TRUE;
		//Note that Src and Dest blends are set to something specific but unknown, and SRCALPHA/INVSRCALPHA do not produce the proper results.
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		VertexShader = compile vs_2_0 vRender();
		PixelShader = compile ps_2_0 pShadow();
	}
}

technique Post
{
	pass Raw
	{
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		VertexShader = compile vs_2_0 vPost();
		PixelShader = compile ps_2_0 pRaw();
	}

	/*pass SSFX
	{
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		VertexShader = compile vs_2_0 vPost();
		PixelShader = compile ps_2_0 pSSFX();
	}*/

	pass Lighting
	{
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		VertexShader = compile vs_2_0 vPost();
		PixelShader = compile ps_2_0 pLighting();
	}

	pass ShadowBlur
	{
		VertexShader = compile vs_2_0 vPost();
		PixelShader = compile ps_2_0 pShadowBlur();
	}

	pass Blur
	{
		VertexShader = compile vs_2_0 vPost();
		PixelShader = compile ps_2_0 pBlur();
	}
}
