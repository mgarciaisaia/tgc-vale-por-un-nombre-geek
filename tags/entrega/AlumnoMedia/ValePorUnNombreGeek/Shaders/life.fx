float pointsY;
float pointsX;



struct PS_INPUT
{
	float4 Color: COLOR0;
	float2 Texcoord : TEXCOORD0;   
};

float4 ps_colorHorizontalLife(PS_INPUT input) : COLOR0
{      
	float4 color;
	if( input.Texcoord[0]>pointsX || pointsX == 0)
		color = 0;
	else color = input.Color;

	return color;

	
}

float4 ps_colorVerticalLife(PS_INPUT input) : COLOR0
{      
	float4 color;
	if( input.Texcoord[1]< pointsY || pointsY == 1)
		color = 0;
	else color = input.Color;

	return color;

	
}

 technique COLOR_VERTICAL
{
   pass Pass_0
   {
	  PixelShader = compile ps_2_0 ps_colorVerticalLife();
   }
}

 technique COLOR_HORIZONTAL
{
   pass Pass_0
   {
	  PixelShader = compile ps_2_0 ps_colorHorizontalLife();
   }
}
