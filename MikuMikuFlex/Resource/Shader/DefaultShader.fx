float Script:STANDARDGLOBAL<string ScriptClass="scene";string Script="";> = 0.8;
float AmbientCoefficient=0.3;
float DiffuseCoefficient=1.0;
/*
変換行列系
*/
float4x4 matWVP : WORLDVIEWPROJECTION<string Object="Camera";>;
float4x4 matWV:WORLDVIEW < string Object = "Camera"; >;
float4x4 WorldMatrix : WORLD;
float4x4 ViewMatrix : VIEW;

float2 viewportSize:VIEWPORTPIXELSIZE;
float edgeSize:EDGETHICKNESS;

float4 ViewPointPosition:POSITION<string object="camera";>;
float4 LightPointPosition:POSITION<string object="light";>;

float4 clCol={0.2,0.6,0.2,1};

/*
スキニング
*/
float4x4 BoneTrans[512]:BONETRANS;

/*
マテリアル:テクスチャ
*/
Texture2D Texture:MATERIALTEXTURE;
Texture2D SphereTexture:MATERIALSPHEREMAP;
Texture2D Toon:MATERIALTOONTEXTURE;

bool spadd;

//材質単位で変わらないもの
cbuffer BasicMaterialConstant
{
	float4 AmbientColor:packoffset(c0);
	float4 DiffuseColor:packoffset(c1);
	float4 SpecularColor:packoffset(c2);
	float SpecularPower:packoffset(c3);
}

SamplerState mySampler
{
	// sampler state
   Filter = MIN_MAG_LINEAR_MIP_POINT;
   AddressU = MIRROR;
   AddressV = MIRROR;
};

SamplerState toonsmp
{
	// sampler state
   Filter = MIN_MAG_LINEAR_MIP_POINT;
   AddressU = MIRROR;
   AddressV = MIRROR;
};

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

struct MMM_SKINNING_OUTPUT
{
	float4 Position;
	float3 Normal;
};

struct VS_INPUT
{
	float4 pos:POSITION;
	float3 nor:NORMAL;
	float2 uv:UV;
	uint bi1:BONEA;
	uint bi2:BONEB;
	uint bi3:BONEC;
	uint bi4:BONED;
	float bw1:BONEWEIGHTA;
	float bw2:BONEWEIGHTB;
	float bw3:BONEWEIGHTC;
	float bw4:BONEWEIGHTD;
};


struct VS_OUTPUT_DEPTH
{
	float4 pos:SV_Position;
	float4 depth:POSITION;
};

//スキンメッシュアニメーション
float4x4 GetBoneLinerTransformMatrix(MMM_SKINNING_INPUT input)
{
	return BoneTrans[input.BlendIndices[0]]*input.BoneWeight[0]+BoneTrans[input.BlendIndices[1]]*input.BoneWeight[1]+BoneTrans[input.BlendIndices[2]]*input.BoneWeight[2]+BoneTrans[input.BlendIndices[3]]*input.BoneWeight[3];
}

//頂点シェーダ
struct VS_OUTPUT
{
	float4 Pos		:SV_Position;
    float2 Tex		: TEXCOORD1;   // テクスチャ
    float3 Normal	: TEXCOORD2;   // 法線
    float3 Eye		: TEXCOORD3;   // カメラとの相対位置
    float2 SpTex	: TEXCOORD4;	 // スフィアマップテクスチャ座標
    float4 Color	: COLOR0;      // ディフューズ色
};
VS_OUTPUT VS_Main(MMM_SKINNING_INPUT input,uint vid:SV_VertexID,uniform bool useTexture,uniform bool useSphereMap,uniform bool useToon)
{    
	VS_OUTPUT Out;
	
	//スキンメッシュアニメーション
	float4x4 bt=GetBoneLinerTransformMatrix(input);
	// カメラ視点のワールドビュー射影変換
	Out.Pos=mul(input.Pos,mul(bt,matWVP));
	
    // カメラとの相対位置
    Out.Eye = ViewPointPosition - mul( input.Pos, WorldMatrix );
    // 頂点法線
    Out.Normal = normalize( mul( input.Normal, (float3x3)WorldMatrix ) );
	
	// ディフューズ色＋アンビエント色 計算
    Out.Color.rgb =  DiffuseColor.rgb;
    Out.Color.a = DiffuseColor.a;
    Out.Color = saturate( Out.Color );
	
	Out.Tex=input.Tex;
	
    if ( useSphereMap ) {
        // スフィアマップテクスチャ座標
        float2 NormalWV = mul( Out.Normal, (float3x3)ViewMatrix );
        Out.SpTex.x = NormalWV.x * 0.5f + 0.5f;
        Out.SpTex.y = NormalWV.y * -0.5f + 0.5f;
    }
    
	return Out;
}

