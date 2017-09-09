using BulletSharp;

namespace MMF.Physics
{
    internal class SuperProperty
    {
        public bool kinematic
        {
            get;
            private set;
        }

        public CollisionFilterGroups group
        {
            get;
            private set;
        }

        public CollisionFilterGroups mask
        {
            get;
            private set;
        }

        public SuperProperty(bool kinematic = false, CollisionFilterGroups group = CollisionFilterGroups.DefaultFilter, CollisionFilterGroups mask = CollisionFilterGroups.AllFilter)
        {
            this.kinematic = kinematic;
            this.group = group;
            this.mask = mask;
        }
    }
}
