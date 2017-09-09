using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MMF.MME.VariableSubscriber.TextureSubscriber
{
    /// <summary>
    /// テクスチャのアノテーションを解釈する際に役立つメソッドをまとめた静的クラス
    /// </summary>
    internal static class TextureAnnotationParser
    {
        /// <summary>
        /// テクスチャのアノテーション解釈に一般的に必要な項目を取得するメソッド
        /// </summary>
        /// <param name="variable">エフェクト変数</param>
        /// <param name="context">レンダーコンテキスト</param>
        /// <param name="defaultFormat"></param>
        /// <param name="defaultViewPortRatio"></param>
        /// <param name="width">テクスチャの幅</param>
        /// <param name="height">テクスチャの高さ</param>
        /// <param name="depth">テクスチャの深さ(ない場合は-1)</param>
        /// <param name="mip">mipレベル</param>
        /// <param name="textureFormat">指定されているフォーマット</param>
        public static void GetBasicTextureAnnotations(EffectVariable variable, RenderContext context,Format defaultFormat,Vector2 defaultViewPortRatio,bool isTextureSubscriber, out int width, out int height, out int depth,
            out int mip, out Format textureFormat)
        {
            width = -1;
            height = -1;
            depth = -1;
            mip = 0;
            textureFormat = defaultFormat;
            EffectVariable rawWidthVal = EffectParseHelper.getAnnotation(variable, "width", "int");
            EffectVariable rawHeightVal = EffectParseHelper.getAnnotation(variable, "height", "int");
            EffectVariable rawDepthVal = EffectParseHelper.getAnnotation(variable, "depth", "int");
            if (rawWidthVal != null) width = rawWidthVal.AsScalar().GetInt();
            if (rawHeightVal != null) height = rawHeightVal.AsScalar().GetInt();
            if (rawDepthVal != null) depth = rawDepthVal.AsScalar().GetInt();
            EffectVariable rawViewportRatio = EffectParseHelper.getAnnotation(variable, "viewportratio", "float2");
            if (rawViewportRatio != null)
            {
                if (width != -1 || height != -1 || depth != -1)
                {
                    throw new InvalidMMEEffectShaderException(
                        string.Format("変数「{0} {1}」のサイズ指定が不正です。Width,Height,Depth/ViewportRatio/Dimensionsはそれぞれ同時に使用できません。",
                            variable.GetVariableType().Description.TypeName.ToLower(), variable.Description.Name));
                }
                else
                {
                    Vector4 rawRatio = rawViewportRatio.AsVector().GetVector();
                    Viewport vp = context.DeviceManager.Context.Rasterizer.GetViewports()[0];
                    width = (int)(vp.Width * rawRatio.X);
                    height = (int)(vp.Height * rawRatio.Y);
                }
            }
            EffectVariable rawDimensions = EffectParseHelper.getAnnotation(variable, "dimension", "uint2/uint3");
            if (rawDimensions != null)
            {
                if (width != -1 || height != -1 || depth != -1)
                {
                    throw new InvalidMMEEffectShaderException(
                        string.Format(
                            "変数「{0} {1}」のサイズ指定が不正です。Width,Height,Depth/ViewportRatio/Dimensionsはそれぞれ同時に使用できません。",
                            variable.GetVariableType().Description.TypeName.ToLower(), variable.Description.Name));
                }
                else
                {
                    string typeName = rawDimensions.GetVariableType().Description.TypeName.ToLower();
                    if (typeName == "int2")
                    {
                        int[] rawDimension = rawDimensions.AsVector().GetIntVectorArray(2);
                        width = rawDimension[0];
                        height = rawDimension[1];
                    }
                    else
                    {
                        int[] rawDimension = rawDimensions.AsVector().GetIntVectorArray(3);
                        width = rawDimension[0];
                        height = rawDimension[1];
                        depth = rawDimension[2];
                    }
                }
            }
            if (width == -1 || height == -1)
            {
                if (defaultViewPortRatio != Vector2.Zero)
                {
                    Viewport port = context.DeviceManager.Context.Rasterizer.GetViewports()[0];
                    width = (int) (port.Width*defaultViewPortRatio.X);
                    height = (int) (port.Height*defaultViewPortRatio.Y);
                }
                else
                {
                    if(!isTextureSubscriber)throw new InvalidMMEEffectShaderException(string.Format("width,heightのどちらかの指定は必須です。"));
                }
            }
            EffectVariable rawMipLevel = EffectParseHelper.getAnnotation(variable, "MipLevels", "int");
            EffectVariable rawLevel = EffectParseHelper.getAnnotation(variable, "levels", "int");
            if (rawMipLevel != null && rawLevel != null)
            {
                throw new InvalidMMEEffectShaderException(
                    string.Format("変数「{0} {1}」のミップマップレベルが重複して指定されています。「int Miplevels」、「int Levels」は片方しか指定できません。",
                        variable.GetVariableType().Description.TypeName.ToLower(), variable.Description.Name));
            }
            else if(rawMipLevel!=null||rawLevel!=null)
            {
                EffectVariable mipVal = rawMipLevel ?? rawLevel;
                mip = mipVal.AsScalar().GetInt();
            }
            EffectVariable rawFormat = EffectParseHelper.getAnnotation(variable, "format", "string");
            if (rawFormat != null)
            {
                string formatString = rawFormat.AsString().GetString();
                textureFormat = TextureFormat(formatString);
            }
        }

        public static Format TextureFormat(string formatString)
        {
            Format textureFormat;
            if (formatString.IndexOf('_') != -1)
            {
                //FMT_~~~,D3DFMT_~~~の場合は、~~~に統一
                string[] splitedFormat = formatString.Split('_');
                if (splitedFormat.Length > 2 && (splitedFormat[0].Equals("FMT") || splitedFormat[0].Equals("D3DFMT")))
                {
                    formatString = string.Join("_", splitedFormat, 1);
                }
                //DXGI_FORMAT_~~~の場合は、~~~に統一
                if (splitedFormat[0].Equals("DXGI") && splitedFormat[1].Equals("FORMAT"))
                {
                    formatString = string.Join("_", splitedFormat, 2);
                }
            }
            switch (formatString)
            {
                //DirectX9時代のテクスチャの認識部分
                case "R8G8B8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "A8R8G8B8":
                    textureFormat = Format.R8G8B8A8_UNorm;
                    break;
                case "X8R8G8B8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "R5G6B5":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "X1R5G5B5":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "A1R5G5B5":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "A4R4G4B4":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "R3G3B2":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "A8":
                    textureFormat = Format.A8_UNorm;
                    break;
                case "A8R3G3B2":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "X4R4G4B4":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "A2B10G10R10":
                    textureFormat = Format.R10G10B10A2_UNorm;
                    break;
                case "A8B8G8R8":
                    textureFormat = Format.R8G8B8A8_UNorm;
                    break;
                case "X8B8G8R8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "G16R16":
                    textureFormat = Format.R16G16_UNorm;
                    break;
                case "A2R10G10B10":
                    textureFormat = Format.R10G10B10A2_UNorm;
                    break;
                case "A16B16G16R16":
                    textureFormat = Format.R16G16B16A16_UNorm;
                    break;
                case "A8P8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "P8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "L8":
                    textureFormat = Format.R8_UNorm;
                    break;
                case "A8L8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "A4L4":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "V8U8":
                    textureFormat = Format.R8G8_SNorm;
                    break;
                case "L6V5U5":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "X8L8V8U8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "Q8W8V8U8":
                    textureFormat = Format.R8G8B8A8_SNorm;
                    break;
                case "V16U16":
                    textureFormat = Format.R16G16_SNorm;
                    break;
                case "W11V11U10":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "A2W10V10U10":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "UYVY":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "R8G8_B8G8":
                    textureFormat = Format.G8R8_G8B8_UNorm;
                    break;
                case "YUY2":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "G8R8_G8B8":
                    textureFormat = Format.R8G8_B8G8_UNorm;
                    break;
                case "DXT1":
                    textureFormat = Format.BC1_UNorm;
                    break;
                case "DXT2":
                    textureFormat = Format.BC1_UNorm;
                    break;
                case "DXT3":
                    textureFormat = Format.BC2_UNorm;
                    break;
                case "DXT4":
                    textureFormat = Format.BC2_UNorm;
                    break;
                case "DXT5":
                    textureFormat = Format.BC3_UNorm;
                    break;
                case "D16":
                    textureFormat = Format.D16_UNorm;
                    break;
                case "D16_LOCKABLE":
                    textureFormat = Format.D16_UNorm;
                    break;
                case "D32":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "D15S1":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "D24S8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "D24X8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "D24X4S4":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "D32F_LOCLABLE":
                    textureFormat = Format.D32_Float;
                    break;
                case "D24FS8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "S1D15":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "S8D24":
                    textureFormat = Format.D24_UNorm_S8_UInt;
                    break;
                case "X8D24":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "X4S4D24":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "L16":
                    textureFormat = Format.R16_UNorm;
                    break;
                case "INDEX16":
                    textureFormat = Format.R16_UInt;
                    break;
                case "INDEX32":
                    textureFormat = Format.R32_UInt;
                    break;
                case "Q16W16V16U16":
                    textureFormat = Format.R16G16B16A16_SNorm;
                    break;
                case "MULTI2_ARGB8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "R16F":
                    textureFormat = Format.R16_Float;
                    break;
                case "G16R16F":
                    textureFormat = Format.R16G16_Float;
                    break;
                case "A16B16G16R16F":
                    textureFormat = Format.R16G16B16A16_Float;
                    break;
                case "R32F":
                    textureFormat = Format.R32_Float;
                    break;
                case "G32R32F":
                    textureFormat = Format.R32G32_Float;
                    break;
                case "A32B32G32R32F":
                    textureFormat = Format.R32G32B32A32_Float;
                    break;
                case "CxV8U8":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットD3DFMT_{0} は利用不可能です。", formatString));
                case "D3DDECLTYPE_FLOAT1":
                    textureFormat = Format.R32_Float;
                    break;
                case "D3DDECLTYPE_FLOAT2":
                    textureFormat = Format.R32G32_Float;
                    break;
                case "D3DDECLTYPE_FLOAT3":
                    textureFormat = Format.R32G32B32_Float;
                    break;
                case "D3DDECLTYPE_FLOAT4":
                    textureFormat = Format.R32G32B32A32_Float;
                    break;
                case "D3DDECLTYPED3DCOLOR":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマット{0} は利用不可能です。", formatString));
                case "D3DDECLTYPE_UBYTE4":
                    textureFormat = Format.R8G8B8A8_UInt;
                    break;
                case "D3DDECLTYPE_SHORT2":
                    textureFormat = Format.R16G16_SInt;
                    break;
                case "D3DDECLTYPE_SHORT4":
                    textureFormat = Format.R16G16B16A16_SInt;
                    break;
                case "D3DDECLTYPE_UBYTE4N":
                    textureFormat = Format.R8G8B8A8_UNorm;
                    break;
                case "D3DDECLTYPE_SHORT2N":
                    textureFormat = Format.R16G16_SNorm;
                    break;
                case "D3DDECLTYPE_SHORT4N":
                    textureFormat = Format.R16G16B16A16_SNorm;
                    break;
                case "D3DDECLTYPE_USHORT2N":
                    textureFormat = Format.R16G16_UNorm;
                    break;
                case "D3DDECLTYPE_USHORT4N":
                    textureFormat = Format.R16G16B16A16_UNorm;
                    break;
                case "D3DDECLTYPE_UDEC3":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマット{0} は利用不可能です。", formatString));
                case "D3DDECLTYPE_DEC3N":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマット{0} は利用不可能です。", formatString));
                case "D3DDECLTYPE_FLOAT16_2":
                    textureFormat = Format.R16G16_Float;
                    break;
                case "D3DDECLTYPE_FLOAT16_4":
                    textureFormat = Format.R16G16B16A16_Float;
                    break;

                //DirectX11用のテクスチャフォーマット
                case "R32G32B32A32_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R32G32B32A32_FLOAT":
                    textureFormat = Format.R32G32B32A32_Float;
                    break;
                case "R32G32B32A32_UINT":
                    textureFormat = Format.R32G32B32A32_UInt;
                    break;
                case "R32G32B32A32_SINT":
                    textureFormat = Format.R32G32B32A32_SInt;
                    break;
                case "R32G32B32_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R32G32B32_FLOAT":
                    textureFormat = Format.R32G32B32_Float;
                    break;
                case "R32G32B32_UINT":
                    textureFormat = Format.R32G32B32_UInt;
                    break;
                case "R32G32B32_SINT":
                    textureFormat = Format.R32G32B32_SInt;
                    break;
                case "R16G16B16A16_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R16G16B16A16_FLOAT":
                    textureFormat = Format.R16G16B16A16_Float;
                    break;
                case "R16G16B16A16_UNORM":
                    textureFormat = Format.R16G16B16A16_UNorm;
                    break;
                case "R16G16B16A16_UINT":
                    textureFormat = Format.R16G16B16A16_UInt;
                    break;
                case "R16G16B16A16_SNORM":
                    textureFormat = Format.R16G16B16A16_SNorm;
                    break;
                case "R16G16B16A16_SINT":
                    textureFormat = Format.R16G16B16A16_SInt;
                    break;
                case "R32G32_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R32G32_FLOAT":
                    textureFormat = Format.R32G32_Float;
                    break;
                case "R32G32_UINT":
                    textureFormat = Format.R32G32_UInt;
                    break;
                case "R32G32_SINT":
                    textureFormat = Format.R32G32_SInt;
                    break;
                case "R32G8X24_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "D32_FLOAT_S8X24_UINT":
                    textureFormat = Format.D32_Float_S8X24_UInt;
                    break;
                case "R32_FLOAT_X8X24_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "X32_TYPELESS_G8X24_UINT":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R10G10B10A2_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R10G10B10A2_UNORM":
                    textureFormat = Format.R10G10B10A2_UNorm;
                    break;
                case "R10G10B10A2_UINT":
                    textureFormat = Format.R10G10B10A2_UInt;
                    break;
                case "R11G11B10_FLOAT":
                    textureFormat = Format.R11G11B10_Float;
                    break;
                case "R8G8B8A8_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R8G8B8A8_UNORM":
                    textureFormat = Format.R8G8B8A8_UNorm;
                    break;
                case "R8G8B8A8_UNORM_SRGB":
                    textureFormat = Format.R8G8B8A8_UNorm_SRGB;
                    break;
                case "R8G8B8A8_UINT":
                    textureFormat = Format.R8G8B8A8_UInt;
                    break;
                case "R8G8B8A8_SNORM":
                    textureFormat = Format.R8G8B8A8_SNorm;
                    break;
                case "R8G8B8A8_SINT":
                    textureFormat = Format.R8G8B8A8_SInt;
                    break;
                case "R16G16_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R16G16_FLOAT":
                    textureFormat = Format.R16G16_Float;
                    break;
                case "R16G16_UNORM":
                    textureFormat = Format.R16G16_UNorm;
                    break;
                case "R16G16_UINT":
                    textureFormat = Format.R16G16_UInt;
                    break;
                case "R16G16_SNORM":
                    textureFormat = Format.R16G16_SNorm;
                    break;
                case "R16G16_SINT":
                    textureFormat = Format.R16G16_SInt;
                    break;
                case "R32_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R32_FLOAT":
                    textureFormat = Format.R32_Float;
                    break;
                case "D32_FLOAT":
                    textureFormat = Format.D32_Float;
                    break;
                case "R32_UINT":
                    textureFormat = Format.R32_UInt;
                    break;
                case "R32_SINT":
                    textureFormat = Format.R32_SInt;
                    break;
                case "R24G8_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "D24_UNORM_S8_UINT":
                    textureFormat = Format.D24_UNorm_S8_UInt;
                    break;
                case "R24_UNORM_X8_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "X24_TYPELESS_G8_UINT":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R8G8_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R8G8_UNORM":
                    textureFormat = Format.R8G8_UNorm;
                    break;
                case "R8G8_UINT":
                    textureFormat = Format.R8G8_UInt;
                    break;
                case "R8G8_SNORM":
                    textureFormat = Format.R8G8_SNorm;
                    break;
                case "R8G8_SINT":
                    textureFormat = Format.R8G8_SInt;
                    break;
                case "R16_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R16_FLOAT":
                    textureFormat = Format.R16_Float;
                    break;
                case "D16_UNORM":
                    textureFormat = Format.D16_UNorm;
                    break;
                case "R16_UNORM":
                    textureFormat = Format.R16_UNorm;
                    break;
                case "R16_UINT":
                    textureFormat = Format.R16_UInt;
                    break;
                case "R16_SNORM":
                    textureFormat = Format.R16_SNorm;
                    break;
                case "R16_SINT":
                    textureFormat = Format.R16_SInt;
                    break;
                case "R8_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "R8_UNORM":
                    textureFormat = Format.R8_UNorm;
                    break;
                case "R8_UINT":
                    textureFormat = Format.R8_UInt;
                    break;
                case "R8_SNORM":
                    textureFormat = Format.R8_SNorm;
                    break;
                case "R8_SINT":
                    textureFormat = Format.R8_SInt;
                    break;
                case "A8_UNORM":
                    textureFormat = Format.A8_UNorm;
                    break;
                case "R1_UNORM":
                    textureFormat = Format.R1_UNorm;
                    break;
                case "R9G9B9E5_SHAREDEXP":
                    textureFormat = Format.R9G9B9E5_SharedExp;
                    break;
                case "R8G8_B8G8_UNORM":
                    textureFormat = Format.R8G8_B8G8_UNorm;
                    break;
                case "G8R8_G8B8_UNORM":
                    textureFormat = Format.G8R8_G8B8_UNorm;
                    break;
                case "BC1_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "BC1_UNORM":
                    textureFormat = Format.BC1_UNorm;
                    break;
                case "BC1_UNORM_SRGB":
                    textureFormat = Format.BC1_UNorm_SRGB;
                    break;
                case "BC2_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "BC2_UNORM":
                    textureFormat = Format.BC2_UNorm;
                    break;
                case "BC2_UNORM_SRGB":
                    textureFormat = Format.BC2_UNorm_SRGB;
                    break;
                case "BC3_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "BC3_UNORM":
                    textureFormat = Format.BC3_UNorm;
                    break;
                case "BC3_UNORM_SRGB":
                    textureFormat = Format.BC3_UNorm_SRGB;
                    break;
                case "BC4_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "BC4_UNORM":
                    textureFormat = Format.BC4_UNorm;
                    break;
                case "BC4_SNORM":
                    textureFormat = Format.BC4_SNorm;
                    break;
                case "BC5_TYPELESS":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "BC5_UNORM":
                    textureFormat = Format.BC5_UNorm;
                    break;
                case "BC5_SNORM":
                    textureFormat = Format.BC5_SNorm;
                    break;
                case "B5G6R5_UNORM":
                    textureFormat = Format.B5G6R5_UNorm;
                    break;
                case "B5G5R5A1_UNORM":
                    textureFormat = Format.B5G5R5A1_UNorm;
                    break;
                case "B8G8R8A8_UNORM":
                    textureFormat = Format.B8G8R8A8_UNorm;
                    break;
                case "B8G8R8X8_UNORM":
                    textureFormat = Format.B8G8R8X8_UNorm;
                    break;
                case "FORCE_UINT":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマットDXGI_FORMAT_{0} は利用不可能です。", formatString));
                case "UNKNOWN":
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマット{0} は指定できません。", formatString));
                default:
                    throw new InvalidMMEEffectShaderException(string.Format("フォーマット\"{0}\"が指定されましたが解釈できません。", formatString));
            }
            return textureFormat;
        }


    }
}