//ピクセルシェーダ
float4 PS_Main(VS_OUTPUT IN,uniform bool useTexture,uniform bool useSphereMap,uniform bool mulSphere,uniform bool useToon):SV_Target
{

    // スペキュラ色計算
	float3 LightDirection = -normalize(mul(LightPointPosition,matWV));
	float3 HalfVector = normalize(normalize(IN.Eye) + -mul(LightDirection, matWV));
    float3 Specular = pow( max(0.00001,dot( HalfVector, normalize(IN.Normal) )), SpecularPower ) * SpecularColor.rgb;

	float4 Color = IN.Color;
	//テクスチャカラー取得
	if(useTexture)
	{
		Color *= Texture.Sample(mySampler,IN.Tex);
	}
    if ( useSphereMap ) {
		if(mulSphere){
			Color.rgb *= SphereTexture.Sample(mySampler,IN.SpTex).rgb;
		}else{
        // スフィアマップ適用
        Color.rgb += SphereTexture.Sample(mySampler,IN.SpTex).rgb;
		}
    }
	// シェーディング
    float LightNormal = dot( IN.Normal, -mul(LightDirection,matWV) );
	float shading = saturate(LightNormal);//max(min(1,IN.Normal*saturate(-LightNormal * 16 + 0.5)),0);
    if ( useToon ) {

        float3 MaterialToon = Toon.Sample(toonsmp,float2(0,shading)).rgb;
        Color.rgb *= MaterialToon;
    }else
	{
        float3 MaterialToon = 1.0.xxx*shading;
        Color.rgb *= MaterialToon;
	}
    
    
    // スペキュラ適用
    Color.rgb += Specular;
	Color.rgb += AmbientColor.rgb*0.2;//TODO MMDのAmbientの係数がわからん・・・
	return Color;
}

//
//VS_OUTPUT_DEPTH VS_Depth(VS_INPUT input)
//{
//	VS_OUTPUT_DEPTH output=(VS_OUTPUT_DEPTH)0;
//	output.pos=mul(input.pos,mul(GetBoneLinerTransformMatrix(input),matWVP));
//	output.depth=output.pos;
//	return output;
//}
//
//float4 PS_Depth(VS_OUTPUT_DEPTH input):SV_Target
//{
//	float4 color=input.depth.z/input.depth.w;
//	return color;
//}
//
//technique10 ZplotTechnique<string MMDPass="zplot";>
//{
//	pass ZPlot
//	{
//		SetVertexShader(CompileShader(vs_4_0,VS_Depth()));
//		SetPixelShader(CompileShader(ps_4_0,PS_Depth()));
//	}
//}

//テクニック


technique10 TexturedObjectTechnique<string MMDPass="object";bool UseTexture=true;bool UseSphereMap=false;bool UseToon=true;>
{
	pass Textured
	{		
		SetVertexShader(CompileShader(vs_4_0, VS_Main(true,false,true)));
		SetPixelShader(CompileShader(ps_4_0, PS_Main(true,false,false,true)));
	}
}

technique10 UnTexturedObjectTechnique<string MMDPass="object";bool UseTexture=false;bool UseSphereMap=false;bool UseToon=true;>
{
	pass UnTextured
	{		
		SetVertexShader(CompileShader(vs_4_0, VS_Main(false,false,true)));
		SetPixelShader(CompileShader(ps_4_0, PS_Main(false,false,false,true)));
	}
}

