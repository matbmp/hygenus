
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

float4x4 viewProjection;
float3 cameraTranslation;
float4 cameraRotation;
float3 objectTranslation;
float4 objectRotation;
float3 objectScale;
float4 color;
float K;
float PK;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};


struct VertexInput
{
	float4 Position : SV_POSITION;
	//float4 Color : COLOR0;
};
struct PixelInput
{
    float4 Position : SV_POSITION;
	//float4 Color : COLOR0;
};

float3 PoincareToKlein(float3 p)
{
    return p * 2.0f / (1.0f - K * dot(p, p));
}

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


float3 mobiusMultiplication(float a, float r)
{
    float l = length(a);
    //if (r == 0 || l == 0) return float3(0, 0, 0);
    float plus = pow((1 - K * l), r);
    float minus = pow((1 + K * l), r);
    float m = (-K * ((plus - minus) / (plus + minus)) / l);
    return float(a * m);
}



PixelInput VertexShaderLogic(VertexInput v)
{
    PixelInput output = (PixelInput) 0;
    float3 pos = v.Position;
    pos.xy *= objectScale.xy;
    float4 p = mobiusAddition(float4(qtransform(objectRotation, pos), 0.0), objectTranslation);
    float3 pp = qtransform(cameraRotation, mobiusAddition(p, cameraTranslation).xyz);
    pp = PoincareToKlein(pp * PK);
    float4 transformed = float4(pp, v.Position.w);
    
    output.Position = mul(transformed, viewProjection);

    return output;
};



float4 PixelShaderLogic(PixelInput p) : COLOR0
{
    return color;
};

technique SpriteDrawing
{
	pass P0
	{ 
        VertexShader = compile VS_SHADERMODEL VertexShaderLogic();
        PixelShader = compile PS_SHADERMODEL PixelShaderLogic();
    }
};