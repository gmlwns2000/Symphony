namespace MMF.MME.VariableSubscriber.ControlInfoSubscriber
{
    internal class ControlObjectAnnotation
    {
        public TargetObject Target
        {
            get;
            private set;
        }

        public bool IsString
        {
            get;
            private set;
        }

        public ControlObjectAnnotation(TargetObject target, bool isString)
        {
            Target = target;
            IsString = isString;
        }
    }
}