technique10 SphereObjectTechnique<string MMDPass="object";bool UseTexture=true;bool UseSphereMap=true;bool MulSphere=false;bool UseToon=true;>
{
	pass TexturedAddSphere
	{
		SetVertexShader(CompileShader(vs_4_0,VS_Main(true,true,true)));
		SetPixelShader(CompileShader(ps_4_0, PS_Main(true,true,false,true)));
	}
}



technique10 SpheredUnTextureTechnique<string MMDPass="object";bool UseTexture=false;bool UseSphereMap=true;bool MulSphere=false;bool UseToon=true;>
{
		pass UnTexturedAddSphere
	{
		SetVertexShader(CompileShader(vs_4_0,VS_Main(false,true,true)));
		SetPixelShader(CompileShader(ps_4_0, PS_Main(false,true,false,true)));
	}
}

technique10 MulSphereObjectTechnique<string MMDPass = "object"; bool UseTexture = true; bool UseSphereMap = true; bool MulSphere = true; bool UseToon=true;>
{
    pass TexturedMulSphere
    {
        SetVertexShader(CompileShader(vs_4_0, VS_Main(true, true, true)));
        SetPixelShader(CompileShader(ps_4_0, PS_Main(true, true, true, true)));
    }
}

technique10 MulSpheredUnTextureTechnique<string MMDPass = "object"; bool UseTexture = false; bool UseSphereMap = true; bool MulSphere = true; bool UseToon=true;>
{
    pass UnTexturedMulSphere
    {
        SetVertexShader(CompileShader(vs_4_0, VS_Main(false, true, true)));
        SetPixelShader(CompileShader(ps_4_0, PS_Main(false, true, true, true)));
    }
}

//Toonなし
technique10 TexturedObjectTechniqueUnToon<string MMDPass="object";bool UseTexture=true;bool UseSphereMap=false;bool UseToon=false;>
{
	pass Textured
	{		
		SetVertexShader(CompileShader(vs_4_0, VS_Main(true,false,false)));
		SetPixelShader(CompileShader(ps_4_0, PS_Main(true,false,false,false)));
	}
}

technique10 UnTexturedObjectTechniqueUnToon<string MMDPass="object";bool UseTexture=false;bool UseSphereMap=false;bool UseToon=false;>
{
	pass UnTextured
	{		
		SetVertexShader(CompileShader(vs_4_0, VS_Main(false,false,false)));
		SetPixelShader(CompileShader(ps_4_0, PS_Main(false,false,false,false)));
	}
}

technique10 SphereObjectTechniqueUnToon<string MMDPass="object";bool UseTexture=true;bool UseSphereMap=true;bool MulSphere=false;bool UseToon=false;>
{
	pass TexturedAddSphere
	{
		SetVertexShader(CompileShader(vs_4_0,VS_Main(true,true,false)));
		SetPixelShader(CompileShader(ps_4_0, PS_Main(true,true,false,false)));
	}
}



technique10 SpheredUnTextureTechniqueUnToon<string MMDPass="object";bool UseTexture=false;bool UseSphereMap=true;bool MulSphere=false;bool UseToon=false;>
{
		pass UnTexturedAddSphere
	{
		SetVertexShader(CompileShader(vs_4_0,VS_Main(false,true,false)));
		SetPixelShader(CompileShader(ps_4_0, PS_Main(false,true,false,false)));
	}
}

technique10 MulSphereObjectTechniqueUnToon<string MMDPass = "object"; bool UseTexture = true; bool UseSphereMap = true; bool MulSphere = true; bool UseToon=false;>
{
    pass TexturedMulSphere
    {
        SetVertexShader(CompileShader(vs_4_0, VS_Main(true, true, false)));
        SetPixelShader(CompileShader(ps_4_0, PS_Main(true, true, true, false)));
    }
}

technique10 MulSpheredUnTextureTechniqueUnToon<string MMDPass = "object"; bool UseTexture = false; bool UseSphereMap = true; bool MulSphere = true; bool UseToon=false;>
{
    pass UnTexturedMulSphere
    {
        SetVertexShader(CompileShader(vs_4_0, VS_Main(false, true, false)));
        SetPixelShader(CompileShader(ps_4_0, PS_Main(false, true, true, false)));
    }
}
