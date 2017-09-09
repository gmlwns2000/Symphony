using MMF.MME.VariableSubscriber.MaterialSubscriber;
using MMF.Model;

namespace MMF.MME.VariableSubscriber
{
    public class SubscribeArgument
    {
        public IDrawable Model
        {
            get;
            private set;
        }

        public RenderContext Context
        {
            get;
            private set;
        }

        public MaterialInfo Material
        {
            get;
            private set;
        }

        public SubscribeArgument(IDrawable model, RenderContext context)
        {
            Model = model;
            Context = context;
        }

        public SubscribeArgument(MaterialInfo info, IDrawable model, RenderContext context)
        {
            Material = info;
            Context = context;
            Model = model;
        }
    }
}
