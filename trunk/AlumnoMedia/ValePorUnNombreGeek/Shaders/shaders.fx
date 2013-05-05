/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/


//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))
float4 selectionColor;

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

//Matrix Pallette para skinning
static const int MAX_MATRICES = 26;
float4x3 bonesMatWorldArray[MAX_MATRICES];






//Input del Vertex Shader
struct SKINNING_INPUT
{
	float4 Position : POSITION;
	float3 Normal :   NORMAL;
	float3 Tangent : TANGENT;
	float3 Binormal : BINORMAL;
	float4 BlendWeights : BLENDWEIGHT;
    float4 BlendIndices : BLENDINDICES;

};

//Output del Vertex Shader
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

SKINNING_INPUT fillSkinningInput(
	float4 Position : POSITION, 
	float3 Normal :   NORMAL, 
	float3 Tangent : TANGENT, 
	float3 Binormal : BINORMAL,
	float4 BlendWeights : BLENDWEIGHT,
    float4 BlendIndices : BLENDINDICES)
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


//Input del Vertex Shader
struct VS_INPUT
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

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position : POSITION0;
	float3 WorldNormal : TEXCOORD1;
    float3 WorldTangent	: TEXCOORD2;
    float3 WorldBinormal : TEXCOORD3;
	float4 Color : COLOR0; 
	float2 Texcoord : TEXCOORD0;
};

//Vertex Shader (Es el que usa TgcSkeletalMesh normalmente)
VS_OUTPUT vs_Skeletal_DiffuseMap(VS_INPUT input)
{
	VS_OUTPUT output;

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


	output.Position = sOut.Position;
	output.WorldNormal = sOut.WorldNormal;
    output.WorldTangent	= sOut.WorldTangent;
    output.WorldBinormal = sOut.WorldBinormal;

	//Enviar color directamente
	output.Color = input.Color;

	//Enviar Texcoord directamente
	output.Texcoord = input.Texcoord;
	
	
	//Proyectar posicion (teniendo en cuenta lo que se hizo por skinning)
	output.Position = mul(output.Position, matWorldViewProj);

	
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



/*
* Technique DIFFUSE_MAP
*/
technique SKELETAL_DIFFUSE_MAP
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_Skeletal_DiffuseMap();
	  PixelShader = compile ps_2_0 ps_DiffuseMap();
   }
}

float4 ps_Selected( float2 Texcoord: TEXCOORD0) : COLOR0
{      
	return tex2D(diffuseMap, Texcoord)*0.5 + selectionColor*0.5  + float4(0.2,0.2,0.2,0);
}


technique SKELETAL_DIFFUSE_MAP_SELECTED
{
   pass Pass_0
   {
	  VertexShader = compile vs_2_0 vs_Skeletal_DiffuseMap();
	  PixelShader = compile ps_2_0 ps_Selected();
   }

}



/*
        SOMBRAS
*/



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


technique SKELETAL_SHADOWS
{
    pass p0
    {
        VertexShader = compile vs_2_0 vs_Skeletal_DiffuseMap();
		PixelShader = compile ps_2_0 ps_DiffuseMap();
    }
}

technique SKELETAL_SHADOW_MAP
{
    pass p0
    {
        VertexShader = compile vs_2_0 vs_Skeletal_DiffuseMap();
		PixelShader = compile ps_2_0 ps_DiffuseMap();
    }
}