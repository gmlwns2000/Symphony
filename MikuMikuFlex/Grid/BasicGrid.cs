using MMF.Model;
using MMF.Utility;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace MMF.Grid
{
    public class BasicGrid : IDrawable, System.IDisposable
    {
        private const int GridLength = 100;

        private const int GridWidth = 10;

        private const int GridCount = 20;

        private const int AxisLength = 300;

        private InputLayout axisLayout;

        private int axisVectorCount;

        private Buffer axisVertexBuffer;

        private Effect effect;

        private InputLayout layout;

        private int vectorCount;

        private Buffer vertexBuffer;

        private bool _isVisibleMeasureGrid;

        private bool _isVisibleAxisGrid;

        public bool IsVisibleMeasureGrid
        {
            get
            {
                return _isVisibleMeasureGrid;
            }
            set
            {
                _isVisibleMeasureGrid = value;
            }
        }

        public bool IsVisibleAxisGrid
        {
            get
            {
                return _isVisibleAxisGrid;
            }
            set
            {
                _isVisibleAxisGrid = value;
            }
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

        public BasicGrid()
        {
            Transformer = new BasicTransformer();
            Visibility = true;
            FileName = "@MMF.CG.Model.Grid.BasicGrid@";
        }

        public void Update()
        {
        }

        public void Draw()
        {
            DeviceContext immediateContext = RenderContext.DeviceManager.Device.ImmediateContext;
            immediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
            effect.GetVariableBySemantic("WORLDVIEWPROJECTION").AsMatrix().SetMatrix(RenderContext.MatrixManager.makeWorldViewProjectionMatrix(this));
            if (IsVisibleMeasureGrid)
            {
                immediateContext.InputAssembler.InputLayout = layout;
                immediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, MeasureGridLayout.SizeInBytes, 0));
                effect.GetTechniqueByIndex(0).GetPassByIndex(0).Apply(immediateContext);
                immediateContext.Draw(4 * vectorCount, 0);
            }
            if (IsVisibleAxisGrid)
            {
                immediateContext.InputAssembler.InputLayout = axisLayout;
                immediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(axisVertexBuffer, AxisGridLayout.SizeInBytes, 0));
                effect.GetTechniqueByIndex(0).GetPassByIndex(1).Apply(immediateContext);
                immediateContext.Draw(7 * axisVectorCount, 0);
            }
        }

        public void GetFileName()
        {
        }

        public void Dispose()
        {
            axisVertexBuffer.Dispose();
            vertexBuffer.Dispose();
            layout.Dispose();
            axisLayout.Dispose();
            effect.Dispose();
        }

        public void Load(RenderContext renderContext)
        {
            IsVisibleAxisGrid = true;
            IsVisibleMeasureGrid = true;
            RenderContext = renderContext;
            MakeGridVectors();
            SubsetCount = 1;
        }

        private void MakeGridVectors()
        {
            System.Collections.Generic.List<Vector3> list = new System.Collections.Generic.List<Vector3>();
            effect = CGHelper.CreateEffectFx5FromResource("MMF.Resource.Shader.GridShader.fx", RenderContext.DeviceManager.Device);
            for (int i = 0; i <= 20; i++)
            {
                if (i != 10)
                {
                    list.Add(new Vector3(-100f, 0f, -100 + i * 10));
                    list.Add(new Vector3(100f, 0f, -100 + i * 10));
                }
            }
            for (int i = 0; i <= 20; i++)
            {
                if (i != 10)
                {
                    list.Add(new Vector3(-100 + i * 10, 0f, -100f));
                    list.Add(new Vector3(-100 + i * 10, 0f, 100f));
                }
            }
            using (DataStream dataStream = new DataStream(list.ToArray(), true, true))
            {
                BufferDescription description = new BufferDescription
                {
                    BindFlags = BindFlags.VertexBuffer,
                    SizeInBytes = (int)dataStream.Length
                };
                vertexBuffer = new Buffer(RenderContext.DeviceManager.Device, dataStream, description);
            }
            vectorCount = list.Count;
            System.Collections.Generic.List<float> list2 = new System.Collections.Generic.List<float>();
            BasicGrid.addAxisVector(list2, 300f, 0f, 0f, new Vector4(1f, 0f, 0f, 1f));
            BasicGrid.addAxisVector(list2, 0f, 300f, 0f, new Vector4(0f, 1f, 0f, 1f));
            BasicGrid.addAxisVector(list2, 0f, 0f, 300f, new Vector4(0f, 0f, 1f, 1f));
            using (DataStream dataStream = new DataStream(list2.ToArray(), true, true))
            {
                BufferDescription description = new BufferDescription
                {
                    BindFlags = BindFlags.VertexBuffer,
                    SizeInBytes = (int)dataStream.Length
                };
                axisVertexBuffer = new Buffer(RenderContext.DeviceManager.Device, dataStream, description);
            }
            axisVectorCount = list2.Count;
            ShaderSignature signature = effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature;
            layout = new InputLayout(RenderContext.DeviceManager.Device, signature, MeasureGridLayout.VertexElements);
            signature = effect.GetTechniqueByIndex(0).GetPassByIndex(1).Description.Signature;
            axisLayout = new InputLayout(RenderContext.DeviceManager.Device, signature, AxisGridLayout.VertexElements);
            VertexCount = axisVectorCount + vectorCount;
        }

        private static void addAxisVector(System.Collections.Generic.List<float> vertexList, float x, float y, float z, Vector4 color)
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
