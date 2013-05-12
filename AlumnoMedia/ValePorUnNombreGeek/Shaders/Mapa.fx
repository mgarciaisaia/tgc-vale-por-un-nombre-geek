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
	BORDERCOLOR = 255;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};

//Input del Vertex Shader
struct VS_INPUT
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
   float2 Maskcoord: TEXCOORD1;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
   float2 Maskcoord : TEXCOORD1;
};


//Vertex Shader
VS_OUTPUT vs_mapa(VS_INPUT input)
{
	VS_OUTPUT output;

	
	output.Position = input.Position;

	
	output.Texcoord = input.Texcoord;

	output.Maskcoord = input.Maskcoord;

	return output;
}


struct PS_INPUT
{
	float2 Texcoord : TEXCOORD0;   
	float2 Maskcoord : TEXCOORD1;
};

float alpha(float2 maskcoord){

	float4 maskColor = tex2D(mask, maskcoord);
	return 0.1*maskColor.x + 0.95*maskColor.y+0.2*maskColor.z;
}

//Desvuelve el color de la textura
float4 ps_mapa(PS_INPUT input) : COLOR0
{      
	float4 color = tex2D(diffuseMap, input.Texcoord);
	color[3] = alpha(input.Maskcoord);
	return color;

	
}

//Tiñe el mapa 
float4 sepia = float4(0.64, 0.55, 0.4, 1);
float4 ps_mapa_viejo(PS_INPUT input) : COLOR0
{    
	
	float4 fvBaseColor = tex2D(diffuseMap, input.Texcoord);
	float4 fvHeight = tex2D(heightMap, input.Texcoord);
	float luminance = (0.1*fvBaseColor.x + 0.95*fvBaseColor.y+0.2*fvBaseColor.z)+0.2; 
	float height = (0.1*fvHeight.x + 0.95*fvHeight.y+0.2*fvHeight.z)+0.2; 
	float4 color = (luminance + 0.6 - height)*sepia;
	color[3] = alpha(input.Maskcoord);
	return color;	
	
}

float4 ps_posiciones(PS_INPUT input):COLOR0
{
	
	float4 color = tex2D(posiciones, input.Texcoord);
	if(color.r+color.g+color.b <0.01) color = ps_mapa(input);
		else color[3] = alpha(input.Maskcoord);
	
	return color;

}

float4 ps_posiciones_viejo(PS_INPUT input):COLOR0
{
	
	float4 color = tex2D(posiciones, input.Texcoord);
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




