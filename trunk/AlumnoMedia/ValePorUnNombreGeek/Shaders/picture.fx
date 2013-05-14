
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

texture g_mask;
bool mask_enable;
sampler2D mask = sampler_state
{
	Texture = (g_mask);
	ADDRESSU = BORDER;
	ADDRESSV = BORDER;
	BORDERCOLOR = 0;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

struct PS_INPUT
{
	float2 Texcoord : TEXCOORD0;   
	float2 Maskcoord : TEXCOORD1;
};

float alpha(float2 maskcoord){
	
	float alpha;
	if(mask_enable){
		float4 maskColor = tex2D(mask, maskcoord);
		alpha = 0.1*maskColor.x + 0.95*maskColor.y+0.2*maskColor.z;
	}else alpha = 255;

	return alpha;
}

float4 ps_DiffuseMap(PS_INPUT input) : COLOR0
{      
	float4 color = tex2D(diffuseMap, input.Texcoord);
	color[3] = alpha(input.Maskcoord);
	return color;
}

technique DIFFUSE_MAP
{
    pass p0
    {        
		PixelShader = compile ps_3_0  ps_DiffuseMap();
    }
}
