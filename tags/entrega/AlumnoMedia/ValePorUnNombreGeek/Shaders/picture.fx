
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

texture g_frame;
sampler2D frame = sampler_state
{
	Texture = (g_frame);
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
	float alphaX = alpha(input.Maskcoord);
	if(alphaX<255) color[3]=alphaX;
	return color;
}




float4 ps_luminance(PS_INPUT input): COLOR0
{
	float4 color = tex2D(diffuseMap, input.Texcoord);
	color =  0.1*color.x + 0.95*color.y+0.2*color.z;
	float alphaX = alpha(input.Maskcoord);
	if(alphaX<255) color[3]=alphaX;
	return color;
}

float4 selectionColor;

float4 ps_selected(PS_INPUT input):COLOR0
{
	float4 luminance = ps_luminance(input);
	float4 color =  selectionColor*0.5 + luminance;
	float alphaX = alpha(input.Maskcoord);
	if(alphaX<255) color[3]=alphaX;
	
	return color;
}



float4 ps_frame(float2 framecoord:TEXCOORD1):COLOR0
{
	
	return tex2D(frame, framecoord);
}




technique DIFFUSE_MAP
{
    pass p0
    {        
		PixelShader = compile ps_3_0  ps_DiffuseMap();
    }
}

technique BLACK_WHITE
{
    pass p0
    {        
		PixelShader = compile ps_3_0  ps_luminance();
    }
}

technique SELECTED
{
    pass p0
    {        
		PixelShader = compile ps_3_0  ps_selected();
    }
}




 technique FRAME
{
   pass Pass_0
   {
	  
	  PixelShader = compile ps_2_0 ps_frame();
   }
}

