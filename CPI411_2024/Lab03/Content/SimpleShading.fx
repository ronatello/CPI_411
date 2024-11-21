// Matrix

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

// Color

float4 AmbientColor;
float AmbientIntensity;
float3 DiffuseLightDirection;
float4 DiffuseColor;
float DiffuseIntensity;

struct VertexInput {
	float4 Position: POSITION;
	float4 Normal: NORMAL;
};

struct VertexOutput {
	float4 Position: POSITION;
	float4 Color: COLOR;
};

VertexOutput VertexShaderFunction(VertexInput input) {
	VertexOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	float4 screenPosition = mul(viewPosition, Projection);
	output.Position = screenPosition;

	// Per-vertex Lighting 90s
	float4 normal = mul(input.Normal, WorldInverseTranspose);
	float lightIntensity = max(0, dot(normalize(normal.xyz), normalize(DiffuseLightDirection))); // avoid negative colors
	// if (lightIntensity < 0) lightIntensity = 0; -- avoid negative colors
	output.Color = saturate(DiffuseColor * DiffuseIntensity * lightIntensity);

	return output;
};

float4 PixelShaderFunction(VertexOutput input) : COLOR
{
	return saturate(input.Color + AmbientColor * AmbientIntensity);
};

technique MyTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
};