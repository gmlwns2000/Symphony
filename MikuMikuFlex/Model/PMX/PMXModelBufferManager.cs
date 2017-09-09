using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.BoneWeight;
using MMF.Utility;
using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.Model.PMX
{
    public class PMXModelBufferManager : IBufferManager, System.IDisposable
    {
        private DataBox vertexDataBox;

        public Buffer VertexBuffer
        {
            get;
            private set;
        }

        public Buffer IndexBuffer
        {
            get;
            private set;
        }

        public InputLayout VertexLayout
        {
            get;
            set;
        }

        public int VerticiesCount
        {
            get
            {
                return InputVerticies.Length;
            }
        }

        public BasicInputLayout[] InputVerticies
        {
            get;
            private set;
        }

        public Device Device
        {
            get;
            set;
        }

        public bool NeedReset
        {
            get;
            set;
        }

        public void Dispose()
        {
            if (VertexBuffer != null && !VertexBuffer.Disposed)
            {
                VertexBuffer.Dispose();
            }
            if (IndexBuffer != null && !IndexBuffer.Disposed)
            {
                IndexBuffer.Dispose();
            }
            if (VertexLayout != null && !VertexLayout.Disposed)
            {
                VertexLayout.Dispose();
            }
            if (InputVerticies != null)
            {
                InputVerticies = null;
            }
        }

        public void Initialize(object model, Device device, Effect effect)
        {
            Device = device;
            InitializeBuffer(model, device);
            VertexLayout = new InputLayout(device, effect.GetTechniqueByIndex(1).GetPassByIndex(0).Description.Signature, BasicInputLayout.VertexElements);
        }

        private void InitializeBuffer(object model, Device device)
        {
            ModelData modelData = (ModelData)model;
            System.Collections.Generic.List<BasicInputLayout> list = new System.Collections.Generic.List<BasicInputLayout>();
            System.Collections.Generic.List<uint> list2 = new System.Collections.Generic.List<uint>();
            for (int i = 0; i < modelData.VertexList.VertexCount; i++)
            {
                LoadVertex(modelData.VertexList.Vertexes[i], list);
            }
            vertexDataBox = new DataBox(0, 0, new DataStream(list.ToArray(), true, true));
            VertexBuffer = CGHelper.CreateBuffer(device, list.Count * BasicInputLayout.SizeInBytes, BindFlags.VertexBuffer);
            device.ImmediateContext.UpdateSubresource(vertexDataBox, VertexBuffer, 0);
            InputVerticies = list.ToArray();
            list.Clear();
            foreach (SurfaceData current in modelData.SurfaceList.Surfaces)
            {
                list2.Add(current.p);
                list2.Add(current.q);
                list2.Add(current.r);
            }
            IndexBuffer = CGHelper.CreateBuffer<uint>(list2, device, BindFlags.IndexBuffer);
        }

        private void LoadVertex(VertexData vertexData, System.Collections.Generic.List<BasicInputLayout> verticies)
        {
            BasicInputLayout item = default(BasicInputLayout);
            item.Position = new Vector4(vertexData.Position, 1f);
            item.Normal = vertexData.Normal;
            item.UV = vertexData.UV;
            item.Index = (uint)verticies.Count;
            if (vertexData.BoneWeight is BDEF1)
            {
                BDEF1 bDEF = (BDEF1)vertexData.BoneWeight;
                item.BoneIndex1 = (uint)bDEF.boneIndex;
                item.BoneWeight1 = 1f;
            }
            else if (vertexData.BoneWeight is BDEF2)
            {
                BDEF2 bDEF2 = (BDEF2)vertexData.BoneWeight;
                item.BoneIndex1 = (uint)bDEF2.Bone1ReferenceIndex;
                item.BoneIndex2 = (uint)bDEF2.Bone2ReferenceIndex;
                item.BoneWeight1 = bDEF2.Weight;
                item.BoneWeight2 = 1f - bDEF2.Weight;
            }
            else if (vertexData.BoneWeight is SDEF)
            {
                SDEF sDEF = (SDEF)vertexData.BoneWeight;
                item.BoneIndex1 = (uint)sDEF.Bone1ReferenceIndex;
                item.BoneIndex2 = (uint)sDEF.Bone2ReferenceIndex;
                item.BoneWeight1 = sDEF.Bone1Weight;
                item.BoneWeight2 = 1f - sDEF.Bone1Weight;
                item.Sdef_C = new Vector4(sDEF.SDEF_C, 1f);
                item.SdefR0 = sDEF.SDEF_R0;
                item.SdefR1 = sDEF.SDEF_R1;
            }
            else
            {
                BDEF4 bDEF3 = (BDEF4)vertexData.BoneWeight;
                float num = bDEF3.Weights.X + bDEF3.Weights.X + bDEF3.Weights.Z + bDEF3.Weights.W;
                item.BoneIndex1 = (uint)bDEF3.Bone1ReferenceIndex;
                item.BoneIndex2 = (uint)bDEF3.Bone2ReferenceIndex;
                item.BoneIndex3 = (uint)bDEF3.Bone3ReferenceIndex;
                item.BoneIndex4 = (uint)bDEF3.Bone4ReferenceIndex;
                item.BoneWeight1 = bDEF3.Weights.X / num;
                item.BoneWeight2 = bDEF3.Weights.Y / num;
                item.BoneWeight3 = bDEF3.Weights.Z / num;
                item.BoneWeight4 = bDEF3.Weights.W / num;
            }
            verticies.Add(item);
        }

        public void RecreateVerticies()
        {
            if (NeedReset)
            {
                vertexDataBox.Data.WriteRange<BasicInputLayout>(InputVerticies);
                vertexDataBox.Data.Position = 0L;
                Device.ImmediateContext.UpdateSubresource(vertexDataBox, VertexBuffer, 0);
                NeedReset = false;
            }
        }
    }
}