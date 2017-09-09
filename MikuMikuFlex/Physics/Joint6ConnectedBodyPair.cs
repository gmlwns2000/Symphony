namespace MMF.Physics
{
    internal class Joint6ConnectedBodyPair
    {
        public Joint6ConnectedBody connectedBodyA
        {
            get;
            private set;
        }

        public Joint6ConnectedBody connectedBodyB
        {
            get;
            private set;
        }

        public Joint6ConnectedBodyPair(Joint6ConnectedBody connectedBodyA, Joint6ConnectedBody connectedBodyB)
        {
            this.connectedBodyA = connectedBodyA;
            this.connectedBodyB = connectedBodyB;
        }
    }
}
