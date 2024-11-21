texture MyTexture;

sampler mySampler = sampler_state {
	Texture = <MyTexture> ;
};

struct VertexPositionTexture {
    float4 Position : POSITION;
    float2 TextureCoordinate : TEXCOORD;
};

struct VertexPositionColor {
    float4 Position : POSITION;
    float4 Color : COLOR;
};

/* VertexPositionTexture MyVertexShader(VertexPositionTexture input)
{
	VertexPositionTexture output;

	output.Position = input.Position;
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}
*/

VertexPositionColor MyVertexShader(VertexPositionColor input)
{
	VertexPositionColor output;
	output.Position = input.Position;
	output.Color = input.Color;

	return output;
}

float4 MyPixelShader(VertexPositionColor input): COLOR
{ 
	/*if (input.Color.r %0.1 < 0.05f) return float4(1, 1, 1, 1);
	else return input.Color;*/
	VertexPositionColor output;
	output.Color = input.Color;
	output.Color.a = 0.5f;
	return output.Color;
}

/*
float4 MyPixelShader(VertexPositionTexture input): COLOR
{
	return tex2D(mySampler, input.TextureCoordinate);
}
*/

technique MyTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 MyVertexShader();
		PixelShader = compile ps_4_0 MyPixelShader();
	}
}