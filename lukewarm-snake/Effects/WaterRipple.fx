﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float Damping;
float2 iResolution;

Texture2D Current;
sampler2D CurrentSampler = sampler_state
{
    Texture = <Current>;
};

Texture2D Previous;
sampler2D PreviousSampler = sampler_state
{
    Texture = <Previous>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 fragColor = float4(1.0, 1.0, 1.0, 1.0);
    fragColor = tex2D(CurrentSampler, input.TextureCoordinates);
    
    float2 texelSize = 1.0f / iResolution;
    
    fragColor = (
        tex2D(PreviousSampler, input.TextureCoordinates + float2(texelSize.x, 0)) +
        tex2D(PreviousSampler, input.TextureCoordinates + float2(-texelSize.x, 0)) +
        tex2D(PreviousSampler, input.TextureCoordinates + float2(0, texelSize.y)) +
        tex2D(PreviousSampler, input.TextureCoordinates + float2(0, -texelSize.y))
    ) / 2 - fragColor;
    
    fragColor *= Damping;
    
    return fragColor;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};