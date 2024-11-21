// Image Processing Template

float4x4 MatrixTransform;
texture2D modelTexture;
texture2D depthTexture;
float imageWidth;
float imageHeight;

texture2D filterTexture;

float3x3 RGB2YCbCr =
{
    { 0.2989f, 0.5866f, 0.1145f },
    { -0.1687f, -0.3312f, 0.5000f },
    { 0.5000f, -0.4183f, -0.0816f }
};

float DecodeFloatRGB(float4 color)
{
    const float3 byte_to_float = float3(1.0, 1.0 / 256, 1.0 / (256 * 256));
    return dot(color.xyz, byte_to_float);
}

sampler TextureSampler : register(s0) = sampler_state
//sampler TextureSampler = sampler_state
{
    Texture = <modelTexture>;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
};

sampler depthTextureSampler : register(s1) = sampler_state
//sampler depthTextureSampler = sampler_state
{
    Texture = <depthTexture>;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
};

/*
sampler FilterSampler : register(s1) = sampler_state
{
    Texture = <filterTexture>;
    ADDRESSU = WRAP;
	//ADDRESSV = CLAMP;
};
*/

struct VS_OUTPUT
{
    float4 Pos : POSITION;
    float2 UV0 : TEXCOORD0;
    float4 UV1 : TEXCOORD1;
};

VS_OUTPUT vtxSh(float4 inPos : POSITION, float2 inTex : TEXCOORD0)
{
    VS_OUTPUT Out;
    Out.Pos = mul(inPos, MatrixTransform);
    Out.UV0 = inTex;
    Out.UV1 = float4(2 / imageWidth, 0, 0, 2 / imageHeight);
	
    return Out;
};

float4 pxlSh(VS_OUTPUT In) : COLOR
{
    float4 tex = tex2D(TextureSampler, In.UV0);
    
    float4 texA = 2.0f * (tex2D(TextureSampler, In.UV0 - In.UV1.xy - In.UV1.zw) - 0.5f);
    float4 texC = 2.0f * (tex2D(TextureSampler, In.UV0 + In.UV1.xy - In.UV1.zw) - 0.5f);
    float4 texF = 2.0f * (tex2D(TextureSampler, In.UV0 - In.UV1.xy + In.UV1.zw) - 0.5f);
    float4 texH = 2.0f * (tex2D(TextureSampler, In.UV0 + In.UV1.xy + In.UV1.zw) - 0.5f);
    
    float4 color;
    
    color.rgb = 0.5 * (dot(texA.rgb, texH.rgb) + dot(texC.rgb, texF.rgb));
    color.a = 0.0f;
    
    
    /*
    float4 ztexA = tex2D(depthTextureSampler, In.UV0 - In.UV1.xy - In.UV1.zw);
    float4 ztexC = tex2D(depthTextureSampler, In.UV0 + In.UV1.xy - In.UV1.zw);
    float4 ztexF = tex2D(depthTextureSampler, In.UV0 - In.UV1.xy + In.UV1.zw);
    float4 ztexH = tex2D(depthTextureSampler, In.UV0 + In.UV1.xy + In.UV1.zw);
    */
    
    /*
    float ztexA = DecodeFloatRGB(tex2D(depthTextureSampler, In.UV0 - In.UV1.xy - In.UV1.zw));
    float ztexC = DecodeFloatRGB(tex2D(depthTextureSampler, In.UV0 + In.UV1.xy - In.UV1.zw));
    float ztexF = DecodeFloatRGB(tex2D(depthTextureSampler, In.UV0 - In.UV1.xy + In.UV1.zw));
    float ztexH = DecodeFloatRGB(tex2D(depthTextureSampler, In.UV0 + In.UV1.xy + In.UV1.zw));
    
    color.a = pow((1.0f - 0.5f * abs(ztexA - ztexH)), 2.0f) * pow((1.0f - 0.5f * abs(ztexC - ztexF)), 2.0f);
    
    if (color.r == 1.0 && color.g == 1.0 && color.b == 1.0)
    {
        color.a = 0.0f;
    }
    */
	
    return color;
    //return tex;
};


float4 zPxlSh(VS_OUTPUT In) : COLOR
{
    float texA = DecodeFloatRGB(tex2D(depthTextureSampler, In.UV0 - In.UV1.xy - In.UV1.zw));
    float texC = DecodeFloatRGB(tex2D(depthTextureSampler, In.UV0 + In.UV1.xy - In.UV1.zw));
    float texF = DecodeFloatRGB(tex2D(depthTextureSampler, In.UV0 - In.UV1.xy + In.UV1.zw));
    float texH = DecodeFloatRGB(tex2D(depthTextureSampler, In.UV0 + In.UV1.xy + In.UV1.zw));
    
    float4 color;
    color.rgb = 1.0f - pow((1.0f - 0.5f * abs(texA - texH)), 2.0f) * pow((1.0f - 0.5f * abs(texC - texF)), 2.0f);
    color.a = 1.0f;
    
	
    return color;
};

technique ZEdgeMap
{
    pass P0
    {
        VertexShader = compile vs_4_0 vtxSh();
        PixelShader = compile ps_4_0 zPxlSh();
    }
};

/*
technique NormalEdgeMap
{
    pass P0
    {
        VertexShader = compile vs_4_0 vtxSh();
        PixelShader = compile ps_4_0 normalPxlSh();
    }
};
*/

/*
technique EdgeDraw
{
    pass P0
    {
        VertexShader = compile vs_4_0 vtxSh();
        PixelShader = compile ps_4_0 pxlSh();
    }
};
*/