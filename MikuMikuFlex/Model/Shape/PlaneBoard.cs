using MMF.Sprite;
using MMF.Utility;
using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.Model.Shape
{
    public class PlaneBoard : IDrawable, System.IDisposable
    {
        public Buffer VertexBuffer;

        private EffectPass renderPass;

        private RenderContext context;

        private readonly ShaderResourceView _resView;

        private InputLayout VertexInputLayout;

        private Effect SpriteEffect
        {
            get;
            set;
        }

        public bool Visibility
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            private set;
        }

        public int SubsetCount
        {
            get;
            private set;
        }

        public int VertexCount
        {
            get;
            private set;
        }

        public ITransformer Transformer
        {
            get;
            private set;
        }

        public Vector4 SelfShadowColor
        {
            get;
            set;
        }

        public Vector4 GroundShadowColor
        {
            get;
            set;
        }

        public PlaneBoard(RenderContext context, ShaderResourceView resView) : this(context, resView, new Vector2(200f, 200f))
        {
        }

        public PlaneBoard(RenderContext context, ShaderResourceView resView, Vector2 size)
        {
            this.context = context;
            _resView = resView;
            Visibility = true;
            SpriteEffect = CGHelper.CreateEffectFx5FromResource("MMF.Resource.Shader.SpriteShader.fx", context.DeviceManager.Device);
            VertexInputLayout = new InputLayout(context.DeviceManager.Device, SpriteEffect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature, SpriteVertexLayout.InputElements);
            renderPass = SpriteEffect.GetTechniqueByIndex(0).GetPassByIndex(0);
            float num = size.X / 2f;
            float num2 = size.Y / 2f;
            System.Collections.Generic.List<byte> list = new System.Collections.Generic.List<byte>();
            CGHelper.AddListBuffer(new Vector3(-num, num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(0f, 0f), list);
            CGHelper.AddListBuffer(new Vector3(num, num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(1f, 0f), list);
            CGHelper.AddListBuffer(new Vector3(-num, -num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(0f, 1f), list);
            CGHelper.AddListBuffer(new Vector3(num, num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(1f, 0f), list);
            CGHelper.AddListBuffer(new Vector3(num, -num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(1f, 1f), list);
            CGHelper.AddListBuffer(new Vector3(-num, -num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(0f, 1f), list);
            using (DataStream dataStream = new DataStream(list.ToArray(), true, true))
            {
                BufferDescription description = new BufferDescription
                {
                    BindFlags = BindFlags.VertexBuffer,
                    SizeInBytes = (int)dataStream.Length
                };
                VertexBuffer = new Buffer(context.DeviceManager.Device, dataStream, description);
            }
            Transformer = new BasicTransformer();
            Transformer.Scale = new Vector3(0.2f);
        }

        public void Dispose()
        {
            if (VertexBuffer != null && !VertexBuffer.Disposed)
            {
                VertexBuffer.Dispose();
            }
            if (VertexInputLayout != null && !VertexInputLayout.Disposed)
            {
                VertexInputLayout.Dispose();
            }
        }

        public void Draw()
        {
            SpriteEffect.GetVariableBySemantic("WORLDVIEWPROJECTION").AsMatrix().SetMatrix(context.MatrixManager.makeWorldViewProjectionMatrix(this));
            SpriteEffect.GetVariableBySemantic("SPRITETEXTURE").AsResource().SetResource(_resView);
            context.DeviceManager.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, SpriteVertexLayout.SizeInBytes, 0));
            context.DeviceManager.Context.InputAssembler.InputLayout = VertexInputLayout;
            context.DeviceManager.Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderPass.Apply(context.DeviceManager.Context);
            context.DeviceManager.Context.Draw(12, 0);
        }

        public void Update()
        {
        }
    }
}