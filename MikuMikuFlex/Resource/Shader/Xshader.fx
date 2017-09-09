float4x4 matWVP:WORLDVIEWPROJECTION<string Object="camera";>;
float4 LightPointPosition:POSITION<string object="light";>;

struct MMM_SKINNING_INPUT
{
	float4 Pos:POSITION;//頂点位置
	float4 BoneWeight:BLENDWEIGHT;
	uint4 BlendIndices:BLENDINDICES;
	float3 Normal:NORMAL;
	float2 Tex:TEXCOORD0;
	float4 AddUV1:TEXCOORD1;
	float4 AddUV2:TEXCOORD2;
	float4 AddUV3:TEXCOORD3;
	float4 AddUV4:TEXCOORD4;
	float4 SdefC:TEXCOORD5;
	float3 SdefR0:TEXCOORD6;
	float3 SdefR1:TEXCOORD7;
	float EdgeWeight:TEXCOORD8;
	uint Index:PSIZE15;
};

struct VS_SIMPLE_OUTPUT
{
	float4 Position:SV_Position;
	float3 Normal:NORMAL;
};

VS_SIMPLE_OUTPUT VS_Simple(MMM_SKINNING_INPUT input)
{
	VS_SIMPLE_OUTPUT Out;
	Out.Position=mul(input.Pos,matWVP);
	Out.Normal=input.Normal;
	return Out;
}

float4 PS_Simple(VS_SIMPLE_OUTPUT input):SV_Target
{
	float3 lightVector=normalize((float3)LightPointPosition-input.Position);
	float target=max(0,dot(input.Normal,lightVector));
	return float4(target,target,target,1);
}

technique10 SimpleTechnique
{
	pass Simple
	{
		SetVertexShader(CompileShader(vs_4_0, VS_Simple()));
		SetPixelShader(CompileShader(ps_4_0, PS_Simple()));
	}
}