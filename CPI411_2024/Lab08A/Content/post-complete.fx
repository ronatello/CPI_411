// Image Processing Template

float4x4 MatrixTransform;
texture2D modelTexture;
float imageWidth;
float imageHeight;

texture2D filterTexture;

float3x3 RGB2YCbCr = {
{ 0.2989f, 0.5866f, 0.1145f },
{-0.1687f, -0.3312f, 0.5000f },
{ 0.5000f, -0.4183f, -0.0816f } };

sampler TextureSampler:register(s0) = sampler_state {
	Texture = <modelTexture>;
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
};

sampler FilterSampler: register(s1) = sampler_state {
	Texture = <filterTexture>;
	ADDRESSU = WRAP;
	//ADDRESSV = CLAMP;
};

struct VS_OUTPUT {
	float4 Pos: POSITION;
	float2 UV0: TEXCOORD0;
	float4 UV1: TEXCOORD1;
};

VS_OUTPUT vtxSh(float4 inPos: POSITION, float2 inTex: TEXCOORD0) {
	VS_OUTPUT Out;
	Out.Pos = mul(inPos, MatrixTransform);
	Out.UV0 = inTex;
	Out.UV1 = float4 (2/imageWidth, 0, 0, 2/imageHeight);
	
	return Out;
};

float4 pxlSh(VS_OUTPUT In): COLOR {
	float4 tex = tex2D(TextureSampler, In.UV0);
	
	//Example 1 - Posterization
	//tex = ceil(tex * 8) / 8;
	//tex.a = 1;
	
	//Example 2 - Tone Curve
	//tex.r = tex2D(FilterSampler, float2(tex.r, 0));
	//tex.g = tex2D(FilterSampler, float2(tex.g, 0));
	//tex.b = tex2D(FilterSampler, float2(tex.b, 0));

	//Example 3 - Monochrome Conversion
	//tex.rgb = (mul(RGB2YCbCr, tex.rgb)).r;
	//tex.rgb = (tex.r + tex.b + tex.g)/3;

	//Example 4 - Blur
	float4 tex1 = tex2D(TextureSampler, In.UV0 + In.UV1.xy * 3);
	float4 tex2 = tex2D(TextureSampler, In.UV0 - In.UV1.xy * 3);
	float4 tex3 = tex2D(TextureSampler, In.UV0 + In.UV1.zw * 3);
	float4 tex4 = tex2D(TextureSampler, In.UV0 - In.UV1.zw * 3);
	// Blur
	tex = (tex + tex1 + tex2 + tex3 + tex4) / 5; 
	// Edge Detection
	//tex = tex *4 -(tex1 + tex2 + tex3 + tex4);
	//Sharpen
	//tex = tex *5 -(tex1 + tex2 + tex3 + tex4);


	return tex;
};

technique MyShader {
	pass P0 {
		VertexShader = compile vs_4_0 vtxSh();
		PixelShader = compile ps_4_0 pxlSh();
	}
};