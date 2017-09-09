using System.Collections;
using System.Collections.Generic;
using DirectCanvas.Shapes;

namespace DirectCanvas.Transforms
{
    public sealed class TransformCollection: IList<GeneralTransform>
    {
        private List<GeneralTransform> m_innerList = new List<GeneralTransform>();

        internal TransformCollection()
        {
        }
        #region ListStuffs
        public int IndexOf(GeneralTransform item)
        {
            return m_innerList.IndexOf(item);
        }

        public void Insert(int index, GeneralTransform item)
        {
            m_innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            m_innerList.RemoveAt(index);
        }

        public GeneralTransform this[int index]
        {
            get { return m_innerList[index]; }
            set { m_innerList[index] = value; }
        }

        public void Add(GeneralTransform item)
        {
            m_innerList.Add(item);
        }

        public void Clear()
        {
            m_innerList.Clear();
        }

        public bool Contains(GeneralTransform item)
        {
            return m_innerList.Contains(item);
        }

        public void CopyTo(GeneralTransform[] array, int arrayIndex)
        {
            m_innerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return m_innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(GeneralTransform item)
        {
            return m_innerList.Remove(item);
        }

        public IEnumerator<GeneralTransform> GetEnumerator()
        {
            return m_innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_innerList.GetEnumerator();
        }
        #endregion
    }
}