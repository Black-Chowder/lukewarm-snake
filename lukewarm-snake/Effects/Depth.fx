#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define BLACK float3(0, 0, 0)

float depth; //0 -> 1 (invisible to visible)

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
	float4 fragColor = tex2D(SpriteTextureSampler,input.TextureCoordinates);
	
	//Calculate color
	float isInputColor = 1 - step(0.5, depth);
	fragColor.rgb = BLACK * isInputColor + //if isInputColor, fragColor.rgb = BLACK
		lerp(BLACK, fragColor.rgb, depth - 0.5) * (1 - isInputColor); //else fragColor.rgb = lerp(BLACK< fragColor.rgb, depth - 0.5)
	
	//Calculate opacity
	fragColor.a = lerp(0, fragColor.a, depth);


	return fragColor * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};