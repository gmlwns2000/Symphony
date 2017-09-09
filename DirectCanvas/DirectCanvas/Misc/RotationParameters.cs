namespace DirectCanvas.Misc
{
    public struct RotationParameters
    {
        private static RotationParameters _empty;
        public static RotationParameters Empty
        {
            get
            {
                return _empty;
            }
        }

        private float m_rotateX;
        private float m_rotateY;
        private float m_rotateZ;
        private PointF m_rotationCenter;

        public RotationParameters(float rotateX, float rotateY, float rotateZ, PointF rotationCenter)
        {
            m_rotateX = rotateX;
            m_rotateY = rotateY;
            m_rotateZ = rotateZ;
            m_rotationCenter = rotationCenter;
        }

        public RotationParameters(float rotateX, float rotateY, float rotateZ)
        {
            m_rotateX = rotateX;
            m_rotateY = rotateY;
            m_rotateZ = rotateZ;
            m_rotationCenter = new PointF(0.5f, 0.5f);
        }

        public float RotateX
        {
            get { return m_rotateX; }
            set { m_rotateX = value; }
        }

        public float RotateY
        {
            get { return m_rotateY; }
            set { m_rotateY = value; }
        }

        public float RotateZ
        {
            get { return m_rotateZ; }
            set { m_rotateZ = value; }
        }

        public PointF RotationCenter
        {
            get { return m_rotationCenter; }
            set { m_rotationCenter = value; }
        }
    }
}
