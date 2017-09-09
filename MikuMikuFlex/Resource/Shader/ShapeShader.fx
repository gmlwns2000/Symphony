float4x4 matWVP:WORLDVIEWPROJECTION;
float4 col:COLOR;

struct VS_INPUT
{
	float4 Position:POSITION;
};

struct VS_OUTPUT
{
	float4 Position:SV_Position;
};

VS_OUTPUT VS(VS_INPUT input)
{
	VS_OUTPUT output;
	output.Position = mul(input.Position, matWVP);
	return output;
}

float4 PS(VS_OUTPUT output):SV_Target
{
	return col;
}

float PS_HitTest(VS_OUTPUT output):SV_Target
{
	return col.x;
}

technique10 DefaultTechnique
{
	pass Basic
	{
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetPixelShader(CompileShader(ps_4_0, PS()));
	}
	
	pass HitTest
	{
		SetVertexShader(CompileShader(vs_4_0, VS()));
		SetPixelShader(CompileShader(ps_4_0, PS_HitTest()));
	}
}