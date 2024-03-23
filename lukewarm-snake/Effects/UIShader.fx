﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float2 iResolution;

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 fragColor = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    float2 texelSize = 1.0f / iResolution;
	
    //TODO: Get rid of if statements
    if (fragColor.a == 0)
    {
        for (int r = -1; r <= 1; r++)
        {
            for (int c = -1; c <= 1; c++)
            {
                float4 sampleColor = tex2D(
					SpriteTextureSampler,
					input.TextureCoordinates + texelSize * float2(r, c));
                
                if (sampleColor.a > 0)
                {
                    fragColor = float4(0.0, 0.0, 0.0, 1.0);
                }
            }
        }
    }
    else
        fragColor.rgba = 1;
	
    return fragColor;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};