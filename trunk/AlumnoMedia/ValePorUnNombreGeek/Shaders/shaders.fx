/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/


//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))


/**************************************************************************************/
/* SKINNING (Posicionar el mesh segun el skeleton)*/  
/**************************************************************************************/

//Matrix Pallette para skinning
static const int MAX_MATRICES = 26;
float4x3 bonesMatWorldArray[MAX_MATRICES];


struct SKINNING_INPUT
{
	float4 Position : POSITION;
	float3 Normal :   NORMAL;
	float3 Tangent : TANGENT;
	float3 Binormal : BINORMAL;
	float4 BlendWeights : BLENDWEIGHT;
    float4 BlendIndices : BLENDINDICES;

};


struct SKINNING_OUTPUT
{
	float4 Position : POSITION;
	float3 WorldNormal : TEXCOORD;
    float3 WorldTangent	: TEXCOORD;
    float3 WorldBinormal : TEXCOORD;
	
};


SKINNING_OUTPUT skinning(SKINNING_INPUT input){

	SKINNING_OUTPUT output;
	
	//Pasar indices de float4 a array de int
	int BlendIndicesArray[4] = (int[4])input.BlendIndices;
	
	//Skinning de posicion
	float3 skinPosition = mul(input.Position, bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;;
	skinPosition += mul(input.Position, bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinPosition += mul(input.Position, bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinPosition += mul(input.Position, bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w;
	output.Position = float4(skinPosition.xyz, 1.0);

	
	//Skinning de normal
	float3 skinNormal = mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;
	skinNormal += mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinNormal += mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinNormal += mul(input.Normal, (float3x3)bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w; 
	output.WorldNormal = normalize(skinNormal);
	
	//Skinning de tangent
	float3 skinTangent = mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;
	skinTangent += mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinTangent += mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinTangent += mul(input.Tangent, (float3x3)bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w;
	output.WorldTangent = normalize(skinTangent);
	
	//Skinning de binormal
	float3 skinBinormal = mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[0]]) * input.BlendWeights.x;
	skinBinormal += mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[1]]) * input.BlendWeights.y;
	skinBinormal += mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[2]]) * input.BlendWeights.z;
	skinBinormal += mul(input.Binormal, (float3x3)bonesMatWorldArray[BlendIndicesArray[3]]) * input.BlendWeights.w;
	output.WorldBinormal = normalize(skinBinormal);


	  
	return output;
}

SKINNING_INPUT fillSkinningInput(float4 Position, float3 Normal, float3 Tangent, float3 Binormal, float4 BlendWeights, float4 BlendIndices)
{
	SKINNING_INPUT input;
	input.Position = Position;
	input.Normal = Normal;
	input.Tangent = Tangent;
	input.Binormal = Binormal;
	input.BlendWeights = BlendWeights;
	input.BlendIndices = BlendIndices;

	return input;
}








/**************************************************************************************/
/**************************************************************************************/
/* SHADERS DIFFUSE MAP */
/**************************************************************************************/
/**************************************************************************************/

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


struct VS_INPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0; 
	float2 Texcoord : TEXCOORD0;

};


struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0; 
	float2 Texcoord : TEXCOORD0;
};


//Vertex Shader comun
VS_OUTPUT vs_DiffuseMap(VS_INPUT input)
{
	VS_OUTPUT output;

	//Enviar color directamente
	output.Color = input.Color;

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;
	
	//Proyectar posicion 
	output.Position = mul(input.Position, matWorldViewProj);

	
	return output;
}


//Para TgcSkeletalMesh es diferente

struct VS_SKELETAL_INPUT
{
	float4 Position : POSITION0;
	float3 Normal :   NORMAL0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
	float4 BlendWeights : BLENDWEIGHT;
    float4 BlendIndices : BLENDINDICES;
	float4 Color : COLOR0; 
	float2 Texcoord : TEXCOORD0;

};


struct VS_SKELETAL_OUTPUT
{
	float4 Position : POSITION0;
	float3 WorldNormal : TEXCOORD1;
    float3 WorldTangent	: TEXCOORD2;
    float3 WorldBinormal : TEXCOORD3;
	float4 Color : COLOR0; 
	float2 Texcoord : TEXCOORD0;
};

//Vertex Shader (Es el que usa TgcSkeletalMesh normalmente)
VS_SKELETAL_OUTPUT vs_Skeletal_DiffuseMap(VS_SKELETAL_INPUT input)
{
	VS_SKELETAL_OUTPUT output;

	SKINNING_OUTPUT sOut = skinning(
		fillSkinningInput(
							input.Position, 
							input.Normal, 
							input.Tangent, 
							input.Binormal,
							input.BlendWeights,
							input.BlendIndices
						)
					);


	output.WorldNormal = sOut.WorldNormal;
    output.WorldTangent	= sOut.WorldTangent;
    output.WorldBinormal = sOut.WorldBinormal;

	//Enviar color directamente
	output.Color = input.Color;

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;
		
	//Proyectar posicion 
	output.Position = mul(sOut.Position, matWorldViewProj);

	
	return output;

}




