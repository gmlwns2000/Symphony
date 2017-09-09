using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.MorphOffset;
using MMF.Model.PMX;
using MMF.Motion;
using System;

namespace MMF.Morph
{
    public class GroupMorphProvider : IMorphProvider
    {
        private IMorphManager morphManager;

        private System.Collections.Generic.Dictionary<int, string> morphNameList = new System.Collections.Generic.Dictionary<int, string>();

        public System.Collections.Generic.Dictionary<string, GroupMorphData> Morphs = new System.Collections.Generic.Dictionary<string, GroupMorphData>();

        public GroupMorphProvider(PMXModel model, IMorphManager morph)
        {
            morphManager = morph;
            int num = 0;
            foreach (MorphData current in model.Model.MorphList.Morphes)
            {
                if (current.type == MorphType.Group)
                {
                    Morphs.Add(current.MorphName, new GroupMorphData(current));
                }
                morphNameList.Add(num, current.MorphName);
                num++;
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
                GroupMorphData groupMorphData = Morphs[morphName];
                foreach (GroupMorphOffset current in groupMorphData.MorphOffsets)
                {
                    string text = morphNameList[current.MorphIndex];
                    if (morphName.Equals(text))
                    {
                        throw new InvalidOperationException("グループモーフに自身のモーフが指定されています。");
                    }
                    morphManager.ApplyMorphProgress(progress * current.MorphRatio, text);
                }
                result = true;
            }
            return result;
        }
    }
}