using MMF.Utility;
using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.Model.Other
{
    public class DebugDotManager
    {
        public const float dotlength = 0.8f;

        public Buffer VertexBuffer
        {
            get;
            private set;
        }

        public InputLayout VertexLayout
        {
            get;
            private set;
        }

        public Effect Effect
        {
            get;
            private set;
        }

        public EffectPass RenderPass
        {
            get;
            set;
        }

        public RenderContext Context
        {
            get;
            set;
        }

        public DebugDotManager(RenderContext context)
        {
            Context = context;
            System.Collections.Generic.List<byte> list = new System.Collections.Generic.List<byte>();
            CGHelper.AddListBuffer(new Vector3(-0.4f, 0.4f, 0f), list);
            CGHelper.AddListBuffer(new Vector3(0.4f, 0.4f, 0f), list);
            CGHelper.AddListBuffer(new Vector3(-0.4f, -0.4f, 0f), list);
            CGHelper.AddListBuffer(new Vector3(0.4f, 0.4f, 0f), list);
            CGHelper.AddListBuffer(new Vector3(0.4f, -0.4f, 0f), list);
            CGHelper.AddListBuffer(new Vector3(-0.4f, -0.4f, 0f), list);
            VertexBuffer = CGHelper.CreateBuffer(list, context.DeviceManager.Device, BindFlags.VertexBuffer);
            Effect = CGHelper.CreateEffectFx5("Shader\\debugDot.fx", context.DeviceManager.Device);
            RenderPass = Effect.GetTechniqueByIndex(0).GetPassByIndex(0);
            VertexLayout = new InputLayout(context.DeviceManager.Device, Effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature, DebugDotInputLayout.InputElements);
        }

        public void Draw(System.Collections.Generic.List<Vector3> positions, Vector4 color)
        {
            if (positions != null)
            {
                Effect.GetVariableBySemantic("COLOR").AsVector().Set(color);
                for (int i = 0; i < positions.Count; i++)
                {
                    Vector3 vector = positions[i];
                    Vector3 right = Vector3.Normalize(Context.MatrixManager.ViewMatrixManager.CameraPosition - vector);
                    Vector3 axis = Vector3.Cross(new Vector3(0f, 0f, -1f), right);
                    float angle = (float)System.Math.Acos(Vector3.Dot(new Vector3(0f, 0f, -1f), right));
                    Quaternion localRotation = Quaternion.RotationAxis(axis, angle);
                    DeviceContext context = Context.DeviceManager.Context;
                    Effect.GetVariableBySemantic("WORLDVIEWPROJECTION").AsMatrix().SetMatrix(Context.MatrixManager.makeWorldViewProjectionMatrix(new Vector3(1f), localRotation, vector));
                    context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, DebugDotInputLayout.SizeInBytes, 0));
                    context.InputAssembler.InputLayout = VertexLayout;
                    context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    RenderPass.Apply(context);
                    context.Draw(6, 0);
                }
            }
        }
    }
}