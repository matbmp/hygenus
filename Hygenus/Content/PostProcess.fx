#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
	#define GS_SHADERMODEL gs_4_0
#endif

// global object/variables
Texture2D SpriteTexture;
float K;

//	This is the sampler object that that will be used to sample the texture and return
//	back the color information about a particular pixel.  This is a sampler2D object,
//	and we created it by setting it equal to a 'sampler_state' struct and set the
//	Texture property equal to the Texture2D from above. 
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct TransformPixelInput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};


float3 qtransform(float4 q, float3 v)
{
	return v + 2.0 * cross(cross(v, q.xyz) + q.w * v, q.xyz);
};

float4 mobiusAddition(float4 b, float3 a)
{
    float3 c = K * cross(a.xyz, b.xyz);
    float d = 1.0f - K * dot(a.xyz, b.xyz);
    float3 t = a.xyz + b.xyz;
    float3 o = (t * d + cross(c, t)) / (d * d + dot(c, c));
    return float4(o.xyz, b.w);
};

float2 PoincareToKlein(float2 p)
{
    return p * 2.0f / (1.0f - K * dot(p, p));
}


float4 HyperbolicTransformation(TransformPixelInput p) : COLOR0
{
    //return tex2D(SpriteTextureSampler, (p.TexCoord));
    float2 poincare = (p.TexCoord - 0.5) * 2.0;
    float area = step(dot(poincare, poincare), 0.85);
    float2 klein = (PoincareToKlein(poincare) / 2.0) + 0.5;
    
    return tex2D(SpriteTextureSampler, klein)*area;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL HyperbolicTransformation();
    }
};