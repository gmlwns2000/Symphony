namespace MMF.Model.Assimp
{
    [System.Flags]
    public enum AssimpFileFilter
    {
        AllFile = 15,
        CommonModelFile = 1,
        CommonGameEngineFile = 2,
        CommonGameFile = 4,
        OtherFile = 8
    }
}
