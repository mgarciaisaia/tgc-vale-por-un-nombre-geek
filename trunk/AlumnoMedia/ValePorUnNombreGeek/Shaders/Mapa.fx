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

//Input del Vertex Shader
struct VS_INPUT
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
   float4 Position : POSITION0;
   float2 Texcoord : TEXCOORD0;
};


//Vertex Shader
VS_OUTPUT vs_mapa(VS_INPUT input)
{
	VS_OUTPUT output;

	
	output.Position = input.Position;

	
	output.Texcoord = input.Texcoord;

	return output;
}

//Input del Pixel Shader
struct PS_INPUT
{
	
	float2 Texcoord : TEXCOORD0;   
};

//Pixel Shader
float4 ps_mapa(PS_INPUT input) : COLOR0
{      
	return tex2D(diffuseMap, input.Texcoord);
	
}

//Pixel Shader
float4 sepia = float4(0.64, 0.55, 0.4, 1);
float4 ps_mapa_viejo(PS_INPUT input) : COLOR0
{    
	float4 fvBaseColor = tex2D(diffuseMap, input.Texcoord);
	float4 fvHeight = tex2D(heightMap, input.Texcoord);
	float luminance = (0.1*fvBaseColor.x + 0.95*fvBaseColor.y+0.2*fvBaseColor.z)+0.2; 
	float height = (0.1*fvHeight.x + 0.95*fvHeight.y+0.2*fvHeight.z)+0.2; 
	return (luminance + 0.6 - height)*sepia;	
	
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




