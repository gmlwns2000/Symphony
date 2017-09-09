using MMF.Utility;

namespace MMF.Model
{
    public class BasicSubresourceLoader : ISubresourceLoader
    {
        public string BaseDirectory
        {
            get;
            set;
        }

        public BasicSubresourceLoader(string baseDir)
        {
            if (string.IsNullOrEmpty(baseDir))
            {
                BaseDirectory = ".\\";
            }
            else
            {
                BaseDirectory = System.IO.Path.GetFullPath(baseDir);
            }
        }

        public System.IO.Stream getSubresourceByName(string name)
        {
            System.IO.Stream result;
            if (System.IO.Path.GetExtension(name).ToUpper().Equals(".TGA"))
            {
                if (string.IsNullOrEmpty(BaseDirectory))
                {
                    result = TargaSolver.LoadTargaImage(name, null);
                }
                else
                {
                    result = TargaSolver.LoadTargaImage(System.IO.Path.Combine(BaseDirectory, name), null);
                }
            }
            else if (string.IsNullOrEmpty(BaseDirectory))
            {
                result = System.IO.File.OpenRead(name);
            }
            else
            {
                string path = System.IO.Path.Combine(BaseDirectory, name);
                if (System.IO.File.Exists(path))
                {
                    result = System.IO.File.OpenRead(path);
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }
    }
}
