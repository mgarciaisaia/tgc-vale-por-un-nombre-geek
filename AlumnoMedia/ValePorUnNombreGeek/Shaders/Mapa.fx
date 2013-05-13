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


texture texHeightMap;
sampler2D heightMap = sampler_state
{
	Texture = (texHeightMap);
	ADDRESSU = BORDER;
	ADDRESSV = BORDER;
	BORDERCOLOR = 0;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

texture g_mask;
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
/* Posiciones de los personajes*/
/**************************************************************************************/


struct VS_INPUT_TransformedColored
{
   float4 Position : POSITION0;
   float4 Color : COLOR0;
};


//Vertex Shader
VS_INPUT_TransformedColored vs_TransformedColored(VS_INPUT_TransformedColored input)
{
	return input;
}


float4 ps_TransformedColored(float4 Color : COLOR0) : COLOR0
{      
	return Color;
}


technique POSICIONES
{
	pass Pass_0
	{
	  VertexShader = compile vs_2_0 vs_TransformedColored();
	  PixelShader = compile ps_2_0 ps_TransformedColored();
   }
}



/**************************************************************************************/
/* MAPA */
/**************************************************************************************/
struct VS_INPUT
{
   float4 Position : POSITION0;
   float2 Mapcoord : TEXCOORD0;
   float2 Maskcoord: TEXCOORD1;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
   float4 Position : POSITION0;
   float2 Mapcoord : TEXCOORD0;
   float2 Maskcoord : TEXCOORD1;
};



VS_OUTPUT vs_mapa(VS_INPUT input)
{
	VS_OUTPUT output;

	
	output.Position = input.Position;

	
	output.Mapcoord = input.Mapcoord;


	output.Maskcoord = input.Maskcoord;

	return output;
}


float alpha(float2 maskcoord){

	float4 maskColor = tex2D(mask, maskcoord);
	return 0.1*maskColor.x + 0.95*maskColor.y+0.2*maskColor.z;
}



struct PS_INPUT
{
	float2 Mapcoord : TEXCOORD0;   
	float2 Maskcoord : TEXCOORD1;
};

//Mapita sin efectos
float4 ps_mapa(PS_INPUT input) : COLOR0
{      
	float4 color = tex2D(diffuseMap, input.Mapcoord);
	color[3] = alpha(input.Maskcoord);
	return color;

	
}

//Ti�e el mapa 
float4 sepia = float4(0.64, 0.55, 0.4, 1);
float4 ps_mapa_viejo(PS_INPUT input) : COLOR0
{    
	float4 fvBaseColor = tex2D(diffuseMap, input.Mapcoord);
	float4 fvHeight = tex2D(heightMap,input.Mapcoord);
	float luminance = (0.1*fvBaseColor.x + 0.95*fvBaseColor.y+0.2*fvBaseColor.z)+0.2; 
	float height = (0.1*fvHeight.x + 0.95*fvHeight.y+0.2*fvHeight.z)+0.2; 
	float4 color = (luminance + 0.6 - height)*sepia;
	color[3] = alpha(input.Maskcoord);
	return color;	
	
}

//Para cuando se muestran las posiciones:

float4 ps_posiciones(PS_INPUT input):COLOR0
{
	
	float4 color = tex2D(posiciones,  input.Mapcoord);
	if(color.r+color.g+color.b <0.01) color = ps_mapa(input);
		else color[3] = alpha(input.Maskcoord);
	
	return color;

}


float4 ps_posiciones_viejo(PS_INPUT input):COLOR0
{
	
	float4 color = tex2D(posiciones,  input.Mapcoord);
	if(color.r+color.g+color.b <0.01) color = ps_mapa_viejo(input);
		else color[3] = alpha(input.Maskcoord);
	return color;
}






 technique MAPA
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_mapa();
	  PixelShader = compile ps_2_0 ps_mapa();
   }
}


 technique MAPA_VIEJO
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_mapa();
	  PixelShader = compile ps_2_0 ps_mapa_viejo();
   }
}

 technique MAPA_POSICIONES
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_mapa();
	  PixelShader = compile ps_2_0 ps_posiciones();
   }
}


 technique MAPA_VIEJO_POSICIONES
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_mapa();
	  PixelShader = compile ps_2_0 ps_posiciones_viejo();
   }
}





/**************************************************************************************/
/* MARCO */
/**************************************************************************************/


struct VS_FRAME
{
   float4 Position : POSITION0;
   float2 Framecoord: TEXCOORD1;
};



VS_FRAME vs_frame(VS_FRAME input)
{
	return input;
}


float4 ps_frame(float2 framecoord:TEXCOORD1):COLOR0
{
	
	return tex2D(frame, framecoord);
}

 technique FRAME
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_frame();
	  PixelShader = compile ps_2_0 ps_frame();
   }
}




