#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float LightAngle;

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
    float2 uv = input.TextureCoordinates * 2.0 - 1.0;
    float4 fragColor = tex2D(SpriteTextureSampler, input.TextureCoordinates);
		
	//Calculate shadow value according to light angle
    float2 lightVector = float2(cos(LightAngle), sin(LightAngle));
    float shadowVal = dot(lightVector, normalize(uv));
    shadowVal = 1.0 - shadowVal;
	
    float dist = length(uv);
    shadowVal *= dist;
	
    shadowVal = pow(shadowVal, 1.0);
    shadowVal -= 0.5;

	//Only apply effect to non-transparent pixels
    if (fragColor.a > 0)
        fragColor = lerp(fragColor, input.Color, shadowVal);
	
    return fragColor;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};