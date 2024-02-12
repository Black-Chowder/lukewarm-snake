#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0
#endif

float iTime;
float2 iResolution;

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
    float offset = (iTime - floor(iTime)) / iTime;
    float CurrentTime = iTime * offset;
	
    float3 WaveParams = float3(0.0, 0.8, 0.1);
	
    float ratio = iResolution.y / iResolution.x;
	
    float2 WaveCentre = float2(0.5, 0.5);
    //WaveCentre.y *= ratio;
	
    float2 texCoord = input.Position.xy / iResolution.xy;
    //texCoord.y *= ratio;
    float Dist = distance(texCoord, WaveCentre);
	
    float4 Color = tex2D(SpriteTextureSampler, texCoord);
	
    if ((Dist <= (CurrentTime + WaveParams.z)) && (Dist >= (CurrentTime - WaveParams.z)))
    {
        float Diff = (Dist - CurrentTime);
        float ScaleDiff = (1.0 - pow(abs(Diff * WaveParams.x), WaveParams.y));
        float DiffTime = (Diff * ScaleDiff);
        
        float2 DiffTexCoord = normalize(texCoord - WaveCentre);
        
        texCoord += ((DiffTexCoord * DiffTime) / (CurrentTime * Dist * 40.0));
        Color = tex2D(SpriteTextureSampler, texCoord);
        
        //Color += (Color * ScaleDiff) / 1.0; // (CurrentTime * Dist * 40.0);
        
    }
    
    return Color * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

/*
//Use as you will.

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    //Sawtooth function to pulse from centre.
    float offset = (iTime- floor(iTime))/iTime;
	float CurrentTime = (iTime)*(offset);    
    
	vec3 WaveParams = vec3(10.0, 0.8, 0.1 ); 
    
    float ratio = iResolution.y/iResolution.x;
    
    //Use this if you want to place the centre with the mouse instead
	//vec2 WaveCentre = vec2( iMouse.xy / iResolution.xy );
       
    vec2 WaveCentre = vec2(0.5, 0.5);
    WaveCentre.y *= ratio; 
   
	vec2 texCoord = fragCoord.xy / iResolution.xy;      
    texCoord.y *= ratio;    
	float Dist = distance(texCoord, WaveCentre);
    
	
	vec4 Color = texture(iChannel0, texCoord);
    
//Only distort the pixels within the parameter distance from the centre
if ((Dist <= ((CurrentTime) + (WaveParams.z))) && 
	(Dist >= ((CurrentTime) - (WaveParams.z)))) 
	{
        //The pixel offset distance based on the input parameters
		float Diff = (Dist - CurrentTime); 
		float ScaleDiff = (1.0 - pow(abs(Diff * WaveParams.x), WaveParams.y)); 
		float DiffTime = (Diff  * ScaleDiff);
        
        //The direction of the distortion
		vec2 DiffTexCoord = normalize(texCoord - WaveCentre);         
        
        //Perform the distortion and reduce the effect over time
		texCoord += ((DiffTexCoord * DiffTime) / (CurrentTime * Dist * 40.0));
		Color = texture(iChannel0, texCoord);
        
        //Blow out the color and reduce the effect over time
		Color += (Color * ScaleDiff) / (CurrentTime * Dist * 40.0);
	} 
    
	fragColor = Color; 
}
*/