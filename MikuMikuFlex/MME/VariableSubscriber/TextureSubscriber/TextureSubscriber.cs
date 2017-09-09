using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MMF.MME.VariableSubscriber.TextureSubscriber
{
    internal class TextureSubscriber : SubscriberBase, System.IDisposable
    {
        private ShaderResourceView resourceView;

        private SlimDX.Direct3D11.Resource resourceTexture;

        public override string Semantics
        {
            get
            {
                throw new System.InvalidOperationException("このサブスクライバにはセマンティクスを持ちません");
            }
        }

        public override VariableType[] Types
        {
            get
            {
                return new VariableType[]
                {
                    VariableType.Texture,
                    VariableType.Texture2D,
                    VariableType.Texture3D,
                    VariableType.TextureCUBE
                };
            }
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            TextureSubscriber textureSubscriber = new TextureSubscriber();
            string text = variable.GetVariableType().Description.TypeName.ToLower();
            int width;
            int height;
            int depth;
            int mipLevels;
            Format format;
            TextureAnnotationParser.GetBasicTextureAnnotations(variable, context, Format.B8G8R8A8_UNorm, Vector2.Zero, true, out width, out height, out depth, out mipLevels, out format);
            EffectVariable annotation = EffectParseHelper.getAnnotation(variable, "ResourceType", "string");
            EffectVariable annotation2 = EffectParseHelper.getAnnotation(variable, "ResourceName", "string");
            string text3;
            if (annotation != null)
            {
                string text2 = annotation.AsString().GetString().ToLower();
                if (text2 != null)
                {
                    if (!(text2 == "cube"))
                    {
                        if (!(text2 == "2d"))
                        {
                            if (!(text2 == "3d"))
                            {
                                goto ILOVECODING;
                            }
                            if (!text.Equals("texture3d"))
                            {
                                throw new InvalidMMEEffectShaderException("ResourceTypeには3Dが指定されていますが、型がtexture3dではありません。");
                            }
                            text3 = "texture3d";
                        }
                        else
                        {
                            if (!text.Equals("texture2d") && !text.Equals("texture"))
                            {
                                throw new InvalidMMEEffectShaderException("ResourceTypeには2Dが指定されていますが、型がtexture2Dもしくはtextureではありません。");
                            }
                            text3 = "texture2d";
                        }
                    }
                    else
                    {
                        if (!text.Equals("texturecube"))
                        {
                            throw new InvalidMMEEffectShaderException("ResourceTypeにはCubeが指定されていますが、型がtextureCUBEではありません。");
                        }
                        text3 = "texturecube";
                    }
                    goto ILIKEVOCALOIDS;
                }
                ILOVECODING:
                throw new InvalidMMEEffectShaderException("認識できないResourceTypeが指定されました。");
            }
            text3 = text;
            ILIKEVOCALOIDS:
            if (annotation2 != null)
            {
                string @string = annotation2.AsString().GetString();
                ImageLoadInformation imageLoadInformation = default(ImageLoadInformation);
                string text2 = text3;
                if (text2 != null)
                {
                    if (!(text2 == "texture2d"))
                    {
                        if (!(text2 == "texture3d"))
                        {
                            if (!(text2 == "texturecube"))
                            {
                            }
                        }
                        else
                        {
                            imageLoadInformation.Width = width;
                            imageLoadInformation.Height = height;
                            imageLoadInformation.Depth = depth;
                            imageLoadInformation.MipLevels = mipLevels;
                            imageLoadInformation.Format = format;
                            imageLoadInformation.Usage = ResourceUsage.Default;
                            imageLoadInformation.BindFlags = BindFlags.ShaderResource;
                            imageLoadInformation.CpuAccessFlags = CpuAccessFlags.None;
                            System.IO.Stream subresourceByName = effectManager.SubresourceLoader.getSubresourceByName(@string);
                            if (subresourceByName != null)
                            {
                                textureSubscriber.resourceTexture = Texture3D.FromStream(context.DeviceManager.Device, subresourceByName, (int)subresourceByName.Length);
                            }
                        }
                    }
                    else
                    {
                        imageLoadInformation.Width = width;
                        imageLoadInformation.Height = height;
                        imageLoadInformation.MipLevels = mipLevels;
                        imageLoadInformation.Format = format;
                        imageLoadInformation.Usage = ResourceUsage.Default;
                        imageLoadInformation.BindFlags = BindFlags.ShaderResource;
                        imageLoadInformation.CpuAccessFlags = CpuAccessFlags.None;
                        System.IO.Stream subresourceByName = effectManager.SubresourceLoader.getSubresourceByName(@string);
                        if (subresourceByName != null)
                        {
                            textureSubscriber.resourceTexture = Texture2D.FromStream(context.DeviceManager.Device, subresourceByName, (int)subresourceByName.Length);
                        }
                    }
                }
            }
            textureSubscriber.resourceView = new ShaderResourceView(context.DeviceManager.Device, textureSubscriber.resourceTexture);
            return textureSubscriber;
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            subscribeTo.AsResource().SetResource(resourceView);
        }

        public void Dispose()
        {
            if (resourceView != null && !resourceView.Disposed)
            {
                resourceView.Dispose();
                resourceView = null;
            }
        }
    }
}