//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

float4 ps_DiffuseMap(float2 texcoord:TEXCOORD0) : COLOR0
{      
	
	return tex2D(diffuseMap, texcoord);
}

technique DIFFUSE_MAP
{
    pass p0
    {        
		PixelShader = compile ps_3_0  ps_DiffuseMap();
    }
}
