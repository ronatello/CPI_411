// Image Processing Template

float4x4 MatrixTransform;
texture2D normalEdgeTexture;
texture2D depthEdgeTexture;
float imageWidth;
float imageHeight;

float DecodeFloatRGB(float4 color)
{
    const float3 byte_to_float = float3(1.0, 1.0 / 256, 1.0 / (256 * 256));
    return dot(color.xyz, byte_to_float);
}

sampler depthEdgeTextureSampler : register(s0) = sampler_state
{
    Texture = <depthEdgeTexture>;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
};

sampler normalEdgeTextureSampler : register(s1) = sampler_state
{
    Texture = <normalEdgeTexture>;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
};

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
    float4 ntex = tex2D(normalEdgeTextureSampler, In.UV0);
    float4 ztex = tex2D(depthEdgeTextureSampler, In.UV0);
    
    float4 color;
    
    color.rgb = 0.6 * (1.0 - ntex.rgb);
    //color.a = ztex.r;
    
    if (color.r <= 0.01f && color.g <= 0.01f && color.b <= 0.01f)
    {
        color.a = 0.0f;
    }
    else
    {
        color.a = DecodeFloatRGB(ztex);
    }

	
    return color;
};

float4 finalFragmentComposite(VS_OUTPUT In) : COLOR
{
    float4 bgtex = tex2D(normalEdgeTextureSampler, In.UV0);
    float4 fgtex = tex2D(depthEdgeTextureSampler, In.UV0);
    
    float4 color;
    
    //color.rgb = fgtex.rgb;
    
    //color.rgb = fgtex.rgb + float3(0.0f, 0.369f, 0.788f);
    //color.a = fgtex.a;
    
    //color.rgb = fgtex.rgb * float3(0.0f, 0.369f, 0.788f);
    //color.rgb = fgtex.rgb * float3(0.643, 0.808, 1);
    //color.a = 1.0f;
    
    color.rgb = fgtex.rgb;
    color.rgb = (1.0f - color.rgb) * bgtex.rgb;
    color.a = fgtex.a;
   
    
    //color.rgb = fgtex.rgb;
    /*
    if (color.r <= 0.1f && color.g <= 0.1f && color.b <= 0.1f)
    {
        color.rgb = bgtex;
    }
    /*
    else
    {
        color.rgb = color.rgb * float3(1.0f, 0.0f, 0.0f);
    }
    */
    //color.a = 1.0f;
    //color.rgb = (1.0f - fgtex.rgb) * bgtex.rgb;
    //color.rgb += fgtex.rgb;
    //color.rgb = fgtex.rgb + bgtex.rgb;
    
    return color;
};

technique ComposeEdgeMaps
{
    pass P0
    {
        VertexShader = compile vs_4_0 vtxSh();
        PixelShader = compile ps_4_0 pxlSh();
    }
};

technique DrawBG
{
    pass P0
    {
        VertexShader = compile vs_4_0 vtxSh();
        PixelShader = compile ps_4_0 finalFragmentComposite();
    }
};