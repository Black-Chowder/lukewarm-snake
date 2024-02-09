#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float2 texelSize;
float4 OutlineColor;

texture SpriteTexture;

sampler SpriteTextureSampler = sampler_state
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
	float4 texColor = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;

	if (texColor.a == 0) {

		[unroll]
		for (int r = -1; r <= 1; r++) {
			[unroll]
			for (int c = -1; c <= 1; c++) {
				//if (abs(r) == abs(c))
				//	continue;

				float4 sampleColor = tex2D(
					SpriteTextureSampler,
					input.TextureCoordinates + texelSize * float2(r, c));

				if (sampleColor.a > 0) {
					texColor = OutlineColor;
				}
			}
		}
	}

	return texColor;
	//return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};