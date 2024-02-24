#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, WorldViewProjection);
	output.Color = input.Color;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return input.Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};


// GlowEffect.fx

float glowStrength = 1.5f;

cbuffer Constants : register(b0)
{
    float blockSize;
}

struct GameBoardCell
{
    float4 Color; // Assuming Color.xyz represents the color and Color.w represents the ShouldGlow value
};

Texture2D<float4> GameBoardTexture : register(t0);

sampler PointSampler : register(s0)
{
    Filter = POINT;
    AddressU = CLAMP;
    AddressV = CLAMP;
};


float4 PixelShaderFunction(float2 position : VPOS, float4 color : COLOR0) : COLOR0
{
    // Convert position to normalized device coordinates
    float2 cellPosition = position;

    // Get the ShouldGlow value directly from the red channel of the texture
    float ShouldGlow = tex2D(PointSampler, cellPosition).r;

    // Apply the glow effect if ShouldGlow is true
    float3 newColor = color.rgb * ShouldGlow * glowStrength;
    
    return float4(newColor, color.a);
}

technique GlowEffectTechnique
{
    pass GlowEffectPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
