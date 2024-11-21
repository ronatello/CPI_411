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

float4x4 InverseCamera; //Inverse Camera Matrix
texture2D Texture;


sampler ParticleSampler : register(s0) = sampler_state
{
    texture = <Texture>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU = clamp;
    AddressV = clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION;
    //float4 Normal : NORMAL;
    float2 TexCoord : TEXCOORD0;
    float4 ParticlePosition : POSITION1;
    float4 ParticleParameter : POSITION2; // x: Scale x/y: Color
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    float4 WorldPosition : TEXCOORD2;
    float4 Color : COLOR0;
};

VertexShaderOutput ParticleVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, InverseCamera);
    worldPosition.xyz = worldPosition.xyz * sqrt(input.ParticleParameter.x);
    worldPosition += input.ParticlePosition;
    
    float4 worldMatrixPosition = mul(worldPosition, World);

    output.Position = mul(mul(worldMatrixPosition, View), Projection);
    output.Normal = CameraPosition - worldMatrixPosition.xyz;
    //output.Normal = mul(float4(0, 1, 0, 1), InverseCamera);
    //output.Normal = float4(0, 0, 1, 1);
    output.WorldPosition = worldMatrixPosition;
    output.TexCoord = input.TexCoord;
    output.Color = 1 - input.ParticleParameter.x / input.ParticleParameter.y;

    return output;
}

float4 ParticlePhongShader(VertexShaderOutput input) : COLOR
{
    //float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
    float3 N = normalize(input.Normal);
    float3 L = normalize(DiffuseLightDirection - input.WorldPosition.xyz);
    float3 R = normalize(reflect(-L, N));
    float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
    float3 H = normalize(V + L);

	// Color Calculation
    float4 ambient = AmbientColor * AmbientIntensity;
    float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
    float4 specular = pow(max(0, dot(V, R)), Shininess) * SpecularColor * SpecularIntensity;
    float4 color = saturate(ambient + diffuse + specular);
    color.a = 1;
    
    color *= input.Color;

    return color;
}

float4 ParticlePixelShader(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(ParticleSampler, input.TexCoord);
    color *= input.Color;

    return color;
}

technique Particle
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 ParticleVertexShader();
        PixelShader = compile ps_4_0 ParticlePixelShader();
    }
};

technique Phong
{
    pass Pass0
    {
        VertexShader = compile vs_4_0 ParticleVertexShader();
        PixelShader = compile ps_4_0 ParticlePhongShader();
    }
};