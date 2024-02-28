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

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 fragColor = float4(1.0, 1.0, 1.0, 1.0);
	
    float2 uv = input.TextureCoordinates * 2.0 - 1.0;
    
    float z = sqrt(1.0 - (pow(uv.x, 2.0) + pow(uv.y, 2.0)));
    float3 position = float3(uv, z);
    
    float3 normal = normalize(position) * 0.5 + 0.5;
    fragColor = float4(normal, 1.0);
	
	return fragColor;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};