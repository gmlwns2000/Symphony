using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.UI
{
    public interface IObjectEditor
    {
        event EventHandler<ObjectChangedArgs> ObjectChanged;
    }

    public class ObjectChangedArgs : EventArgs
    {
        public object Object { get; set; }

        public ObjectChangedArgs(object obj)
        {
            Object = obj;
        }
    }

    public class ObjectChangedArgs<T> : EventArgs
    {
        public T Object { get; set; }

        public ObjectChangedArgs(T obj)
        {
            Object = obj;
        }
    }
}
