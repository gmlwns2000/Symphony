using Assimp;
using MMF.MME.VariableSubscriber.MaterialSubscriber;
using MMF.Utility;
using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.Model.Assimp
{
    public class AssimpSubset : ISubset, System.IDisposable
    {
        private Mesh mesh;

        private Buffer vBuffer;

        private RenderContext context;

        public MaterialInfo MaterialInfo
        {
            get;
            private set;
        }

        public int SubsetId
        {
            get;
            private set;
        }

        public IDrawable Drawable
        {
            get;
            set;
        }

        public bool DoCulling
        {
            get;
            private set;
        }

        public AssimpSubset(RenderContext context, ISubresourceLoader loader, IDrawable drawable, Scene scene, int index)
        {
            SubsetId = index;
            this.context = context;
            DoCulling = false;
            Drawable = drawable;
            Material material = scene.Materials[scene.Meshes[index].MaterialIndex];
            mesh = scene.Meshes[index];
            Initialize();
            MaterialInfo = MaterialInfo.FromMaterialData(drawable, material, context, loader);
        }

        private void Initialize()
        {
            System.Collections.Generic.List<BasicInputLayout> list = new System.Collections.Generic.List<BasicInputLayout>();
            Face[] faces = mesh.Faces;
            for (int i = 0; i < faces.Length; i++)
            {
                Face face = faces[i];
                uint[] indices = face.Indices;
                for (int j = 0; j < indices.Length; j++)
                {
                    int num = (int)indices[j];
                    BasicInputLayout item = default(BasicInputLayout);
                    item.Position = mesh.Vertices[num].ToSlimDXVec4(1f).InvX();
                    item.Normal = mesh.Normals[num].ToSlimDX();
                    if (mesh.GetTextureCoords(0) != null)
                    {
                        Vector3 vector = mesh.GetTextureCoords(0)[num].ToSlimDX();
                        item.UV = new Vector2(vector.X, 1f - vector.Y);
                    }
                    item.BoneWeight1 = 1f;
                    item.BoneIndex1 = 0u;
                    list.Add(item);
                }
            }
            vBuffer = CGHelper.CreateBuffer<BasicInputLayout>(list, context.DeviceManager.Device, BindFlags.VertexBuffer);
        }

        public void Draw(Device device)
        {
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vBuffer, BasicInputLayout.SizeInBytes, 0));
            device.ImmediateContext.Draw(mesh.FaceCount * 3, 0);
        }

        public void Dispose()
        {
            if (vBuffer != null && !vBuffer.Disposed)
            {
                vBuffer.Dispose();
            }
            MaterialInfo.Dispose();
        }
    }
}