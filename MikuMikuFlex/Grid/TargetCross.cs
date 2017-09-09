using MMF.Model;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace MMF.Grid
{
    public class TargetCross : IDrawable, System.IDisposable
    {
        private const int AxisLength = 3;

        private InputLayout axisLayout;

        private int axisVectorCount;

        private Buffer axisVertexBuffer;

        private Effect effect;

        public bool IsVisibleAxisGrid
        {
            get;
            set;
        }

        private RenderContext RenderContext
        {
            get;
            set;
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

        public bool Visibility
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public int SubsetCount
        {
            get;
            private set;
        }

        public TargetCross()
        {
            Transformer = new BasicTransformer();
        }

        public void Draw()
        {
            DeviceContext immediateContext = RenderContext.DeviceManager.Device.ImmediateContext;
            immediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
            effect.GetVariableBySemantic("WORLDVIEWPROJECTION").AsMatrix().SetMatrix(RenderContext.MatrixManager.makeWorldViewProjectionMatrix(Transformer.Scale, Transformer.Rotation, Transformer.Position));
            if (IsVisibleAxisGrid)
            {
                immediateContext.InputAssembler.InputLayout = axisLayout;
                immediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(axisVertexBuffer, AxisGridLayout.SizeInBytes, 0));
                effect.GetTechniqueByIndex(0).GetPassByIndex(1).Apply(immediateContext);
                immediateContext.Draw(7 * axisVectorCount, 0);
            }
        }

        public void Dispose()
        {
            axisVertexBuffer.Dispose();
            axisLayout.Dispose();
            effect.Dispose();
        }

        public void Update()
        {
        }

        public void Load(RenderContext renderContext)
        {
            IsVisibleAxisGrid = true;
            RenderContext = renderContext;
            MakeGridVectors();
            SubsetCount = 1;
        }

        public void GetFileName()
        {
        }

        private void MakeGridVectors()
        {
            using (ShaderBytecode shaderBytecode = ShaderBytecode.CompileFromFile("Shader\\grid.fx", "fx_5_0"))
            {
                effect = new Effect(RenderContext.DeviceManager.Device, shaderBytecode);
            }
            System.Collections.Generic.List<float> list = new System.Collections.Generic.List<float>();
            TargetCross.AddAxisVector(list, 3f, 0f, 0f, new Vector4(1f, 0f, 0f, 1f));
            TargetCross.AddAxisVector(list, 0f, 3f, 0f, new Vector4(0f, 1f, 0f, 1f));
            TargetCross.AddAxisVector(list, 0f, 0f, 3f, new Vector4(0f, 0f, 1f, 1f));
            using (DataStream dataStream = new DataStream(list.ToArray(), true, true))
            {
                BufferDescription description = new BufferDescription
                {
                    BindFlags = BindFlags.VertexBuffer,
                    SizeInBytes = (int)dataStream.Length
                };
                axisVertexBuffer = new Buffer(RenderContext.DeviceManager.Device, dataStream, description);
            }
            axisVectorCount = list.Count;
            ShaderSignature signature = effect.GetTechniqueByIndex(0).GetPassByIndex(1).Description.Signature;
            axisLayout = new InputLayout(RenderContext.DeviceManager.Device, signature, AxisGridLayout.VertexElements);
            VertexCount = axisVectorCount;
        }

        private static void AddAxisVector(System.Collections.Generic.List<float> vertexList, float x, float y, float z, Vector4 color)
        {
            vertexList.Add(x);
            vertexList.Add(y);
            vertexList.Add(z);
            vertexList.Add(color.X);
            vertexList.Add(color.Y);
            vertexList.Add(color.Z);
            vertexList.Add(color.W);
            vertexList.Add(-x);
            vertexList.Add(-y);
            vertexList.Add(-z);
            vertexList.Add(color.X);
            vertexList.Add(color.Y);
            vertexList.Add(color.Z);
            vertexList.Add(color.W);
        }
    }
}
