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

// Camera and Light
float3 CameraPosition;
float Shininess;
float4 SpecularColor;
float SpecularIntensity;

// Bump Map
texture normalMap;


sampler tsampler1 = sampler_state
{
    texture = <normalMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct ShaderInput {
	float4 Position: POSITION;
	float4 Normal: NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
    float4 Tangent : TANGENT0;
    float4 Binormal : BINORMAL0;
};

struct ShaderOutput {
	float4 Position: POSITION;
	//float4 Color: COLOR;
	float2 TextureCoordinate : TEXCOORD0;
	float3 Normal: TEXCOORD1;
	float3 WorldPosition: TEXCOORD2;
    float3 Tangent : TEXCOORD3;
    float3 Binormal : TEXCOORD4;
};

ShaderOutput VertexShaderFunction(ShaderInput input) {
	ShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	float4 screenPosition = mul(viewPosition, Projection);
	output.Position = screenPosition;
	
	output.WorldPosition = worldPosition.xyz;
    output.TextureCoordinate = input.TextureCoordinate;	
	
    output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
    output.Binormal = normalize(mul(input.Binormal, WorldInverseTranspose).xyz);
    output.Tangent = normalize(mul(input.Tangent, WorldInverseTranspose).xyz);

	return output;
};

float4 PixelShaderFunction(ShaderOutput input) : COLOR {
	
    float3 normalTex = (tex2D(tsampler1, input.TextureCoordinate).xyz - float3(0.5, 0.5, 0.5)) * 2.0;
    float3 bump = normalize(input.Normal + normalTex.x * input.Tangent + normalTex.y * input.Binormal).xyz;
	
    float3 L = normalize(DiffuseLightDirection);
    float3 R = reflect(-L, normalize(bump));
	float3 V = normalize(CameraPosition - input.WorldPosition);
	float3 H = normalize(V + L);
	
    float4 ambient = AmbientColor * AmbientIntensity;
    float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(bump, L));
    float4 specular = pow(max(0, dot(V, R)), Shininess) * SpecularColor * SpecularIntensity;
    
	return saturate(ambient + diffuse + specular);
	
    //return tex2D(tsampler1, input.TextureCoordinate);
}

technique MyTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
};