
float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
float4x4 LightViewMatrix;
float4x4 LightProjectionMatrix;

float3 CameraPosition;
float3 LightPosition;
float AmbientColor;

struct VertexShaderInput
{
	float4 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float3 WorldPosition : TEXCOORD2;
};

VertexShaderOutput VSFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	float4 worldPos = mul(input.Position, World);
	float4 viewPosition = mul(worldPos, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = worldPos.xyz;
	return output;
}

float4 ShadowPSFunction(VertexShaderOutput input) : COLOR0
{
	//float4 color;
	//return color;

	//return float4(AmbientColor, AmbientColor, AmbientColor, 1.0);


	// Step 1
	float4 projTexCoord = mul(mul(float4(input.WorldPosition, 1), LightViewMatrix), LightProjectionMatrix);
	// Step 2
	projTexCoord = projTexCoord / projTexCoord.w;
	// Step 3
	projTexCoord.xy = 0.5 * projTexCoord.xy + float2 (0.5, 0.5);
	// Step 4
	projTexCoord.y = 1.0 - projTexCoord.y;
	// Step 5
	float depth = 1.0 - projTexCoord.z;
	// Step 6
	float4 color = (depth > 0) ? depth : float4 (0, 0, 0, 1);

	return color;

}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VSFunction();
		PixelShader = compile ps_4_0 ShadowPSFunction();
	}
}
