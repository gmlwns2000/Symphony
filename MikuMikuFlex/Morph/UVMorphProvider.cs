using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.MorphOffset;
using MMF.Model;
using MMF.Model.PMX;
using MMF.Motion;
using SlimDX;

namespace MMF.Morph
{
    public class UVMorphProvider : IMorphProvider
    {
        public System.Collections.Generic.Dictionary<string, UVMorphData> Morphs = new System.Collections.Generic.Dictionary<string, UVMorphData>();

        private ModelData model;

        private IBufferManager bufferManager;

        private MorphType targetMorph;

        public UVMorphProvider(PMXModel model, MorphType targetType)
        {
            bufferManager = model.BufferManager;
            targetMorph = targetType;
            this.model = model.Model;
            if (model.Model.Header.AdditionalUVCount + MorphType.UV_Additional1 > targetType)
            {
                foreach (MorphData current in model.Model.MorphList.Morphes)
                {
                    if (current.type == targetMorph)
                    {
                        Morphs.Add(current.MorphName, new UVMorphData(current));
                    }
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
        }

        private bool SetMorphProgress(float progress, string morphName)
        {
            bool result;
            if (!Morphs.ContainsKey(morphName))
            {
                result = false;
            }
            else
            {
                UVMorphData uVMorphData = Morphs[morphName];
                foreach (UVMorphOffset current in uVMorphData.MorphOffsets)
                {
                    switch (targetMorph)
                    {
                        case MorphType.UV:
                            bufferManager.InputVerticies[(int)((System.UIntPtr)current.VertexIndex)].UV = model.VertexList.Vertexes[(int)((System.UIntPtr)current.VertexIndex)].UV + new Vector2(current.UVOffset.X, current.UVOffset.Y) * progress;
                            break;
                        case MorphType.UV_Additional1:
                            bufferManager.InputVerticies[(int)((System.UIntPtr)current.VertexIndex)].AddUV1 = model.VertexList.Vertexes[(int)((System.UIntPtr)current.VertexIndex)].AdditionalUV[0] + current.UVOffset * progress;
                            break;
                        case MorphType.UV_Additional2:
                            bufferManager.InputVerticies[(int)((System.UIntPtr)current.VertexIndex)].AddUV2 = model.VertexList.Vertexes[(int)((System.UIntPtr)current.VertexIndex)].AdditionalUV[1] + current.UVOffset * progress;
                            break;
                        case MorphType.UV_Additional3:
                            bufferManager.InputVerticies[(int)((System.UIntPtr)current.VertexIndex)].AddUV3 = model.VertexList.Vertexes[(int)((System.UIntPtr)current.VertexIndex)].AdditionalUV[2] + current.UVOffset * progress;
                            break;
                        case MorphType.UV_Additional4:
                            bufferManager.InputVerticies[(int)((System.UIntPtr)current.VertexIndex)].AddUV4 = model.VertexList.Vertexes[(int)((System.UIntPtr)current.VertexIndex)].AdditionalUV[3] + current.UVOffset * progress;
                            break;
                        default:
                            throw new System.ArgumentOutOfRangeException("不適切なモーフタイプが渡されました");
                    }
                }
                result = true;
            }
            return result;
        }
    }
}