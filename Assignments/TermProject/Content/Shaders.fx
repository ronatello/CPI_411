float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;

float normalOffset = 1.0f/2048;

texture2D DepthTexture;

sampler depthMap = sampler_state
{
    Texture = <DepthTexture>;
    MipFilter = NONE;
    MinFilter = POINT;
    MagFilter = POINT;
};


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Position2D : TEXCOORD0;
    float4 Normal : NORMAL;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Position2D : TEXCOORD0;
    float3 Normal : TEXCOORD1;
};

float4 EncodeFloatRGB(float f)
{
    float4 color;
    f *= 256;
    color.x = floor(f);
    f = (f - color.x) * 256;
    color.y = floor(f);
    color.z = f - color.y;
    color.xy *= 0.00390625; // *= 1.0/256
    color.a = 1;

    return color;
}

float DecodeFloatRGB(float4 color)
{
    const float3 byte_to_float = float3(1.0, 1.0 / 256, 1.0 / (256 * 256));
    return dot(color.xyz, byte_to_float);
}

VertexShaderOutput DepthMapVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(mul(mul(input.Position, World), View), Projection);
    output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
    output.Position2D = output.Position; //+float4((output.Normal * normalOffset), 0);

    return output;
}

float4 DepthMapPixelShader(VertexShaderOutput input) : COLOR0
{
    float4 projTexCoord = input.Position2D / input.Position2D.w;
    projTexCoord.xy = 0.5 * projTexCoord.xy + float2(0.5, 0.5);
    projTexCoord.y = 1.0 - projTexCoord.y; // invert Y direction (because UV map is opposite to y coordinate system)
    
    if (projTexCoord.x >= 0 && projTexCoord.x <= 1 && projTexCoord.y >= 0 && projTexCoord.y <= 1 && saturate(projTexCoord).x == projTexCoord.x && saturate(projTexCoord).y == projTexCoord.y)
    {
        
        float depth = projTexCoord.z;
        float4 color;
        color = (depth > 0) ? EncodeFloatRGB(depth) : EncodeFloatRGB(0);
        color.a = (depth > 0) ? 1 : 0; // culling

        return color;
        

        /*
        float depth = projTexCoord.z;
        float4 color;
        //color = (depth > 0) ? EncodeFloatRGB(depth) : EncodeFloatRGB(0);
        color.rgb = (normalize(input.Normal.xyz)) / 2.0f + 0.5f;
        color.a = (depth > 0) ? depth : 0; // culling
        
        return color;
        */
    }
    else
    {
        discard;
        
        float4 color;
        color = EncodeFloatRGB(0);
        color.a = 0; // culling

        return color;
        
        /*
        float4 color;
        color.rgb = (normalize(input.Normal.xyz)) / 2.0f + 0.5f;
        color.a = 0; // culling
        return color;
        */
    }   
}

float4 NormalPixelShader(VertexShaderOutput input) : COLOR0
{
    float4 projTexCoord = input.Position2D / input.Position2D.w;
    projTexCoord.xy = 0.5 * projTexCoord.xy + float2(0.5, 0.5);
    projTexCoord.y = 1.0 - projTexCoord.y; // invert Y direction (because UV map is opposite to y coordinate system)

    /*
    float4 color;
    color.rgb = (normalize(input.Normal.xyz)) / 2.0f + 0.5f;
    color.a = 1;

    return color;
    */

    float depth = projTexCoord.z;
    float4 color;
    //color = (depth > 0) ? EncodeFloatRGB(depth) : EncodeFloatRGB(0);
    color.rgb = (normalize(input.Normal.xyz)) / 2.0f + 0.5f;
    color.a = (depth > 0) ? 1 : 0; // culling

    return color;
}

VertexShaderOutput DepthPeelingVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(mul(mul(input.Position, World), View), Projection);
    output.Normal = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
    output.Position2D = output.Position;

    return output;
}



