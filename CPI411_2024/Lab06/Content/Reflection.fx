float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float3 CameraPosition;
texture decalMap;
texture environmentMap;

sampler tsampler1 = sampler_state
{
	texture = <decalMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

samplerCUBE SkyBoxSampler = sampler_state
{
	texture = <environmentMap>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = Mirror;
	AddressV = Mirror;
};

struct VertexShaderInput {
	float4 Position: POSITION;
	float4 Normal: NORMAL;
	float2 TextureCoordinate: TEXCOORD;
};

struct VertexShaderOutput {
	float4 Position: POSITION;
	float2 TextureCoordinate: TEXCOORD0;
	float3 R: TEXCOORD1;
};

VertexShaderOutput ReflectVS (VertexShaderInput input) {
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	float4 screenPosition = mul(viewPosition, Projection);
	output.Position = screenPosition;
	output.TextureCoordinate = input.TextureCoordinate;

	float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	float3 I =  worldPosition.xyz - CameraPosition;
	output.R = reflect(I, N);
	
	return output;
};

float4 ReflectPS (VertexShaderOutput input) : COLOR
{
	float4 reflectedColor = texCUBE(SkyBoxSampler, input.R);
	float4 decalColor = tex2D(tsampler1, input.TextureCoordinate);
	return lerp(decalColor, reflectedColor, 0.5);
};

technique Reflect
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 ReflectVS();
		PixelShader = compile ps_4_0 ReflectPS();
	}
};