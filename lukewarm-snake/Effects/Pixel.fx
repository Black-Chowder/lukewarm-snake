#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

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

float2 CRTCurveUV(float2 uv)
{
	uv = uv * 2.0 - 1.0;
	float2 offset = abs(uv.yx) / float2(6.0, 4.0);
	uv = uv + uv * offset * offset;
	uv = uv * 0.5 + 0.5;
	return uv;
}

void DrawVignette(inout float3 color, float2 uv)
{
	float vignette = uv.x * uv.y * (1.0 - uv.x) * (1.0 - uv.y);
	vignette = clamp(pow(abs(16.0 * vignette), 0.3), 0.0, 1.0);
	color *= vignette;
}

void DrawScanline(inout float3 color, float2 uv)
{
	float scanline = clamp(0.95 + 0.05 * cos(3.14 * (uv.y + 0.008 * 1.0/*iTime*/) * 240.0 * 1.0), 0.0, 1.0);
	float grille = 0.85 + 0.15 * clamp(1.5 * cos(3.14 * uv.x * 640.0 * 1.0), 0.0, 1.0);
	color *= scanline * grille * 1.2;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 uv = CRTCurveUV(input.TextureCoordinates.xy);
	float4 pixel = tex2D(SpriteTextureSampler, uv) * input.Color;

	DrawScanline(pixel.rgb, uv);
	DrawVignette(pixel.rgb, input.TextureCoordinates.xy);

	return input.Color * pixel;
}



technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};