//Input del Pixel Shader
struct PS_DIFFUSE_MAP
{
	float4 Color : COLOR0; 
	float2 Texcoord : TEXCOORD0;
};

//Pixel Shader
float4 ps_DiffuseMap(PS_DIFFUSE_MAP input) : COLOR0
{      
	//Modular color de la textura por color del mesh
	return tex2D(diffuseMap, input.Texcoord) * input.Color;
}


//Cambia de color el mesh al color de seleccion
float4 selectionColor;

float4 ps_DiffuseMap_Selected( float2 Texcoord: TEXCOORD0) : COLOR0
{      
	return tex2D(diffuseMap, Texcoord)*0.5 + selectionColor*0.5  + float4(0.2,0.2,0.2,0);
}

float dayFactor; //Valor entre 0 y 1
float4 someKindOfBlue = float4(0.5,0.5,0.7,0);

float4 ps_Night( float2 Texcoord: TEXCOORD0, float4 Color:COLOR0) : COLOR0
{      
	
	float4 fvBaseColor = tex2D( diffuseMap, Texcoord );

	float luminance = (0.1*fvBaseColor.x + 0.95*fvBaseColor.y+0.2*fvBaseColor.z); //Paso a escala de grises
	float4 nightColor = luminance*someKindOfBlue; //Lo cambio al color deseado
	nightColor[3] = fvBaseColor[3]; //Mantengo el alpha

	return fvBaseColor*dayFactor + nightColor*(1-dayFactor); //Combino los colores
}





/**************************************************************************************/
/* Techniques DIFFUSE MAP */
/**************************************************************************************/

technique DIFFUSE_MAP
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_DiffuseMap();
	  PixelShader = compile ps_2_0 ps_DiffuseMap();
   }
}

technique SKELETAL_DIFFUSE_MAP
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_Skeletal_DiffuseMap();
	  PixelShader = compile ps_2_0 ps_DiffuseMap();
   }
}


technique SKELETAL_DIFFUSE_MAP_SELECTED
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_Skeletal_DiffuseMap();
	  PixelShader = compile ps_2_0 ps_DiffuseMap_Selected();
   }

}


technique NIGHT
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_DiffuseMap();
	  PixelShader = compile ps_2_0 ps_Night();
   }
}

technique SKELETAL_NIGHT
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_Skeletal_DiffuseMap();
	  PixelShader = compile ps_2_0 ps_Night();
   }

}

technique SKELETAL_NIGHT_SELECTED
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_Skeletal_DiffuseMap();
	   PixelShader = compile ps_2_0 ps_DiffuseMap_Selected();
   }

}





						/*


									Psyduck
         
											// //  //
								 __    ____||_//  //
							   _/__--~~        ~~~-_
							  /  /___        ___    \
							 /  /(  +)      ( + )    |
							/  |  ~~~    __  ~~~   _/\/|
						   |    \  ___.-~  ~-.___  \   / 
							\    \(     ` '      ~~)|   \
							 \     )              / |    \
							  \/   /              \ |    |
							  /   |               | |    |
							 |    /               |  \__/
							 |    \_            _/      |    ___
							 \      ~----...---~       /_.-~~ _/
							  \_                      |    _-~ 
								\                    /  _-~ 
								 ~-.__             _/--~
								_.-~  ~~~-----~~~~~
							   ~-.-. _-~     /_ ._ \   Amw
									~       ~  ~  ~-
						*/

/**************************************************************************************/
/**************************************************************************************/
/*                      SHADERS PARA CREAR EL SHADOW MAP
/**************************************************************************************/
/**************************************************************************************/



#define SMAP_SIZE 1024
#define EPSILON 0.05f


float4x4 g_mViewLightProj;
float4x4 g_mProjLight;
float3   g_vLightPos;  // posicion de la luz (en World Space) = pto que representa patch emisor Bj 
float3   g_vLightDir;  // Direcion de la luz (en World Space) = normal al patch Bj



texture  g_txShadow;	// textura para el shadow map
sampler2D g_samShadow =
sampler_state
{
    Texture = <g_txShadow>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};





void VertShadow( float4 Pos : POSITION0,
                 float3 Normal : NORMAL0,
                 out float4 oPos : POSITION0,
                 out float2 Depth : TEXCOORD0 )
{
	// transformacion estandard 
    oPos = mul( Pos, matWorld);					// uso el del mesh
    oPos = mul( oPos, g_mViewLightProj );		// pero visto desde la pos. de la luz
    
    // devuelvo: profundidad = z/w 
    Depth.xy = oPos.zw;
}



struct VS_SKELETAL_INPUT_SHADOW_MAP
{
	float4 Position : POSITION0;
	float3 Normal :   NORMAL0;
	float3 Tangent : TANGENT0;
	float3 Binormal : BINORMAL0;
	float4 BlendWeights : BLENDWEIGHT;
    float4 BlendIndices : BLENDINDICES;
	float4 Color : COLOR0; 
	float2 Texcoord : TEXCOORD0;

};


