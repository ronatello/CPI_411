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

// Skybox
texture environmentMap;

// Reflection-Refraction
float etaRatio;
float reflectivity;

// Mip-Map
int mipmap;

// Texture Scales and Heights
float ScaleU;
float ScaleV;
float BumpHeight;

// Normalization Settings
int NormalizeTangentFrame;
int NormalizeNormalMap;

sampler tsampler1 = sampler_state
{
    texture = <normalMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler tsampler1NoMipMap = sampler_state
{
    texture = <normalMap>;
    magfilter = NONE;
    minfilter = NONE;
    mipfilter = NONE;
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


struct ShaderInput
{
    float4 Position : POSITION;
    float4 Normal : NORMAL;
    float2 TextureCoordinate : TEXCOORD;
    float4 Tangent : TANGENT0;
    float4 Binormal : BINORMAL0;
};

struct ShaderOutput
{
    float4 Position : POSITION;
	//float4 Color: COLOR;
    float3 Normal : TEXCOORD0;
    float2 TextureCoordinate : TEXCOORD1;
    float3 WorldPosition : TEXCOORD2;
    float3 Tangent : TEXCOORD3;
    float3 Binormal : TEXCOORD4;
};

ShaderOutput VertexShaderFunction(ShaderInput input)
{
    ShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    float4 screenPosition = mul(viewPosition, Projection);
    output.Position = screenPosition;
	
    output.WorldPosition = worldPosition.xyz;
    output.TextureCoordinate = float2(input.TextureCoordinate.x * ScaleU, input.TextureCoordinate.y * ScaleV);
    
    output.Normal = mul(input.Normal, WorldInverseTranspose).xyz;
    output.Binormal = mul(input.Binormal, WorldInverseTranspose).xyz;
    output.Tangent = mul(input.Tangent, WorldInverseTranspose).xyz;

    return output;
};

float4 NormalMapPS(ShaderOutput input) : COLOR
{
    float3 normalTex = mipmap * tex2D(tsampler1, input.TextureCoordinate) + (1 - mipmap) * tex2D(tsampler1NoMipMap, input.TextureCoordinate);
    normalTex.x *= (1 + 0.2 * (BumpHeight - 5));
    normalTex.y *= (1 + 0.2 * (BumpHeight - 5));
    normalTex.z *= (1 + 0.2 * (5 - BumpHeight));
    
    return float4(normalTex, 1);
}

float4 NormalWorldMapPS(ShaderOutput input) : COLOR
{
    //float3x3 rotation = float3x3(input.Tangent, input.Binormal, input.Normal); //Use rotation matrices? Check Tangent to World slide
    
    /*
    float3x3 TangentToWorld;
    TangentToWorld[0] = normalize(mul(float4(input.Tangent, 1), World)).xyz;
    TangentToWorld[1] = normalize(mul(float4(input.Binormal, 1), World)).xyz;
    TangentToWorld[2] = normalize(mul(float4(input.Normal, 1), World)).xyz;
    */
    float3 TBN_N;
    float3 TBN_B;
    float3 TBN_T;

    float3 normalTex = ((mipmap * tex2D(tsampler1, input.TextureCoordinate) + (1 - mipmap) * tex2D(tsampler1NoMipMap, input.TextureCoordinate)).xyz - float3(0.5, 0.5, 0.5)) * 2.0;
    normalTex.x *= (1 + 0.2 * (BumpHeight - 5));
    normalTex.y *= (1 + 0.2 * (BumpHeight - 5));
    normalTex.z *= (1 + 0.2 * (5 - BumpHeight));

    TBN_N = normalize(input.Normal);
    TBN_B = normalize(input.Binormal);
    TBN_T = normalize(input.Tangent);
    
    //float3 bump = normalize(input.Normal + normalTex.x * input.Tangent + normalTex.y * input.Binormal).xyz;
    //float3 bump = normalize(mul(normalTex, TangentToWorld));
    float3 bump = normalize(normalTex.x * TBN_T + normalTex.y * TBN_B + normalTex.z * TBN_N).xyz;
    //bump = (0.5 * bump) + float3(0.5, 0.5, 0.5);
    
    return float4(bump, 1);
}

float4 BumpLightPS(ShaderOutput input) : COLOR
{
    /*
    float3x3 TangentToWorld;
    TangentToWorld[0] = normalize(mul(float4(input.Tangent, 1), World)).xyz;
    TangentToWorld[1] = normalize(mul(float4(input.Binormal, 1), World)).xyz;
    TangentToWorld[2] = normalize(mul(float4(input.Normal, 1), World)).xyz;
    */
    float3 TBN_N;
    float3 TBN_B;
    float3 TBN_T;
    
    float3 normalTex = ((mipmap * tex2D(tsampler1, input.TextureCoordinate) + (1 - mipmap) * tex2D(tsampler1NoMipMap, input.TextureCoordinate)).xyz - float3(0.5, 0.5, 0.5)) * 2.0;
    normalTex.x *= (1 + 0.2 * (BumpHeight - 5));
    normalTex.y *= (1 + 0.2 * (BumpHeight - 5));
    normalTex.z *= (1 + 0.2 * (5 - BumpHeight));
    
    if (NormalizeTangentFrame == 1)
    {
        TBN_N = normalize(input.Normal);
        TBN_B = normalize(input.Binormal);
        TBN_T = normalize(input.Tangent);
    }
    else
    {
        TBN_N = input.Normal;
        TBN_B = input.Binormal;
        TBN_T = input.Tangent;
    }
    
    //float3 bump = normalize(input.Normal + normalTex.x * input.Tangent + normalTex.y * input.Binormal).xyz;
    //float3 bump = normalize(mul(normalTex, TangentToWorld));
    float3 bump = (normalTex.x * TBN_T + normalTex.y * TBN_B + normalTex.z * TBN_N).xyz;
    
    float3 nDiffuse;
    float3 nSpecular;
    
    if (NormalizeNormalMap == 0)
    {
        nDiffuse = bump;
        nSpecular = bump;
    }
    
    else if (NormalizeNormalMap == 1)
    {
        nDiffuse = normalize(bump);
        nSpecular = normalize(bump);
    }
    
    else
    {
        nDiffuse = bump;
        nSpecular = normalize(bump);
    }
    
    float3 L = normalize(DiffuseLightDirection);
    float3 R = reflect(-L, normalize(bump));
    float3 V = normalize(CameraPosition - input.WorldPosition);
    float3 H = normalize(V + L);
	
    float4 ambient = AmbientColor * AmbientIntensity;
    float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(nDiffuse, L));
    float4 specular = pow(max(0, dot(H, nSpecular)), Shininess) * SpecularColor * SpecularIntensity;
    
    return saturate(ambient + diffuse + specular);
	
    //return tex2D(tsampler1, input.TextureCoordinate);
}

float4 BumpReflectPS(ShaderOutput input) : COLOR
{
    /*
    float3x3 TangentToWorld;
    TangentToWorld[0] = normalize(mul(float4(input.Tangent, 1), World)).xyz;
    TangentToWorld[1] = normalize(mul(float4(input.Binormal, 1), World)).xyz;
    TangentToWorld[2] = normalize(mul(float4(input.Normal, 1), World)).xyz;
    */
    float3 TBN_N;
    float3 TBN_B;
    float3 TBN_T;
	
    float3 normalTex = ((mipmap * tex2D(tsampler1, input.TextureCoordinate) + (1 - mipmap) * tex2D(tsampler1NoMipMap, input.TextureCoordinate)).xyz - float3(0.5, 0.5, 0.5)) * 2.0;
    normalTex.x *= (1 + 0.2 * (BumpHeight - 5));
    normalTex.y *= (1 + 0.2 * (BumpHeight - 5));
    normalTex.z *= (1 + 0.2 * (5 - BumpHeight));
    
    TBN_N = normalize(input.Normal);
    TBN_B = normalize(input.Binormal);
    TBN_T = normalize(input.Tangent);
    
    //float3 bump = normalize(input.Normal + normalTex.x * input.Tangent + normalTex.y * input.Binormal).xyz;
    //float3 bump = normalize(mul(normalTex, TangentToWorld));
    float3 bump = normalize(normalTex.x * TBN_T + normalTex.y * TBN_B + normalTex.z * TBN_N).xyz;
    
    float3 I = input.WorldPosition.xyz - CameraPosition;
    float3 R = reflect(I, bump);
    
    float4 reflectedColor = texCUBE(SkyBoxSampler, R);
    
	
    float3 L = normalize(DiffuseLightDirection);
    //float3 R = reflect(-L, normalize(bump));
    float3 V = normalize(CameraPosition - input.WorldPosition);
    float3 H = normalize(V + L);
	
    float4 ambient = AmbientColor * AmbientIntensity;
    float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(bump, L));
    float4 specular = pow(max(0, dot(H, bump)), Shininess) * SpecularColor * SpecularIntensity;
    
    float4 baseColor = saturate(ambient + diffuse + specular);
    
    return lerp(baseColor, reflectedColor, reflectivity);
}

