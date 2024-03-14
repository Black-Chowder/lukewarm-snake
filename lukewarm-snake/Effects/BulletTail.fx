#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0
#endif


//Scale multiplier of simplex noise input coordinates
#define SIMPLEX_SCALE 8.0
#define WARP_SIMPLEX_SCALE 6.0
#define WARP_INFLUENCE 0.025

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
float2 hash(in float2 p)  // this hash is not production ready, please
{                        // replace this by something better

    // 2D -> 1D
    int2 n = p.x * float2(3, 37) + p.y * float2(311, 113);

    // 1D hash by Hugo Elias
    n = (n << 13) ^ n;
    n = n * (n * n * 15731 + 789221) + 1376312589;
    return -1.0 + 2.0 * float2(n & int2(0x0fffffff, 0x0fffffff)) / float(0x0fffffff);
}

// return gradient noise (in x) and its derivatives (in yz)
float3 noised(in float2 p)
{
    int2 i = int2(floor(p));
    float2 f = frac(p);

    // quintic interpolation
    float2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
    float2 du = 30.0 * f * f * (f * (f - 2.0) + 1.0);  

    float2 ga = hash(i + int2(0, 0));
    float2 gb = hash(i + int2(1, 0));
    float2 gc = hash(i + int2(0, 1));
    float2 gd = hash(i + int2(1, 1));

    float va = dot(ga, f - float2(0.0, 0.0));
    float vb = dot(gb, f - float2(1.0, 0.0));
    float vc = dot(gc, f - float2(0.0, 1.0));
    float vd = dot(gd, f - float2(1.0, 1.0));

    return float3(va + u.x * (vb - va) + u.y * (vc - va) + u.x * u.y * (va - vb - vc + vd),   // value
        ga + u.x * (gb - ga) + u.y * (gc - ga) + u.x * u.y * (ga - gb - gc + gd) +  // derivatives
        du * (u.yx * (va - vb - vc + vd) + float2(vb, vc) - va));
}

//  <Simplex Noise >  //

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 fragColor = float4(0.0, 0.0, 0.0, 1.0);
    
    //Calculate sampling offset for noise values (warp)
    float2 warpNoisePos = input.TextureCoordinates.xy + float2(iTimer * 0.001, 1.0);
    warpNoisePos *= WARP_SIMPLEX_SCALE;
    float2 warpNoiseVal = noised(warpNoisePos).yz;

    //Calculate noise value
    float2 noiseSamplePos = input.TextureCoordinates.xy + float2(0.0, iTimer * 0.005) + warpNoiseVal * WARP_INFLUENCE;
    noiseSamplePos *= SIMPLEX_SCALE;
	float noiseVal = noised(noiseSamplePos).x;
    noiseVal = (noiseVal + 0.5) * 0.5; //Convert range from [-1,1] to [0,1]

    //Apply octave to noise value
    float2 octavePos = input.TextureCoordinates.xy + float2(1.0, iTimer * 0.0025) + warpNoiseVal * WARP_INFLUENCE;
    octavePos *= SIMPLEX_SCALE * 3.5;
    float octaveVal = noised(octavePos).x;
    noiseVal += octaveVal * 0.175;
    
    //Get mirrored gradient value
    float mirroredGradVal = input.TextureCoordinates.y * 0.5 + input.TextureCoordinates.x;
    if (input.TextureCoordinates.x > 0.5) 
        mirroredGradVal = input.TextureCoordinates.y * 0.5 + (1.0 - input.TextureCoordinates.x);
    mirroredGradVal = 1.0 - mirroredGradVal;

    //Increase gradient area
    mirroredGradVal = smoothstep(0.0, 1.5, mirroredGradVal);

    //Apply gradient to noise val
    noiseVal -= mirroredGradVal;

    //Make noise val solid color
    noiseVal = step(0.125, noiseVal);
    
    //Apply to red channel of output
    fragColor.r = noiseVal;


	return fragColor;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};