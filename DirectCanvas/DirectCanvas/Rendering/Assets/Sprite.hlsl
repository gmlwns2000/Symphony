//Big thanks to this link http://mynameismjp.wordpress.com/2010/11/14/d3d11-features MS-PL
//I think there's still a few lines of code in here from his original HLSL :)

//======================================================================================
// Constant buffers
//======================================================================================

cbuffer VSPerBatch
{
    float2 ViewportSize  : register (b0);
}


//======================================================================================
// Samplers
//======================================================================================

Texture2D	SpriteTexture1 : register(t0);
Texture2D	SpriteTexture2 : register(t1);
Texture2D	SpriteTexture3 : register(t2);
Texture2D	SpriteTexture4 : register(t3);
Texture2D	SpriteTexture5 : register(t4);
Texture2D	SpriteTexture6 : register(t5);
Texture2D	SpriteTexture7 : register(t6);
Texture2D	SpriteTexture8 : register(t7);
Texture2D	SpriteTexture9 : register(t8);
Texture2D	SpriteTexture10 : register(t9);
Texture2D	SpriteTexture11 : register(t10);
Texture2D	SpriteTexture12 : register(t11);
Texture2D	SpriteTexture13 : register(t12);
Texture2D	SpriteTexture14 : register(t13);
Texture2D	SpriteTexture15 : register(t14);
Texture2D	SpriteTexture16 : register(t15);

SamplerState SpriteSampler : register(s0);


//======================================================================================
// Input/Output structs
//======================================================================================
struct VSInputInstanced
{
    float2 Position : POSITION;
    float2 TexCoord : TEXCOORD;
    float4 Color : COLOR;
    float4 SourceRect : SOURCERECT;
	float2 TextureSize : TEXTURESIZE;
	int TextureIndex : TEXTUREINDEX;
	float2 Translation : TRANSLATION;
	float2 Scale : SCALEFACTOR;
	float3 Rotate : ROTATE;
	float2 RotationCenter : ROTATIONCENTER;
};

struct VSOutput
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD;
    float4 Color : COLOR;
	int TextureIndex : TEXTUREINDEX;
};

//======================================================================================
// Static Constants
//======================================================================================
static const float PI = 3.1415926535897932384f;

//======================================================================================
// Methods
//======================================================================================
float DegreeToRadian(float angle)
{
	return PI * angle / 180.0f;
}

float4x4 CreateTranslation(float x, float y, float z)
{
	float4x4 trans = float4x4( float4(1, 0, 0, x), 
							   float4(0, 1, 0, y),
							   float4(0, 0, 1, z),
							   float4(0, 0, 0, 1));
	return trans;
}

float4x4 CreateScale(float3 scale)
{
	float4x4 trans = float4x4( float4(scale.x, 0, 0, 0), 
							   float4(0, scale.y, 0, 0),
							   float4(0, 0, scale.z, 0),
							   float4(0, 0, 0, 1));
	return trans;
}

float4 CreateQuanterionFromAxisAngle(float3 axis, float angle)
{
	float mag = sqrt(axis.x * axis.x + axis.y * axis.y + axis.z * axis.z);

	float halfAngle = angle * 0.5f;
	float sinOfHalfAngle = sin(halfAngle);

	float x = axis.x / mag * sinOfHalfAngle;
	float y = axis.y / mag * sinOfHalfAngle;
	float z = axis.z / mag * sinOfHalfAngle;
	float w = cos(halfAngle);

	return float4(x, y, z, w);
}

float4 MulQuaternion(float4 left, float4 right)
{
	float4 result;

	result.w = left.w * right.w - left.x * right.x - left.y * right.y - left.z * right.z;
    result.x = left.w * right.x + left.x * right.w + left.y * right.z - left.z * right.y;
    result.y = left.w * right.y + left.y * right.w + left.z * right.x - left.x * right.z;
    result.z = left.w * right.z + left.z * right.w + left.x * right.y - left.y * right.x;

	return result;
}

float4x4 QuatToMatrix(float4 quaternion)
{
	float xx = quaternion.x * quaternion.x;
    float yy = quaternion.y * quaternion.y;
    float zz = quaternion.z * quaternion.z;
    float xy = quaternion.x * quaternion.y;
    float zw = quaternion.z * quaternion.w;
    float zx = quaternion.z * quaternion.x;
    float yw = quaternion.y * quaternion.w;
    float yz = quaternion.y * quaternion.z;
    float xw = quaternion.x * quaternion.w;

	float4x4 trans = float4x4( float4(1.0f - (2.0f * (yy + zz)), 2.0f * (xy + zw), 2.0f * (zx - yw), 0),
							   float4(2.0f * (xy - zw), 1.0f - (2.0f * (zz + xx)), 2.0f * (yz + xw), 0),
							   float4(2.0f * (zx + yw), 2.0f * (yz - xw), 1.0f - (2.0f * (yy + xx)), 0),
							   float4(0, 0, 0, 1));
	return trans;
}

float4x4 CreateIdentity()
{
	float4x4 trans = float4x4( float4(1, 0, 0, 0), 
							   float4(0, 1, 0, 0),
							   float4(0, 0, 1, 0), 
							   float4(0, 0, 0, 1) );
	return trans;
}

