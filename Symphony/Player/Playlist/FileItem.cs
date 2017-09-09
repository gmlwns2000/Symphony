using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Symphony.Player
{
    public class FileItem : PlaylistItem
    {
        public FileItem(string filepath)
        {
            FilePath = filepath;
            FileName = Path.GetFileName(filepath);
            
            Util.LimitedTaskScheduler.Factory.StartNew( delegate {
                Tag = new Tags(filepath);

                InvokeUpdated(this, this);
            });
        }

        public override Stream GetStream()
        {
            return File.OpenRead(FilePath);
        }
    }
}
