namespace MMF.MME.Script
{
    internal class FunctionDictionary : System.Collections.Generic.Dictionary<string, FunctionBase>
    {
        public void Add(FunctionBase item)
        {
            base.Add(item.FunctionName, item);
        }
    }
}
