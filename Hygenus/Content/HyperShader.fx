
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

float4x4 view;
float4x4 projection;
float3 cameraTranslation;
float4 cameraRotation;
float3 objectTranslation;
float4 objectRotation;
float3 objectScale;
float4 color;
float K;
float par;
bool ignore_texture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
    AddressU = Wrap;
    AddressV = Wrap;
};


struct VertexInput
{
	float4 Position : SV_POSITION;
    float2 TextureUV  : TEXCOORD0;
};
struct PixelInput
{
    float4 Pos : SV_POSITION0;
    float2 TextureUV  : TEXCOORD1;
    float w_dot : TEXCOORD2;
};

float3 qtransform(float4 q, float3 v)
{
	return v + 2.0 * cross(cross(v, q.xyz) + q.w * v, q.xyz);
};

// HyperMath.cs
float2 mobiusAddition(float2 b, float2 a)
{
    float k = -K;
    float adot = dot(a, a), bdot = dot(b, b), abdot2 = 2 * dot(a, b);
    float x = (1 + k * (abdot2 + bdot));
    float y = (1 - k * adot);
    float z = (1 + k * (abdot2 + k * adot * bdot));
    return (x * a + y * b) / z;
}

PixelInput VertexShaderLogic(VertexInput v)
{
    PixelInput output = (PixelInput) 0;

    //standardowe przeksztalcenia hyperboliczne, wejściem oraz wyjściem są wektory w modelu Poincare'a
    // ? czy możliwe jest wykonanie ich przy pomocy macierzy ?
    float4 p = v.Position;
    p.xy *= objectScale.xy;
    v.TextureUV.xy *= objectScale.xy;
    p.xyz = qtransform(objectRotation, p);
    p.xy = mobiusAddition(p.xy, objectTranslation.xy);  // pozycje wektorów w świecie
    p.xy = mobiusAddition(p, cameraTranslation.xy);
    p.xyz = qtransform(cameraRotation, p);
    float4 transformed = float4(p.xyz, v.Position.w);
    
    // obliczamy kwadrat odległości od środka
    float w_dot = dot(p.xyz, p.xyz);
    //w_dot = max(w_dot, 0.0001);

    //obliczamy skalowanie do modelu ???
    float dotp = (1.0 + w_dot) / (1-w_dot * w_dot);

    // transformacja wektorów do modelu Beltrami-Klein'a (który zachowuje proste linie) ( 2 / (1-|x|^2) )
    transformed.w /= 2.0 / (1 + w_dot);
    
    // wektor na wyjście, właściwie to nie potrzebujemy projekcji, ponieważ model zawiera się w kwadracie od (-1, -1) do (1, 1)
    output.Pos = mul(projection * view, transformed);
    
    // korekcja interpolacji tekstur, zamieniamy liniową interpolację na interpolację w modelu ??? ~( 1 / ( 1 - |x|^4) )
    output.TextureUV = v.TextureUV / (dotp);
    output.w_dot = 1.0 / dotp;

    return output;
};



float4 PixelShaderLogic(PixelInput p) : COLOR0
{
    float4 fragcol;
    if (ignore_texture == true) {
        fragcol = color;
    }
    else {
        fragcol = tex2D(SpriteTextureSampler, p.TextureUV / (p.w_dot)) * color;
    }
    return fragcol;
};

technique SpriteDrawing
{
	pass P0
	{ 
        VertexShader = compile VS_SHADERMODEL VertexShaderLogic();
        PixelShader = compile PS_SHADERMODEL PixelShaderLogic();
    }
};