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
 
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct TransformPixelInput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

float4 HyperbolicTransformation(TransformPixelInput p) : COLOR0
{
    //return tex2D(SpriteTextureSampler, (p.TexCoord));
    
    float2 inputp = (p.TexCoord - 0.5) * 2.0;
    float w_dot = dot(inputp.xy, inputp.xy);
    // bedziemy samplowac z obrazu w modelu Kleina'a
    float dotp = 2 / (1 + w_dot);
    float area = step(w_dot, 0.9);
    inputp *= (dotp);
    inputp = (inputp / 2.0) + 0.5;
    
    return area * tex2D(SpriteTextureSampler, inputp);
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL HyperbolicTransformation();
    }
};