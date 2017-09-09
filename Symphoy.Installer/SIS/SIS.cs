using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Symphoy.Installer.SIS
{
    public class ProgressUpdated : EventArgs
    {
        public string Message;
        public double Value;
        public double Minimum;
        public double Maximum;

        public ProgressUpdated(string msg, double value, double min, double max)
        {
            Message = msg;
            Value = value;
            Minimum = min;
            Maximum = max;
        }
    }

    public class SIS
    {
        public event EventHandler<ProgressUpdated> Updated;
        public event EventHandler<Exception> ExceptionEXEC;

        List<ParsedLine> Lines = new List<ParsedLine>();
        string workingDirectory;

        public SIS(string sisFile, string workingDirectory)
        {
            string[] script = File.ReadAllLines(sisFile);
            foreach(string line in script)
            {
                if (!line.StartsWith("#"))
                {
                    Lines.Add(new ParsedLine(line));
                }
            }
            this.workingDirectory = workingDirectory;
        }

        public void Install(string installDirectory)
        {
            string msg = "";
            for(int i=0; i<Lines.Count; i++)
            {
                ParsedLine line = Lines[i];

                if(line.Args.Length > 0)
                {
                    switch (line.Args[0])
                    {
                        case "MSG":
                            if(line.Args.Length < 2)
                            {
                                Updated?.Invoke(this, new ProgressUpdated("문법 오류 줄:" + i.ToString(), 1, 0, 1));
                                return;
                            }
                            msg = line.Args[1];
                            break;
                        case "EXEC":
                            if (line.Args.Length < 2)
                            {
                                Updated?.Invoke(this, new ProgressUpdated("문법 오류 줄:" + i.ToString(), 1, 0, 1));
                                return;
                            }
                            string cmd = Path.Combine(workingDirectory, line.Args[1]);
                            Console.WriteLine("Excute: " + cmd);
                            Updated?.Invoke(this, new ProgressUpdated(msg, i, 0, Lines.Count));

                            string ext = Path.GetExtension(cmd);

                            try
                            {
                                ProcessStartInfo psi = new ProcessStartInfo();
                                if (ext == ".msi")
                                {
                                    psi.FileName = "msiexec";
                                }
                                else
                                {
                                    psi.FileName = cmd;
                                }

                                if (line.Args.Length > 2)
                                {
                                    string args = line.Args[2];
                                    args = args.Replace("$INST_DIR$", installDirectory);

                                    if (ext == ".msi")
                                    {
                                        psi.Arguments = "/i \"" + cmd.Replace('/', '\\') + "\" " + args;
                                    }
                                    else
                                    {
                                        psi.Arguments = args;
                                    }
                                }

                                if(psi.Arguments != null)
                                    Console.WriteLine(psi.FileName + " " + psi.Arguments);

                                psi.CreateNoWindow = true;
                                psi.UseShellExecute = false;
                                psi.RedirectStandardOutput = true;
                                psi.RedirectStandardError = true;
                                psi.StandardOutputEncoding = Encoding.UTF8;
                                psi.StandardErrorEncoding = Encoding.UTF8;
                                psi.WorkingDirectory = workingDirectory;

                                Process pc = new Process();
                                pc.StartInfo = psi;
                                pc.Start();

                                StreamReader sr = pc.StandardOutput;
                                while (!sr.EndOfStream)
                                {
                                    string pc_line = sr.ReadLine();
                                    Console.WriteLine("stdout: [" + Path.GetFileName(psi.FileName) + "] " + pc_line);
                                }

                                pc.Dispose();
                            }
                            catch (Exception e)
                            {
                                ExceptionEXEC?.Invoke(this, e);
                                Console.WriteLine(e.ToString());
                            }
                            break;
                    }
                }
                else
                {
                    Updated?.Invoke(this, new ProgressUpdated("문법 오류 줄:" + i.ToString(), 1, 0, 1));
                    return;
                }
            }
        }
    }
}