float4x4 MatrixTransformation(float3 scalingCenter, float3 rotationCenter, float4 rotation, float3 translation)
{
	float4x4 m1, m2, m3, m4, m5, m6, m7, p1, p2, p3, p4, p5;
	float4 rc;
    float3 sc, pt;

	sc.x = scalingCenter.x;
    sc.y = scalingCenter.y;
    sc.z = scalingCenter.z;

	rc.x = rotationCenter.x;
	rc.y = rotationCenter.y;
	rc.z = rotationCenter.z;

	pt.x = translation.x;
    pt.y = translation.y;
    pt.z = translation.z;

	m1 = CreateTranslation(-sc.x, -sc.y, -sc.z);

	m2 = CreateIdentity();
	m3 = CreateIdentity();
	m4 = CreateIdentity();

	m6 = QuatToMatrix(rotation);

	m5 = CreateTranslation(sc.x - rc.x,  sc.y - rc.y,  sc.z - rc.z);
	m7 = CreateTranslation(rc.x + pt.x, rc.y + pt.y, rc.z + pt.z);

	p1 = mul(m1, m2);
	p2 = mul(p1, m3);
	p3 = mul(p2, m4);
	p4 = mul(p3, m5);
	p5 = mul(p4, m6);

	return mul(p5, m7);
}


//-------------------------------------------------------------------------------------
// Functionality common to both vertex shaders
//-------------------------------------------------------------------------------------
VSOutput SpriteVSCommon(float2 position,
                        float2 texCoord,
                        float4 color,
                        float4 sourceRect,
						float2 textureSize,
						int textureIndex, 
						float2 translation, 
						float2 scaleFactor, 
						float3 rotate, float2 rotationCenter)
{
	float4 quat = float4(0, 0, 0, 1);
	quat = MulQuaternion(quat, CreateQuanterionFromAxisAngle(float3(1,0,0), DegreeToRadian(rotate.x)));
	quat = MulQuaternion(quat, CreateQuanterionFromAxisAngle(float3(0,1,0), DegreeToRadian(rotate.y)));
	quat = MulQuaternion(quat, CreateQuanterionFromAxisAngle(float3(0,0,1), DegreeToRadian(rotate.z)));
	
	float4x4 scale = CreateScale(float3(scaleFactor.x, scaleFactor.y, 0));
	float4x4 translate = CreateTranslation(translation.x, translation.y, 1);

	
	float4x4 transform = MatrixTransformation(float3(1,1,1), float3(-sourceRect.z * rotationCenter.x, -sourceRect.w * rotationCenter.y, 0), quat, float3(0,0,0));

	transform = mul(scale, transform );
	transform = mul(translate, transform );

	transform = transpose(transform);

    // Scale the quad so that it's texture-sized
    float4 positionScreenSpace = float4(position * (sourceRect.zw), 0.0f, 1);

    // Apply transforms in screen space
    positionScreenSpace = mul(positionScreenSpace, transform);

    // Scale by the viewport size, flip Y, then rescale to device coordinates
    float4 positionDeviceSpace = positionScreenSpace;
    positionDeviceSpace.xy /= ViewportSize;
	
    positionDeviceSpace = (positionDeviceSpace * 2.0f) - 1;
    positionDeviceSpace.y *= -1;

    // Figure out the texture coordinates
    float2 outTexCoord = texCoord;
    outTexCoord.xy *= sourceRect.zw / (textureSize);
    outTexCoord.xy += sourceRect.xy / (textureSize);

    VSOutput output;
    output.Position = positionDeviceSpace;
    output.TexCoord = outTexCoord;
    output.Color = color;
	output.TextureIndex = textureIndex;
    return output;
}

//======================================================================================
// Vertex Shader
//======================================================================================
VSOutput SpriteInstancedVS(in VSInputInstanced input)
{
    return SpriteVSCommon(input.Position, input.TexCoord, input.Color, input.SourceRect, input.TextureSize, input.TextureIndex, input.Translation, input.Scale, input.Rotate, input.RotationCenter);
}

//======================================================================================
// Pixel Shader
//======================================================================================
float4 SpritePS(in VSOutput input) : SV_Target
{
    float4 texColor = 0;
	
	SamplerState spriteSampler = SpriteSampler;

	switch(input.TextureIndex)
	{
		case 0:
			texColor = SpriteTexture1.Sample(spriteSampler, input.TexCoord);
			break; 
		case 1:
			texColor = SpriteTexture2.Sample(spriteSampler, input.TexCoord);
			break;
		case 2:
			texColor = SpriteTexture3.Sample(spriteSampler, input.TexCoord);
			break;
		case 3:
			texColor = SpriteTexture4.Sample(spriteSampler, input.TexCoord);
			break;
		case 4:
			texColor = SpriteTexture5.Sample(spriteSampler, input.TexCoord);
			break; 
		case 5:
			texColor = SpriteTexture6.Sample(spriteSampler, input.TexCoord);
			break;
		case 6:
			texColor = SpriteTexture7.Sample(spriteSampler, input.TexCoord);
			break;
		case 7:
			texColor = SpriteTexture8.Sample(spriteSampler, input.TexCoord);
			break;
		case 8:
			texColor = SpriteTexture9.Sample(spriteSampler, input.TexCoord);
			break; 
		case 9:
			texColor = SpriteTexture10.Sample(spriteSampler, input.TexCoord);
			break;
		case 10:
			texColor = SpriteTexture11.Sample(spriteSampler, input.TexCoord);
			break;
		case 11:
			texColor = SpriteTexture12.Sample(spriteSampler, input.TexCoord);
			break;
		case 12:
			texColor = SpriteTexture13.Sample(spriteSampler, input.TexCoord);
			break;
		case 13:
			texColor = SpriteTexture14.Sample(spriteSampler, input.TexCoord);
			break; 
		case 14:
			texColor = SpriteTexture15.Sample(spriteSampler, input.TexCoord);
			break;
		case 15:
			texColor = SpriteTexture16.Sample(spriteSampler, input.TexCoord);
			break;
		default:
			break;
	}

	texColor *= input.Color;

    return texColor;
}
