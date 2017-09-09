using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Lyrics
{
    public class ValueKeyframeCollection : IEnumerable<ValueKeyframe>
    {
        List<ValueKeyframe> List = new List<ValueKeyframe>();

        public int Count
        {
            get
            {
                return List.Count;
            }
        }
        
        public int Capacity
        {
            get
            {
                return List.Capacity;
            }
        }
        
        public int Add(ValueKeyframe keyframe)
        {
            if(List.Count > 0)
            {
                for (int i = 0; i < List.Count; i++)
                {
                    if (List[i].Time > keyframe.Time)
                    {
                        List.Insert(i, keyframe);
                        return i;
                    }
                }

                List.Add(keyframe);

                return List.Count - 1;
            }
            else
            {
                List.Add(keyframe);

                return List.Count - 1;
            }
        }

        public void Solt()
        {
            if(List.Count <= 1)
            {
                return;
            }

            bool solting = false;
            bool needcheck = false;
            int index = 0;
            while (solting)
            {
                if(index == List.Count - 1)
                {
                    if (needcheck)
                    {
                        needcheck = false;
                        index = 0;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if(List[index].Time > List[index + 1].Time)
                    {
                        ValueKeyframe tmp = List[index];
                        List[index] = List[index + 1];
                        List[index + 1] = tmp;

                        needcheck = true;
                    }
                }
            }
        }

        public void Remove(int index)
        {
            List.RemoveAt(index);
        }

        public void Remove(ValueKeyframe keyframe)
        {
            List.Remove(keyframe);

            Solt();
        }

        public ValueKeyframe this[int index]
        {
            get
            {
                return (List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        public List<string> GetTargets()
        {
            if (List.Count > 0)
            {
                List<string> targetList = new List<string>();
                foreach(ValueKeyframe kf in List)
                {
                    bool exist = false;
                    foreach(string t in targetList)
                    {
                        if (t == kf.Target)
                            exist = true;
                    }
                    if (!exist)
                        targetList.Add(kf.Target);
                }

                return targetList;
            }
            else
            {
                return null;
            }
        }

        public IEnumerator<ValueKeyframe> GetEnumerator()
        {
            return ((IEnumerable<ValueKeyframe>)List).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ValueKeyframe>)List).GetEnumerator();
        }
    }
}
