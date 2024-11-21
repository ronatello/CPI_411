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

struct VertexInput {
	float4 Position: POSITION;
	float4 Normal: NORMAL;
};

/* struct VertexOutput {
	float4 Position: POSITION;
	float4 Color: COLOR;
}; */

// For Per-Vertex Lighting
struct VertexShaderOutput {
	float4 Position: POSITION;
	float4 Color: COLOR;
	float4 Normal: TEXCOORD0;
	float4 WorldPosition: TEXCOORD1;
};

// For Per-Vertex Lighting - Phong
struct PhongVertexShaderOutput {
	float4 Position: POSITION;
	float4 Normal: TEXCOORD0;
	float4 WorldPosition: TEXCOORD1;
};

VertexShaderOutput GouraudVertexShaderFunction(VertexInput input) {
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	float4 screenPosition = mul(viewPosition, Projection);
	output.Position = screenPosition;
	
	output.WorldPosition = worldPosition;
	output.Normal = 0;


	// Per-vertex Lighting 90s
	float4 N = mul(input.Normal, WorldInverseTranspose);
	float3 L = normalize(DiffuseLightDirection);
	float3 R = reflect(-L, normalize(N.xyz));
	float3 V = normalize(CameraPosition - worldPosition.xyz);
	float3 H = normalize(V + L);

	// Color Calculation
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N.xyz, L));
	float4 specular = pow(max(0,dot(V, R)), Shininess) * SpecularColor * SpecularIntensity;
	output.Color = saturate(ambient + diffuse + specular);

	return output;
};

PhongVertexShaderOutput PhongVertexShaderFunction(VertexInput input) {
	PhongVertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	float4 screenPosition = mul(viewPosition, Projection);
	output.Position = screenPosition;
	
	output.WorldPosition = worldPosition;
	output.Normal = input.Normal;
	
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	//return saturate(input.Color + AmbientColor * AmbientIntensity);
	return input.Color;
};

float4 PhongPixelShaderFunction(PhongVertexShaderOutput input) : COLOR
{
	float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	//float3 L = normalize(DiffuseLightDirection); //directional light (currently -LP, use L = LP - P for spotlight)
	float3 L = normalize(DiffuseLightDirection - input.WorldPosition.xyz);
	//float3 R = reflect(-L, normalize(N.xyz));
	float3 R = normalize(reflect(-L, N));
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 H = normalize(V + L);

	// Color Calculation
	float4 ambient = AmbientColor * AmbientIntensity;
	float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
	float4 specular = pow(max(0,dot(V, R)), Shininess) * SpecularColor * SpecularIntensity;
	float4 color = saturate(ambient + diffuse + specular);
	color.a = 1;

	return color;
};

float4 BlinnPhongPixelShaderFunction(PhongVertexShaderOutput input) : COLOR
{
    float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	//float3 L = normalize(DiffuseLightDirection); //directional light (currently -LP, use L = LP - P for spotlight)
    float3 L = normalize(DiffuseLightDirection - input.WorldPosition.xyz);
	//float3 R = reflect(-L, normalize(N.xyz));
    float3 R = normalize(reflect(-L, N));
    float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
    float3 H = normalize(V + L);

	// Color Calculation
    float4 ambient = AmbientColor * AmbientIntensity;
    float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
    float4 specular = pow(max(0, dot(H, N)), Shininess) * SpecularColor * SpecularIntensity;
    float4 color = saturate(ambient + diffuse + specular);
    color.a = 1;

    return color;
};

float4 SchlickPixelShaderFunction(PhongVertexShaderOutput input) : COLOR
{
    float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	//float3 L = normalize(DiffuseLightDirection); //directional light (currently -LP, use L = LP - P for spotlight)
    float3 L = normalize(DiffuseLightDirection - input.WorldPosition.xyz);
	//float3 R = reflect(-L, normalize(N.xyz));
    float3 R = normalize(reflect(-L, N));
    float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
    float3 H = normalize(V + L);
    float t = normalize(dot(V, R)); // clamp or no?
    
	if (t == Shininess)
    {
        t = Shininess + 0.01f;
    };

	// Color Calculation
    float4 ambient = AmbientColor * AmbientIntensity;
    float4 diffuse = DiffuseIntensity * DiffuseColor * max(0, dot(N, L));
    float4 specular = max(0, (t / ((Shininess - t) * (Shininess + t)))) * SpecularColor * SpecularIntensity;
    float4 color = saturate(ambient + diffuse + specular);
    color.a = 1;

    return color;
};

float4 ToonPixelShaderFunction(PhongVertexShaderOutput input) : COLOR
{
	float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	//float3 L = normalize(DiffuseLightDirection); //directional light (currently -LP, use L = LP - P for spotlight)
	float3 L = normalize(DiffuseLightDirection - input.WorldPosition.xyz);
	//float3 R = reflect(-L, normalize(N.xyz));
	float3 R = normalize(reflect(-L, N));
	float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 H = normalize(V + L);

	// Color Calculation
	float D = dot(V, R);
	if(D < -0.7)
	{
	return float4( 0, 0, 0, 1);
	}
	else if(D < 0.2)
	{
	return float4( 0.25, 0.25, 0.25, 1);
	}
	else if(D < 0.97)
	{
	return float4( 0.5, 0.5, 0.5, 1);
	}
	else
	{
	return float4( 1, 1, 1, 1);
	}
};

float4 HalfLifeBlinnPixelShaderFunction(PhongVertexShaderOutput input) : COLOR
{
    float3 N = normalize(mul(input.Normal, WorldInverseTranspose).xyz);
	//float3 L = normalize(DiffuseLightDirection); //directional light (currently -LP, use L = LP - P for spotlight)
    float3 L = normalize(DiffuseLightDirection - input.WorldPosition.xyz);
	//float3 R = reflect(-L, normalize(N.xyz));
    float3 R = normalize(reflect(-L, N));
    float3 V = normalize(CameraPosition - input.WorldPosition.xyz);
    float3 H = normalize(V + L);

	// Color Calculation
    float4 ambient = AmbientColor * AmbientIntensity;
    float4 diffuse = DiffuseIntensity * DiffuseColor * pow((0.5 * (dot(N, L) + 1)), 2);
    float4 specular = pow(max(0, dot(H, N)), Shininess) * SpecularColor * SpecularIntensity;
    float4 color = saturate(ambient + diffuse + specular);
    color.a = 1;

    return color;
};

technique Gouraud
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 GouraudVertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
};

technique Phong
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 PhongVertexShaderFunction();
		PixelShader = compile ps_4_0 PhongPixelShaderFunction();
	}
};

technique Blinn
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 PhongVertexShaderFunction();
        PixelShader = compile ps_4_0 BlinnPhongPixelShaderFunction();
    }
};

technique Schlick
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 PhongVertexShaderFunction();
        PixelShader = compile ps_4_0 SchlickPixelShaderFunction();
    }
};

technique Toon
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 PhongVertexShaderFunction();
		PixelShader = compile ps_4_0 ToonPixelShaderFunction();
	}
};

technique HalfLifeBlinn
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 PhongVertexShaderFunction();
        PixelShader = compile ps_4_0 HalfLifeBlinnPixelShaderFunction();
    }
};