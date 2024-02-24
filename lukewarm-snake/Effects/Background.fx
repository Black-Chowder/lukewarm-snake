#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0
#endif

//  <Shader Constants>  //
#define FOAM_COLOR float4(1.0, 1.0, 1.0, 1.0)

//Thresholds for what should be colored FOAM_COLOR
#define LOWER_FOAM_THRESHOLD 0.0
#define UPPER_FOAM_THRESHOLD 1.0

//Scale multiplier of simplex noise input coordinates
#define SIMPLEX_SCALE 2.0

//Scaler for z-axis variation of noise over time
#define Z_TIMER_MULTIPLIER 0.8

//  </Shader Constants>  //

//Shader parameters
float iTimer;

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

//  <Simplex Noise> //

//------------------------------------------------------------------------------------------
// 3D Simplex Noise
//------------------------------------------------------------------------------------------

float3 mod289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
float3 permute(float3 x) { return mod289(((x * 34.0) + 1.0) * x); }

float3 grad3(float3 p) {
    const float norm = 1.0 / sqrt(3.0);
    float3 grad = float3(
        1.0, 1.0, -1.0) * p.x +
        float3(-1.0, 1.0, -1.0) * p.y +
        float3(1.0, -1.0, -1.0) * p.z;
    return normalize(grad) * norm;
}

float simplexNoise3D(float3 p) {
    const float F3 = 1.0 / 3.0;
    const float G3 = 1.0 / 6.0;

    float3 s = floor(p + dot(p, float3(F3, F3, F3)));
    float3 x0 = p - s + dot(s, float3(G3, G3, G3));

    float3 e = step(float3(0.0, 0.0, 0.0), x0 - x0.yzx);
    float3 i1 = e * (1.0 - e.zxy);
    float3 i2 = 1.0 - e.zxy * (1.0 - e);

    float3 x1 = x0 - i1 + G3;
    float3 x2 = x0 - i2 + 2.0 * G3;
    float3 x3 = x0 - 1.0 + 3.0 * G3;

    float4 w = max(0.5 - float4(dot(x0, x0), dot(x1, x1), dot(x2, x2), dot(x3, x3)), 0.0);
    float4 h = w * w * w * w * float4(dot(x0, grad3(permute(s + 0.0))),
        dot(x1, grad3(permute(s + i1))),
        dot(x2, grad3(permute(s + i2))),
        dot(x3, grad3(permute(s + 1.0))));

        return 32.0 * dot(h, float4(52.0, 52.0, 52.0, 52.0));
}

//  <Simplex Noise >  //

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 offset = float2(1.0, 1.0);
    offset *= iTimer;

    float2 sampleCoords2D = input.TextureCoordinates + iTimer;
    float val = simplexNoise3D(float3(((input.TextureCoordinates + iTimer) * SIMPLEX_SCALE).xy, iTimer * Z_TIMER_MULTIPLIER));

    float4 fragColor = float4(0.0, 0.0, 0.0, 0.0);
    if (val > LOWER_FOAM_THRESHOLD && val < UPPER_FOAM_THRESHOLD)
        fragColor = FOAM_COLOR;
    else
        fragColor = input.Color;

    return fragColor;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};