namespace MMF.Utility
{
    public interface HierarchicalOrderSolver<T>
    {
        int getParentIndex(T child);

        int getIndex(T target);
    }
}