float4 BumpRefractPS(ShaderOutput input) : COLOR
{
    /*
    float3x3 TangentToWorld;
    TangentToWorld[0] = normalize(mul(float4(input.Tangent, 1), World)).xyz;
    TangentToWorld[1] = normalize(mul(float4(input.Binormal, 1), World)).xyz;
    TangentToWorld[2] = normalize(mul(float4(input.Normal, 1), World)).xyz;
    */
    float3 TBN_N;
    float3 TBN_B;
    float3 TBN_T;
	
    float3 normalTex = ((mipmap * tex2D(tsampler1, input.TextureCoordinate) + (1 - mipmap) * tex2D(tsampler1NoMipMap, input.TextureCoordinate)).xyz - float3(0.5, 0.5, 0.5)) * 2.0;
    normalTex.x *= (1 + 0.2 * (BumpHeight - 5));
    normalTex.y *= (1 + 0.2 * (BumpHeight - 5));
    normalTex.z *= (1 + 0.2 * (5 - BumpHeight));
    
    TBN_N = normalize(input.Normal);
    TBN_B = normalize(input.Binormal);
    TBN_T = normalize(input.Tangent);
    
    //float3 bump = normalize(input.Normal + normalTex.x * input.Tangent + normalTex.y * input.Binormal).xyz;
    //float3 bump = normalize(mul(normalTex, TangentToWorld));
    float3 bump = normalize(normalTex.x * TBN_T + normalTex.y * TBN_B + normalTex.z * TBN_N).xyz;
    
    float3 I = input.WorldPosition.xyz - CameraPosition;
    float3 R = refract(I, bump, etaRatio);
    
    float4 refractedColor = texCUBE(SkyBoxSampler, R);
    
	
    float3 L = normalize(DiffuseLightDirection);
    //float3 R = reflect(-L, normalize(bump));
    float3 V = normalize(CameraPosition - input.WorldPosition);
    float3 H = normalize(V + L);
	
    float4 ambient = AmbientColor * AmbientIntensity;
    float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(bump, L));
    float4 specular = pow(max(0, dot(H, bump)), Shininess) * SpecularColor * SpecularIntensity;
    
    float4 baseColor = saturate(ambient + diffuse + specular);
    
    return lerp(baseColor, refractedColor, reflectivity);
}

technique NormalMap
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 NormalMapPS();
    }
};

technique NormalWorldMap
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 NormalWorldMapPS();
    }
};

technique BumpLight
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 BumpLightPS();
    }
};

technique BumpReflect
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 BumpReflectPS();
    }
};

technique BumpRefract
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 BumpRefractPS();
    }
};