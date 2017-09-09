namespace DirectCanvas
{
    class CompositionDrawStateManagement : DrawStateManagement
    {
        public override void EndDrawState()
        {
            base.EndDrawState();
        }

        protected override void ValidateBeginDrawState()
        {
            base.ValidateBeginDrawState();
        }

        protected override void ValidateEndDrawState()
        {
            base.ValidateEndDrawState();
        }

        public override void DrawPreamble()
        {
            base.DrawPreamble();
        }
    }
}
