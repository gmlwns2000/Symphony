using System.IO;

namespace MMF.Model
{
    public interface ISubresourceLoader
    {
        Stream getSubresourceByName(string name);
    }
}