float4 DepthPeelingPixelShader(VertexShaderOutput input) : COLOR0
{
    float4 projTexCoord = input.Position2D / input.Position2D.w;
    projTexCoord.xy = 0.5 * projTexCoord.xy + float2(0.5, 0.5);
    projTexCoord.y = 1.0 - projTexCoord.y; // invert Y direction (because UV map is opposite to y coordinate system)

    /*
    if (projTexCoord.x >= 0 && projTexCoord.x <= 1 && projTexCoord.y >= 0 && projTexCoord.y <= 1 && saturate(projTexCoord).x == projTexCoord.x && saturate(projTexCoord).y == projTexCoord.y)
    {
        float depth = projTexCoord.z;
    
        float4 prevDepthLayer = tex2D(depthMap, projTexCoord.xy);
        //float prevDepth = DecodeFloatRGB(prevDepthLayer);
    
        if (depth <= prevDepthLayer.a + 1.0f / 2048.0f)
        {
            discard;
        
            float4 color;
            //color = EncodeFloatRGB(0);
            color.rgba = 0;
            return color;
        }
        else
        {
            float4 color;
            //color = (depth > 0) ? EncodeFloatRGB(depth) : EncodeFloatRGB(0);
            color.rgb = (normalize(input.Normal.xyz)) / 2.0f + 0.5f;
            color.a = (depth > 0) ? depth : 0;

            return color;
        }
    }
    else
    {
        discard;
        
        float4 color;
        //color = EncodeFloatRGB(0);
        color.rgba = 0;
        return color;
    }
    */

    
    float depth = projTexCoord.z;
    
    float4 prevDepthLayer = tex2D(depthMap, projTexCoord.xy);
    float prevDepth = DecodeFloatRGB(prevDepthLayer);
    
    if (projTexCoord.x >= 0 && projTexCoord.x <= 1 && projTexCoord.y >= 0 && projTexCoord.y <= 1 && saturate(projTexCoord).x == projTexCoord.x && saturate(projTexCoord).y == projTexCoord.y)
    {
        if (depth >= 1.0f - 1.0f / 5000.0f)
        {
            float4 color;
            color.rgba = 0;
            
            return color;
        }
        else
        {
            if (depth <= prevDepth + 1.0f / 4096.5f)
            {
                discard;
        
                float4 color;
                color = EncodeFloatRGB(depth);
            //color = EncodeFloatRGB(0);
            //color.a = 0;
                return color;
            }
            else
            {
                float4 color;
                color = EncodeFloatRGB(depth);
            //color = (depth > 0) ? EncodeFloatRGB(depth) : EncodeFloatRGB(0);
            //color.a = (depth > 0) ? 1 : 0;

                return color;
            }
        }
    }
    else
    {
        discard;
        
        float4 color;
        color = EncodeFloatRGB(depth);
        //color.a = 0;
        return color;
    }
}

float4 NormalPeelingPixelShader(VertexShaderOutput input) : COLOR0
{
    float4 projTexCoord = input.Position2D / input.Position2D.w;
    projTexCoord.xy = 0.5 * projTexCoord.xy + float2(0.5, 0.5);
    projTexCoord.y = 1.0 - projTexCoord.y; // invert Y direction (because UV map is opposite to y coordinate system)
    
    
    
    float depth = projTexCoord.z;
    
    float4 prevDepthLayer = tex2D(depthMap, projTexCoord.xy);
    float prevDepth = DecodeFloatRGB(prevDepthLayer);
    
    if (projTexCoord.x >= 0 && projTexCoord.x <= 1 && projTexCoord.y >= 0 && projTexCoord.y <= 1 && saturate(projTexCoord).x == projTexCoord.x && saturate(projTexCoord).y == projTexCoord.y)
    {
        if (depth <= prevDepth + 1.0f / 4096.5f)
        {
            discard;
        
            float4 color;
            color.rgb = 0;
            color.a = 0;

            return color;
        }
        else
        {
            float4 color;
            color.rgb = (normalize(input.Normal.xyz)) / 2.0f + 0.5f;
            color.a = 1;

            return color;
        }
    }
    else
    {
        discard;
        
        float4 color;
        color.rgb = 0;
        color.a = 0;
        
        return color;
    }
    

    /*
    float depth = projTexCoord.z;
    
    float4 prevDepthLayer = tex2D(depthMap, projTexCoord.xy);
    //float prevDepth = DecodeFloatRGB(prevDepthLayer);
    
    if (depth <= prevDepthLayer.a)
    {
        discard;
        
        float4 color;
        //color = EncodeFloatRGB(0);
        color.rgba = 0;
        return color;
    }
    else
    {
        float4 color;
        //color = (depth > 0) ? EncodeFloatRGB(depth) : EncodeFloatRGB(0);
        color.rgb = (normalize(input.Normal.xyz)) / 2.0f + 0.5f;
        color.a = (depth > 0) ? depth : 0;

        return color;
    }
    */
}
    

technique DepthMap
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 DepthMapVertexShader();
        PixelShader = compile ps_4_0 DepthMapPixelShader();
    }
}

technique NormalMap
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 DepthMapVertexShader();
        PixelShader = compile ps_4_0 NormalPixelShader();
    }
}

technique DepthPeeling
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 DepthPeelingVertexShader();
        PixelShader = compile ps_4_0 DepthPeelingPixelShader();
    }
}

technique NormalPeeling
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 DepthPeelingVertexShader();
        PixelShader = compile ps_4_0 NormalPeelingPixelShader();
    }
}