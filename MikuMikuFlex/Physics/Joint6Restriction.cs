namespace MMF.Physics
{
    internal class Joint6Restriction
    {
        public Joint6MovementRestriction movementRestriction
        {
            get;
            private set;
        }

        public Joint6RotationRestriction rotationRestriction
        {
            get;
            private set;
        }

        public Joint6Restriction(Joint6MovementRestriction movementRestriction, Joint6RotationRestriction rotationRestriction)
        {
            this.movementRestriction = movementRestriction;
            this.rotationRestriction = rotationRestriction;
        }
    }
}
