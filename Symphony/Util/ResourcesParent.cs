using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Util
{
    public class ResourcesParent : IDisposable
    {
        private string _resourceDirectory = "";
        public string ResourceDirectory
        {
            get
            {
                return _resourceDirectory;
            }
            set
            {
                _resourceDirectory = value;

                foreach (Resource r in Resources)
                {
                    r.WorkingDirectory = value;
                }
            }
        }
        public List<Resource> Resources { get; set; } = new List<Resource>();

        public bool GcWhenImport { get; set; } = true;

        ~ResourcesParent()
        {
            Dispose();
        }

        public void Dispose()
        {
            ResourceClose();
        }

        public void ResourceClose()
        {
            foreach(Resource r in Resources)
            {
                r.CloseStream();
            }
        }

        public void ResourceGarbageCollection()
        {
            string[] fis = Directory.GetFiles(ResourceDirectory);
            foreach(string file in fis)
            {
                bool require = false;
                foreach (Resource r in Resources)
                {
                    if(r.FilePath.ToLower() == file.ToLower())
                    {
                        require = true;
                    }
                }

                if (!require)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        Logger.Error("Error on Resource garbage Collection. file using : " + file);
                    }
                }
            }
        }
    }
}
