namespace MMF.MME
{
    public class MMEEffectException : System.Exception
    {
        private readonly string message = "MMEエフェクトの解析中に例外が発生しました。";

        public override string Message
        {
            get
            {
                return message;
            }
        }

        public MMEEffectException(string message)
        {
            this.message = message;
        }
    }
}
