//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = BORDER;
	ADDRESSV = BORDER;
	BORDERCOLOR = 0;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

texture g_Posiciones;
bool show_characters;
sampler2D posiciones = sampler_state
{
	Texture = (g_Posiciones);
	ADDRESSU = BORDER;
	ADDRESSV = BORDER;
	BORDERCOLOR = 0;
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

float2x2 rotation;


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



/**************************************************************************************/
/* MAPA */
/**************************************************************************************/

float alpha(float2 maskcoord){
	
	float alpha;
	if(mask_enable){
		float4 maskColor = tex2D(mask, maskcoord);
		alpha = 0.1*maskColor.x + 0.95*maskColor.y+0.2*maskColor.z;
	}else alpha = 255;

	return alpha;
}

struct PS_INPUT
{
	float2 Mapcoord : TEXCOORD0;   
	float2 Maskcoord : TEXCOORD1;
};


float4 ps_posiciones(PS_INPUT input):COLOR0
{
	
	float4 color = tex2D(posiciones,  input.Mapcoord);
	if(!show_characters) color = 0;
	
	return color;

}



//Mapita sin efectos
float4 ps_mapa(PS_INPUT input) : COLOR0
{      
	float4 color = ps_posiciones(input);
	
	if(color.r+color.g+color.b <0.01) color = tex2D(diffuseMap, input.Mapcoord);
	
	color[3] = alpha(input.Maskcoord);
	return color;

	
}

//Tiñe el mapa 
float4 sepia = float4(0.64, 0.55, 0.4, 1);
float4 ps_mapa_viejo(PS_INPUT input) : COLOR0
{    
	float4 color = ps_posiciones(input);
	
	if(color.r+color.g+color.b <0.01){
		float4 fvBaseColor = tex2D(diffuseMap, input.Mapcoord);
		float luminance = (0.1*fvBaseColor.x + 0.95*fvBaseColor.y+0.2*fvBaseColor.z)+0.2; 
	
		color = luminance*sepia;
	}
	color[3] = alpha(input.Maskcoord);
	return color;	
	
}

technique MAPA
{
   pass Pass_0
   {
		 PixelShader = compile ps_2_0 ps_mapa();
   }
}


 technique MAPA_VIEJO
{
   pass Pass_0
   {
	 
	  PixelShader = compile ps_2_0 ps_mapa_viejo();
   }
}


/**************************************************************************************/
/* MARCO */
/**************************************************************************************/


float4 ps_frame(float2 framecoord:TEXCOORD1):COLOR0
{
	
	return tex2D(frame, framecoord);
}

 technique FRAME
{
   pass Pass_0
   {
	  
	  PixelShader = compile ps_2_0 ps_frame();
   }
}




