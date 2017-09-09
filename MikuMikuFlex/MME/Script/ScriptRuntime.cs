using MMF.MME.Script.Function;
using MMF.Model;
using System.Collections.Generic;

namespace MMF.MME.Script
{
    public class ScriptRuntime
    {
        internal static FunctionDictionary ScriptFunctions;

        internal List<FunctionBase> ParsedExecuters;

        public int CurrentExecuter;

        public Stack<int> LoopBegins = new Stack<int>();

        public Stack<int> LoopCounts = new Stack<int>();

        public Stack<int> LoopEndCount = new Stack<int>();

        public string ScriptCode
        {
            get;
            private set;
        }

        static ScriptRuntime()
        {
            ScriptRuntime.ScriptFunctions = new FunctionDictionary
            {
                new RenderColorTargetFunction(),
                new RenderDepthStencilTargetFunction(),
                new ClearSetColorFunction(),
                new ClearSetDepthFunction(),
                new ClearFunction(),
                new PassFunction(),
                new DrawFunction(),
                new LoopByCountFunction(),
                new LoopEndFunction(),
                new LoopGetIndexFunction()
            };
        }

        private ScriptRuntime(string script, RenderContext context, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            ScriptCode = script;
            Parse(context, manager, technique, pass);
        }

        public ScriptRuntime(string script, RenderContext context, MMEEffectManager manager, MMEEffectTechnique technique) : this(script, context, manager, technique, null)
        {
        }

        public ScriptRuntime(string script, RenderContext context, MMEEffectManager manager, MMEEffectPass pass) : this(script, context, manager, null, pass)
        {
        }

        private void Parse(RenderContext context, MMEEffectManager manager, MMEEffectTechnique technique, MMEEffectPass pass)
        {
            ParsedExecuters = new List<FunctionBase>();
            if (!string.IsNullOrWhiteSpace(ScriptCode))
            {
                string[] array = ScriptCode.Split(new char[]
                {
                    ';'
                });
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string text = array2[i];
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        int index = 0;
                        string[] array3 = text.Split(new char[]
                        {
                            '='
                        });
                        if (array3.Length > 2)
                        {
                            throw new InvalidMMEEffectShaderException("スクリプト中の=の数が多すぎます。");
                        }
                        char c = array3[0][array3[0].Length - 1];
                        if (char.IsNumber(c))
                        {
                            array3[0] = array3[0].Remove(array3[0].Length - 1);
                            index = int.Parse(c.ToString());
                        }
                        if (ScriptRuntime.ScriptFunctions.ContainsKey(array3[0]))
                        {
                            ParsedExecuters.Add(ScriptRuntime.ScriptFunctions[array3[0]].GetExecuterInstance(index, array3[1], context, this, manager, technique, pass));
                        }
                    }
                }
            }
        }

        public void Execute(System.Action<ISubset> drawAction, ISubset ipmxSubset)
        {
            CurrentExecuter = 0;
            while (CurrentExecuter < ParsedExecuters.Count)
            {
                ParsedExecuters[CurrentExecuter].Execute(ipmxSubset, drawAction);
                ParsedExecuters[CurrentExecuter].Increment(this);
            }
        }
    }
}