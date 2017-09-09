using Symphony.Lyrics;
using Symphony.Server;
using Symphony.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Util
{
    public class Resource : IDisposable
    {
        public static int ResourceInstanceCount;
        public int ID { get; set; }
        public bool UseHashName { get; set; } = true;

        public string WorkingDirectory { get; set; }
        public bool IsExist
        {
            get
            {
                if (FilePath == null)
                {
                    return false;
                }
                else
                {
                    return File.Exists(FilePath);
                }
            }
        }
        public string FileName { get; private set; }
        public string FilePath
        {
            get
            {
                if(FileName == null || WorkingDirectory == null)
                {
                    return null;
                }

                return Path.Combine(WorkingDirectory, FileName);
            }
        }

        public Stream Stream { get; set; }

        public ResourcesParent Parent { get; private set; }

        ~Resource()
        {
            Dispose();
        }

        public Resource(string WorkingDirectory)
        {
            ID = ResourceInstanceCount;
            ResourceInstanceCount++;

            this.WorkingDirectory = WorkingDirectory;
        }

        public Resource(ResourcesParent Parent)
        {
            ID = ResourceInstanceCount;
            ResourceInstanceCount++;

            this.Parent = Parent;

            WorkingDirectory = Parent.ResourceDirectory;

            Parent.Resources.Add(this);
        }

        public void Open(string FileName)
        {
            CloseStream();

            this.FileName = FileName;
        }

        public QueryResult Import(string filePath)
        {
            //check exist
            if (!File.Exists(filePath))
            {
                return new QueryResult(null, "파일이 존재 하지않습니다.", false);
            }

            //dispose
            CloseStream();

            if(Parent != null)
            {
                if(Parent.GcWhenImport && IsExist)
                {
                    File.Delete(FilePath);
                }
            }
            else
            {
                if (IsExist)
                {
                    File.Delete(FilePath);
                }
            }

            //Hash name
            if (UseHashName)
            {
                string ext = Path.GetExtension(filePath);
                FileName = Symphony.Player.Crc32.GetFileCRC(filePath);
                FileName += ext;
            }
            else
            {
                FileName = Path.GetFileName(filePath);
            }

            //copy
            if (IsExist)
            {
                DialogMessageResult r = DialogMessage.Show(null, "이미 파일이 존제 합니다. 덮어씨우시겠습니까?", "확인", DialogMessageType.YesNo);
                if(r == DialogMessageResult.Yes)
                {
                    try
                    {
                        File.Delete(FilePath);
                    }
                    catch
                    {

                    }

                    File.Copy(filePath, FilePath);
                }
            }
            else
            {
                File.Copy(filePath, FilePath);
            }

            return new QueryResult(null, "import succ", true);
        }

        public Stream OpenStream()
        {
            CloseStream();

            if (FilePath != null)
            {
                Stream = File.OpenRead(FilePath);
                return Stream;
            }
            return null;
        }

        public void CloseStream()
        {
            if(Stream != null)
            {
                Stream.Close();
                Stream.Dispose();
                Stream = null;
            }
        }

        public void Remove()
        {
            CloseStream();

            if(IsExist)
                File.Delete(FilePath);

            if (Parent != null)
            {
                Parent.Resources.Remove(this);
            }
        }

        public void Dispose()
        {
            CloseStream();
        }
    }
}
