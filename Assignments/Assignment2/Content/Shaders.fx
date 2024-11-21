// Matrix

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

// Color

//float4 AmbientColor;
//float AmbientIntensity;
//float3 DiffuseLightDirection;
//float4 DiffuseColor;
//float DiffuseIntensity;

// Camera and Light
float3 CameraPosition;
//float Shininess;
//float4 SpecularColor;
//float SpecularIntensity;

// Environment Map
texture decalMap;
texture environmentMap;

// Reflection-Refraction
float3 etaRatio;
float reflectivity;
float bias;
float power;
float scale;

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

struct VertexReflectionShaderInput
{
    float4 Position : POSITION;
    float4 Normal : NORMAL;
    float2 TextureCoordinate : TEXCOORD;
};

struct VertexReflectionShaderOutput
{
    float4 Position : POSITION;
    float4 Normal : NORMAL;
    float2 TextureCoordinate : TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
};

VertexReflectionShaderOutput ReflectVS(VertexReflectionShaderInput input)
{
    VertexReflectionShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    float4 screenPosition = mul(viewPosition, Projection);
    output.Position = screenPosition;
    output.Normal = input.Normal;
    output.TextureCoordinate = input.TextureCoordinate;
    output.WorldPosition = worldPosition;
	
    return output;
};

float4 ReflectPS(VertexReflectionShaderOutput input) : COLOR
{
    float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
    float3 I = input.WorldPosition.xyz - CameraPosition;
    float3 R = reflect(I, N);
    
    float4 reflectedColor = texCUBE(SkyBoxSampler, R);
    float4 decalColor = tex2D(tsampler1, input.TextureCoordinate);
    return lerp(decalColor, reflectedColor, reflectivity);
};

VertexReflectionShaderOutput RefractVS(VertexReflectionShaderInput input)
{
    VertexReflectionShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    float4 screenPosition = mul(viewPosition, Projection);
    output.Position = screenPosition;
    output.Normal = input.Normal;
    output.TextureCoordinate = input.TextureCoordinate;
    output.WorldPosition = worldPosition;
	
    return output;
};

float4 RefractPS(VertexReflectionShaderOutput input) : COLOR
{
    
    float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
    float3 I = input.WorldPosition.xyz - CameraPosition;
    float3 R = refract(I, N, etaRatio.x);
    
    float4 refractedColor = texCUBE(SkyBoxSampler, R);
    float4 decalColor = tex2D(tsampler1, input.TextureCoordinate);
    return lerp(decalColor, refractedColor, reflectivity);
};

VertexReflectionShaderOutput RefractDispersionVS(VertexReflectionShaderInput input)
{
    VertexReflectionShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    float4 screenPosition = mul(viewPosition, Projection);
    output.Position = screenPosition;
    output.Normal = input.Normal;
    output.TextureCoordinate = input.TextureCoordinate;
    output.WorldPosition = worldPosition;
	
    return output;
};

float4 RefractDispersionPS(VertexReflectionShaderOutput input) : COLOR
{
    float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
    float3 I = input.WorldPosition.xyz - CameraPosition;
    float3 R = refract(I, N, etaRatio.x);
    float3 G = refract(I, N, etaRatio.y);
    float3 B = refract(I, N, etaRatio.z);
    
    float4 refractedColor;
    refractedColor.r = texCUBE(SkyBoxSampler, R).r;
    refractedColor.g = texCUBE(SkyBoxSampler, G).g;
    refractedColor.b = texCUBE(SkyBoxSampler, B).b;
    refractedColor.a = 1;
    
    float4 decalColor = tex2D(tsampler1, input.TextureCoordinate);
    return lerp(decalColor, refractedColor, reflectivity);
};

VertexReflectionShaderOutput FresnelVS(VertexReflectionShaderInput input)
{
    VertexReflectionShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    float4 screenPosition = mul(viewPosition, Projection);
    output.Position = screenPosition;
    output.Normal = input.Normal;
    output.TextureCoordinate = input.TextureCoordinate;
    output.WorldPosition = worldPosition;
	
    return output;
};

float4 FresnelPS(VertexReflectionShaderOutput input) : COLOR
{
    
    float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
    float3 I = input.WorldPosition.xyz - CameraPosition;
    float3 reflected = reflect(I, N);
    float3 refracted = refract(I, N, etaRatio.x);
    float reflectionCoefficient = max(0, min(1, bias + (scale * pow(1.0f + dot(I, N), power))));
    //float reflectionCoefficient = max(0, min(1, bias + (scale * pow(1 + 0, power))));
    
    float4 reflectedColor = texCUBE(SkyBoxSampler, reflected);
    float4 refractedColor = texCUBE(SkyBoxSampler, refracted);
    float4 finalColor = lerp(refractedColor, reflectedColor, reflectionCoefficient);
    
    float4 decalColor = tex2D(tsampler1, input.TextureCoordinate);
    return lerp(decalColor, finalColor, reflectivity);
};

technique Reflect
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 ReflectVS();
        PixelShader = compile ps_4_0 ReflectPS();
    }
};

technique Refract
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 RefractVS();
        PixelShader = compile ps_4_0 RefractPS();
    }
};

technique RefractDispersion
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 RefractDispersionVS();
        PixelShader = compile ps_4_0 RefractDispersionPS();
    }
};

technique Fresnel
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 FresnelVS();
        PixelShader = compile ps_4_0 FresnelPS();
    }
};