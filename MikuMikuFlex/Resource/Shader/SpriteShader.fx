float4x4 matWVP:WORLDVIEWPROJECTION;
texture2D mapedDisplay:SPRITETEXTURE;
float3 transColor:TRANSCOLOR;
SamplerState mySampler{};

struct VS_OUTPUT
{
	float4 position:SV_Position;
	float4 rawPos:POSITION;
	float2 uv:UV;
};

VS_OUTPUT Sprite_VS(float4 pos:POSITION,float2 uv:UV)
{
	VS_OUTPUT vo;
	vo.position=mul(pos,matWVP);
	vo.rawPos=pos;
	vo.uv=uv;
	return vo;
}

float4 TSprite_PS(VS_OUTPUT vo):SV_Target
{
	float4 color=mapedDisplay.Sample(mySampler,float2(vo.uv.x,vo.uv.y));
	return float4(color.xyz,1);
}

float4 Sprite_PS(VS_OUTPUT vo):SV_Target
{
	return mapedDisplay.Sample(mySampler,float2(vo.uv.x,vo.uv.y));
}

technique10 defaultTechnique
{
	pass TransParentColorEnabledPass
	{
		SetVertexShader(CompileShader(vs_4_0,Sprite_VS()));
		SetPixelShader(CompileShader(ps_4_0,TSprite_PS()));
	}
	pass TransParentColorDisabledPass
	{
		SetVertexShader(CompileShader(vs_4_0,Sprite_VS()));
		SetPixelShader(CompileShader(ps_4_0,Sprite_PS()));
	}

}
