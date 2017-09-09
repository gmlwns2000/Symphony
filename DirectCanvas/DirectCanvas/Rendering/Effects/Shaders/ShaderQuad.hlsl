struct VSInput
{
    float4 PositionCS : POSITION;
    float2 TexCoord : TEXCOORD;
};

struct VSOutput
{
    float4 PositionCS : SV_Position;
    float2 TexCoord : TEXCOORD;
};

VSOutput QuadVS(in VSInput input)
{
    VSOutput output;

    output.PositionCS = input.PositionCS;
    output.TexCoord = input.TexCoord;
    return output;
}
