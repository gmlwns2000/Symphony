using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Symphony.UI.Settings
{
    public class LuaCompletionData : ICompletionData
    {
        public object Content
        {
            get
            {
                return Text;
            }
        }

        public string _desc = "";
        public object Description
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = (string)value;
            }
        }

        public ImageSource Image
        {
            get
            {
                return null;
            }
        }

        public double Priority
        {
            get
            {
                return 0;
            }
        }

        public string Text
        {
            get; set;
        }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }

        public LuaCompletionData(string text, string description)
        {
            Text = text;

            if (!Util.TextTool.StringEmpty(description))
            {
                Description = description;
            }
            else
            {
                Description = text;
            }
        }
    }
}
