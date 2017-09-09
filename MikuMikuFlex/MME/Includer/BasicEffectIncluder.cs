using SlimDX.D3DCompiler;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace MMF.MME.Includer
{
    public class BasicEffectIncluder : Include, System.Collections.Generic.IComparer<IncludeDirectory>
    {
        public ObservableCollection<IncludeDirectory> IncludeDirectories
        {
            get;
            private set;
        }

        public BasicEffectIncluder()
        {
            IncludeDirectories = new ObservableCollection<IncludeDirectory>();
            IncludeDirectories.CollectionChanged += new NotifyCollectionChangedEventHandler(IncludeDirectories_CollectionChanged);
            IncludeDirectories.Add(new IncludeDirectory("Shader\\include", 0));
        }

        private void IncludeDirectories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            System.Collections.Generic.List<IncludeDirectory> list = IncludeDirectories.ToList<IncludeDirectory>();
            IncludeDirectories = new ObservableCollection<IncludeDirectory>(list);
        }

        public void Close(System.IO.Stream stream)
        {
            stream.Close();
        }

        public void Open(IncludeType type, string fileName, System.IO.Stream parentStream, out System.IO.Stream stream)
        {
            if (System.IO.Path.IsPathRooted(fileName))
            {
                stream = System.IO.File.OpenRead(fileName);
            }
            else
            {
                foreach (IncludeDirectory current in IncludeDirectories)
                {
                    if (System.IO.File.Exists(System.IO.Path.Combine(current.DirectoryPath, fileName)))
                    {
                        stream = System.IO.File.OpenRead(System.IO.Path.Combine(current.DirectoryPath, fileName));
                        return;
                    }
                }
                stream = null;
            }
        }

        public int Compare(IncludeDirectory x, IncludeDirectory y)
        {
            return x.Priorty - y.Priorty;
        }
    }
}
