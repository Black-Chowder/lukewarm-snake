#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define PI 3.14159265359

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
    float4 final = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	
    float lightAngle = 5.0 * PI / 3.0;
	
    float dist = length(uv);
	
	//Outside circle
    if (dist > 1.0)
        final = float4(0.0, 0.0, 0.0, 0.0);
	
	//Inside circle
    else
    {
        float2 lightVector = float2(cos(lightAngle), sin(lightAngle));
        float shadowVal = dot(lightVector, normalize(uv));
        shadowVal = 1.0 - shadowVal;
        shadowVal *= dist;
        shadowVal = 1.0 - shadowVal;
        final = lerp(final, input.Color, shadowVal);
    }
	
    return final;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};