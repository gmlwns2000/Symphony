using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.MorphOffset;
using MMF.Model;
using MMF.Motion;
using SlimDX;
using System.Collections.Generic;

namespace MMF.Morph
{
    public class VertexMorphProvider : IMorphProvider
    {
        public System.Collections.Generic.Dictionary<string, VertexMorphData> MorphList = new System.Collections.Generic.Dictionary<string, VertexMorphData>();

        private HashSet<uint> movedVertex = new HashSet<uint>();

        private ModelData model;

        private IBufferManager Buffermanager
        {
            get;
            set;
        }

        public VertexMorphProvider(ModelData model, IBufferManager bufManager)
        {
            this.model = model;
            Buffermanager = bufManager;
            foreach (MorphData current in model.MorphList.Morphes)
            {
                if (current.type == MorphType.Vertex)
                {
                    MorphList.Add(current.MorphName, new VertexMorphData(current));
                }
            }
        }

        public void ApplyMorphProgress(float frameNumber, System.Collections.Generic.IEnumerable<MorphMotion> morphMotions)
        {
            foreach (MorphMotion current in morphMotions)
            {
                SetMorphProgress(current.GetMorphValue(frameNumber), current.MorphName);
            }
        }

        public bool ApplyMorphProgress(float progress, string morphName)
        {
            return SetMorphProgress(progress, morphName);
        }

        public void UpdateFrame()
        {
            ResetVertexPosition();
        }

        private void ResetVertexPosition()
        {
            foreach (uint current in movedVertex)
            {
                VertexData vertexData = model.VertexList.Vertexes[(int)((System.UIntPtr)current)];
                Buffermanager.InputVerticies[(int)((System.UIntPtr)current)].Position = new Vector4(vertexData.Position, 1f);
            }
            movedVertex = new HashSet<uint>();
        }

        private bool SetMorphProgress(float progress, string morphName)
        {
            bool result;
            if (!MorphList.ContainsKey(morphName))
            {
                result = false;
            }
            else
            {
                VertexMorphData vertexMorphData = MorphList[morphName];
                foreach (VertexMorphOffset current in vertexMorphData.MorphOffsets)
                {
                    movedVertex.Add(current.VertexIndex);
                    BasicInputLayout[] expr_67_cp_0 = Buffermanager.InputVerticies;
                    System.UIntPtr expr_67_cp_1 = (System.UIntPtr)current.VertexIndex;
                    expr_67_cp_0[(int)expr_67_cp_1].Position = expr_67_cp_0[(int)expr_67_cp_1].Position + new Vector4(current.PositionOffset * progress, 0f);
                }
                Buffermanager.NeedReset = true;
                result = true;
            }
            return result;
        }
    }
}
