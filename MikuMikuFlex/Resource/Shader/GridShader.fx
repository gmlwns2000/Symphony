
float4x4 matMVP : WORLDVIEWPROJECTION;

struct VS_OUTPUT_AXIS
{
	float4 Position:SV_Position;
	float4 Color:COLOR;
};

struct VS_INPUT_LOCAL_AXIS
{
	float4 Position:POSITION;
	float4 Color:COLOR;
};

struct VS_INPUT_AXIS
{
	float4 Position:POSITION;
	float4 Color:COLOR;
};

float4 VS_Grid(float4 pos:POSITION):SV_Position
{
	return mul(pos,matMVP);
}

float4 PS_Grid():SV_Target
{
	return float4(0.2f,0.2f,0.2f,1);
}

VS_OUTPUT_AXIS VS_Axis_Grid(VS_INPUT_AXIS input)
{
	VS_OUTPUT_AXIS Out;
	Out.Position=mul(input.Position,matMVP);
	Out.Color=input.Color;
	return Out;
}

float4 PS_Axis_Grid(VS_OUTPUT_AXIS input):SV_Target
{
	return input.Color;
}

technique10 DefaultTechnique
{	
	pass NormalGrid
	{
		SetVertexShader(CompileShader(vs_4_0,VS_Grid()));
		SetPixelShader(CompileShader(ps_4_0,PS_Grid()));
	}

	pass AxisGrid
	{
		SetVertexShader(CompileShader(vs_4_0,VS_Axis_Grid()));
		SetPixelShader(CompileShader(ps_4_0,PS_Axis_Grid()));
	}
}