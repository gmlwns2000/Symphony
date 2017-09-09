using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphoy.Installer.SIS
{
    public class ParsedLine
    {
        public string[] Args;

        public ParsedLine(string line)
        {
            StringBuilder b = new StringBuilder();

            List<string> args = new List<string>();
            bool text = false;
            bool escape = false;
            foreach(char s in line)
            {
                if (text)
                {
                    if (s == '\"' && !escape)
                    {
                        text = false;
                        string ts = b.ToString();
                        if (!string.IsNullOrEmpty(ts))
                        {
                            args.Add(b.ToString());
                        }

                        b.Clear();
                    }
                    else
                    {
                        if (s == '\\')
                        {
                            escape = true;
                        }
                        else
                        {
                            escape = false;
                            b.Append(s);
                        }
                    }
                }
                else
                {
                    if(s != ' ')
                    {
                        if(s == '\"' && !escape)
                        {
                            text = true;
                        }
                        else
                        {
                            if(s == '\\')
                            {
                                escape = true;
                            }
                            else
                            {
                                escape = false;
                                b.Append(s);
                            }
                        }
                    }
                    else
                    {
                        string ts = b.ToString();
                        if (!string.IsNullOrEmpty(ts))
                        {
                            args.Add(b.ToString());
                        }
                        b.Clear();
                    }
                }
            }

            Args = args.ToArray();
        }
    }
}
