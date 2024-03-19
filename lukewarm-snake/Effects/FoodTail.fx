#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0
#endif

#define TAIL_COLOR float3(29.0 / 255.0, 143.0 / 255.0, 0.0)
#define INNER_TAIL_COLOR float3(15.0 / 255.0, 77.0 / 255.0, 0.0)
#define MAIN_TAIL_OPACITY 0.5
#define OUTLINE_COLOR float3(0.0, 0.0, 0.0)

float2 iResolution;
float iTime;

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

float sdEllipse(in float2 p, in float2 ab)
{
    p = abs(p);
    if (p.x > p.y)
    {
        p = p.yx;
        ab = ab.yx;
    }
    float l = ab.y * ab.y - ab.x * ab.x;
    float m = ab.x * p.x / l;
    float m2 = m * m;
    float n = ab.y * p.y / l;
    float n2 = n * n;
    float c = (m2 + n2 - 1.0) / 3.0;
    float c3 = c * c * c;
    float q = c3 + m2 * n2 * 2.0;
    float d = c3 + m2 * n2;
    float g = m + m * n2;
    float co;
    if (d < 0.0)
    {
        float h = acos(q / c3) / 3.0;
        float s = cos(h);
        float t = sin(h) * sqrt(3.0);
        float rx = sqrt(-c * (s + t + 2.0) + m2);
        float ry = sqrt(-c * (s - t + 2.0) + m2);
        co = (ry + sign(l) * rx + abs(g) / (rx * ry) - m) / 2.0;
    }
    else
    {
        float h = 2.0 * m * n * sqrt(d);
        float s = sign(q + h) * pow(abs(q + h), 1.0 / 3.0);
        float u = sign(q - h) * pow(abs(q - h), 1.0 / 3.0);
        float rx = -s - u - c * 4.0 + 2.0 * m2;
        float ry = (s - u) * sqrt(3.0);
        float rm = sqrt(rx * rx + ry * ry);
        co = (ry / sqrt(rm - rx) + 2.0 * g / rm - m) / 2.0;
    }
    float2 r = ab * float2(co, sqrt(1.0 - co * co));
    return length(r - p) * sign(p.y - r.y);
}

float4 CalcTail(in float2 textureCoords)
{
    float4 fragColor = float4(0.0, 0.0, 0.0, 1.0);
    
    float offsetVal = sin((textureCoords.y + iTime) * 25.0);
    float2 ellipseSamplePos = textureCoords;
    ellipseSamplePos.x += offsetVal * lerp(0, 0.1, textureCoords.y);
	
    //Calculate distance to ellipse
    float2 p = ellipseSamplePos - float2(0.5, 0.5);
    float2 ra = float2(0.45, 0.5);
    float signedEllipseVal = sdEllipse(p, ra);
    
    //Separate inside from outside
    float val = 1.0 - step(0.0, signedEllipseVal);
    fragColor.rgb = TAIL_COLOR;
    fragColor.rgba *= val;
    fragColor.a = fragColor.a * (1 - val) + MAIN_TAIL_OPACITY * val;
    
    //Create inner tail
    p = ellipseSamplePos - float2(0.5, 0.5);
    ra = float2(0.125, 0.5);
    signedEllipseVal = sdEllipse(p, ra);
    float innerVal = 1.0 - step(0.0, signedEllipseVal);
	
    fragColor.rgb = INNER_TAIL_COLOR * innerVal + fragColor.rgb * (1 - innerVal);
    fragColor.a = fragColor.a * (1 - innerVal) + innerVal;
    
    return fragColor;
}


float4 MainPS(VertexShaderOutput input) : COLOR
{
    //Apply border effect using CalcTail as pseudo lookup texture
    float4 fragColor = CalcTail(input.TextureCoordinates);
    
    /*
    float2 texelSize = 1.0 / iResolution;
    if (fragColor.a != 0)
    {
        [unroll]
        for (int r = -1; r <= 1; r++)
        {
            [unroll]
            for (int c = -1; c <= 1; c++)
            {
                float4 sampleColor = CalcTail(input.TextureCoordinates + texelSize * float2(r, c));
                
                sampleColor.a = ceil(sampleColor.a);
                fragColor = sampleColor.a * fragColor + (1.0 - sampleColor.a) * float4(0.0, 0.0, 0.0, 1.0);
            }
        }

    }
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