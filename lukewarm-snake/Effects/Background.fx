#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0
#endif

//  <Shader Constants>  //
#define PI 3.14159265359

//  <Foam Constants>    //
#define FOAM_COLOR float4(1.0, 1.0, 1.0, 1.0)

//Thresholds for what should be colored FOAM_COLOR
#define LOWER_FOAM_THRESHOLD 0.0
#define UPPER_FOAM_THRESHOLD 1.0

//Scale multiplier of simplex noise input coordinates
#define SIMPLEX_SCALE 2.0

//Scaler for z-axis variation of noise over time
#define Z_TIMER_MULTIPLIER 0.8

//Starting size of wave
#define WAVE_RADIUS 0.3
//  </Foam Constants>   //

//  </Shader Constants> //

//Shader parameters
float iTimer;
float2 iWaveCenter;
//float2 texelSize;
float damping;
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
    float4 fragColor = float4(1.0, 1.0, 1.0, 1.0);
    fragColor = tex2D(CurrentSampler, input.TextureCoordinates);
    
    float2 texelSize = 1.0f / iResolution;
    
    fragColor = (
        tex2D(PreviousSampler, input.TextureCoordinates + float2(texelSize.x, 0)) +
        tex2D(PreviousSampler, input.TextureCoordinates + float2(-texelSize.x, 0)) +
        tex2D(PreviousSampler, input.TextureCoordinates + float2(0, texelSize.y)) +
        tex2D(PreviousSampler, input.TextureCoordinates + float2(0, -texelSize.y))
    ) / 2 - fragColor;
    
    fragColor *= damping;
    /*
    if (input.TextureCoordinates.x > 0.5)
    {
        fragColor = tex2D(PreviousSampler, input.TextureCoordinates);
    }
    else
    {
        fragColor = tex2D(CurrentSampler, input.TextureCoordinates);
    }
    */
    //else
    //{
    //    fragColor = tex2D(CurrentSampler, input.TextureCoordinates);
    //}
    

    /* Idea behind moving the normal in their direction
    float2 pixelSize = 1.0 / iResolution;

    float kernelSize = 8.0;
    float3 final = float3(0.0, 0.0, 0.0);
    for (float x = -kernelSize / 2.0; x < kernelSize / 2.0; x++)
    {
        for (float y = -kernelSize / 2.0; y < kernelSize / 2.0; y++)
        {
            float4 other = tex2D(SpriteTextureSampler, input.TextureCoordinates + pixelSize * float2(x, y));
            other.xy = other.xy * 2.0 - 1.0;

            float otherNormalAngle = atan2(other.y, other.x);

            float angleToSelf = atan2(y, x);

            float otherInfluenceAngle = clamp(dot(otherNormalAngle, angleToSelf), 0.0, 1.0);

            float2 otherInfluence = float2(cos(otherInfluenceAngle), sin(otherInfluenceAngle));

            final += float3(otherInfluence, other.a);
        }
    }
    final /= pow(kernelSize, 2.0);
    final = final * 0.5 + 0.5;
    fragColor = float4(final.xy, 0.0, final.z);
    */


    /*
    float Tau = 6.28318530718; // Pi*2
    
    // GAUSSIAN BLUR SETTINGS {{{
    float Directions = 16.0; // BLUR DIRECTIONS (Default 16.0 - More is better but slower)
    float Quality = 3.0; // BLUR QUALITY (Default 4.0 - More is better but slower)
    float Size = 8.0; // BLUR SIZE (Radius)
    // GAUSSIAN BLUR SETTINGS }}}
   
    float2 Radius = Size / iResolution.xy;
    
    // Normalized pixel coordinates (from 0 to 1)
    float2 uv = input.TextureCoordinates/ iResolution.xy;
    // Pixel colour
    float4 Color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    
    // Blur calculations
    for (float d = 0.0; d < Tau; d += Tau / Directions)
    {
        for (float i = 1.0 / Quality; i <= 1.0; i += 1.0 / Quality)
        {
            Color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(cos(d), sin(d)) * Radius * i);
        }
    }
    
    // Output to screen
    Color /= Quality * Directions - 15.0;
    fragColor.r = Color.r;
    fragColor.gba = float3(0.0, 0.0, 1.0);
    */
    
    /*
    float2 offset = float2(1.0, 1.0);
    offset *= iTimer;

    float2 sampleCoords2D = input.TextureCoordinates + iTimer;
    float val = simplexNoise3D(float3(((input.TextureCoordinates + iTimer) * SIMPLEX_SCALE).xy, iTimer * Z_TIMER_MULTIPLIER));

    fragColor = float4(0.0, 0.0, 0.0, 0.0);
    if (val > LOWER_FOAM_THRESHOLD && val < UPPER_FOAM_THRESHOLD)
        fragColor = FOAM_COLOR;
    else
        fragColor = input.Color;
*/

    return fragColor;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};