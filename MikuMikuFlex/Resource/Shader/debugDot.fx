float4x4 matWVP:WORLDVIEWPROJECTION;
float4 color:COLOR;


float4 VS(float4 pos:POSITION):SV_Position
{
	return mul(pos,matWVP);
}

float4 PS():SV_Target
{
	return color;
}

technique10 defTeq
{
	pass defPass
	{
		SetVertexShader(CompileShader(vs_4_0,VS()));
		SetPixelShader(CompileShader(ps_4_0,PS()));
	}
}