struct VS_SKELETAL_OUTPUT_SHADOW_MAP
{
	float4 Position : POSITION0;
	float3 WorldNormal : TEXCOORD1;
    float3 WorldTangent	: TEXCOORD2;
    float3 WorldBinormal : TEXCOORD3;
	float4 Color : COLOR0; 
	float2 Texcoord : TEXCOORD0;
};

VS_SKELETAL_OUTPUT_SHADOW_MAP VertShadow_Skeletal(VS_SKELETAL_INPUT_SHADOW_MAP input)
{
	VS_SKELETAL_OUTPUT_SHADOW_MAP output;

	SKINNING_OUTPUT sOut = skinning(
		fillSkinningInput(
							input.Position, 
							input.Normal, 
							input.Tangent, 
							input.Binormal,
							input.BlendWeights,
							input.BlendIndices
						)
					);


	output.WorldNormal = sOut.WorldNormal;
    output.WorldTangent	= sOut.WorldTangent;
    output.WorldBinormal = sOut.WorldBinormal;

	//Enviar color directamente
	output.Color = input.Color;

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;
		
	//Proyectar posicion 
	output.Position = mul(sOut.Position, matWorldViewProj);

	
	return output;

}

void PixShadow( float2 Depth : TEXCOORD0,out float4 Color : COLOR0 )
{
    Color = Depth.x/Depth.y;

}



/**************************************************************************************/
/* Techniques SHADOW MAP */
/**************************************************************************************/


technique SHADOW_MAP
{
    pass p0
    {
        VertexShader = compile vs_3_0 VertShadow();
        PixelShader = compile ps_3_0 PixShadow();
    }
}


technique SKELETAL_SHADOW_MAP
{
    pass p0
    { 
        VertexShader = compile vs_3_0 VertShadow_Skeletal();
		PixelShader = compile ps_3_0 PixShadow();
    }
}

technique SKELETAL_SHADOW_MAP_SELECTED
{
    pass p0
    { 
        VertexShader = compile vs_3_0 VertShadow_Skeletal();
		PixelShader = compile ps_3_0 PixShadow();
    }
}



/**************************************************************************************/
/**************************************************************************************/
/*              SHADERS PARA RENDERIZAR CON LA SOMBRAS DEL SHADOW MAP
/**************************************************************************************/
/**************************************************************************************/

void VertScene( float4 iPos : POSITION,
                float2 iTex : TEXCOORD0,
                float3 iNormal : NORMAL,
				out float4 oPos : POSITION,                
                out float2 Tex : TEXCOORD0,
				out float4 vPos : TEXCOORD1,
                out float3 vNormal : TEXCOORD2,
                out float4 vPosLight : TEXCOORD3 
                )
{
    // transformo al screen space
    oPos = mul( iPos, matWorldViewProj );
        
	// propago coordenadas de textura 
    Tex = iTex;
    
	// propago la normal
    vNormal = mul( iNormal, (float3x3)matWorldView );
    
    // propago la posicion del vertice en World space
    vPos = mul( iPos, matWorld);
    // propago la posicion del vertice en el espacio de proyeccion de la luz
    vPosLight = mul( vPos, g_mViewLightProj );
	
}



float4 PixScene(	float2 Tex : TEXCOORD0,
					float4 vPos : TEXCOORD1,
					float3 vNormal : TEXCOORD2,
					float4 vPosLight : TEXCOORD3
					):COLOR
{
    float3 vLight = normalize( float3( vPos - g_vLightPos ) );
	float cono = dot( vLight, g_vLightDir);
	float4 K = 0.0;
	if( cono > 0.7)
    {
    
    	// coordenada de textura CT
        float2 CT = 0.5 * vPosLight.xy / vPosLight.w + float2( 0.5, 0.5 );
        CT.y = 1.0f - CT.y;

		// sin ningun aa. conviene con smap size >= 512 
		float I = (tex2D( g_samShadow, CT) + EPSILON < vPosLight.z / vPosLight.w)? 0.0f: 1.0f;  
		
				
	    if( cono < 0.8)
			I*= 1-(0.8-cono)*10;
		
		K = I;
    }     
		
	float4 color_base = tex2D( diffuseMap, Tex);
	color_base.rgb *= 0.5 + 0.5*K;
	return color_base;	
}	





/**************************************************************************************/
/* Techniques SHADOWS */
/**************************************************************************************/



technique SHADOWS
{
    pass p0
    {
        VertexShader = compile vs_3_0 VertScene();
        PixelShader = compile ps_3_0 PixScene();
    }
}



technique SKELETAL_SHADOWS
{
    pass p0
    {
        VertexShader = compile vs_3_0 vs_Skeletal_DiffuseMap();
		PixelShader = compile ps_3_0 PixScene();
    }
}

technique SKELETAL_SHADOWS_SELECTED
{
    pass p0
    {
        VertexShader = compile vs_3_0 vs_Skeletal_DiffuseMap();
		PixelShader = compile ps_3_0 ps_DiffuseMap_Selected();
    }
}


