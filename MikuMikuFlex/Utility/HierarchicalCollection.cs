using System.Collections.Generic;

namespace MMF.Utility
{
    public class HierarchicalOrderCollection<T> : System.Collections.Generic.List<T>
    {
        public HierarchicalOrderCollection(T[] baseList, HierarchicalOrderSolver<T> solver)
        {
            Queue<int> queue = new Queue<int>();
            HashSet<int> hashSet = new HashSet<int>();
            hashSet.Add(-1);
            while (queue.Count != baseList.Length)
            {
                for (int i = 0; i < baseList.Length; i++)
                {
                    T t = baseList[i];
                    int index = solver.getIndex(t);
                    int parentIndex = solver.getParentIndex(t);
                    if (hashSet.Contains(parentIndex) && !hashSet.Contains(index))
                    {
                        queue.Enqueue(index);
                        hashSet.Add(index);
                    }
                }
            }
            while (queue.Count != 0)
            {
                Add(baseList[queue.Dequeue()]);
            }
        }
    }
}
