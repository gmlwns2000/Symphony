using MMF.Utility;
using SlimDX;

namespace MMF.Bone
{
    public class CCDIK : ITransformUpdater
    {
        private System.Collections.Generic.List<PMXBone> IKbones;

        public CCDIK(System.Collections.Generic.List<PMXBone> IKbones)
        {
            this.IKbones = IKbones;
        }

        public bool UpdateTransform()
        {
            foreach (PMXBone current in IKbones)
            {
                UpdateEachIKBoneTransform(current);
            }
            return false;
        }

        private void UpdateEachIKBoneTransform(PMXBone IKbone)
        {
            foreach (IkLink current in IKbone.ikLinks)
            {
                current.loopCount = 0;
            }
            for (int i = 0; i < IKbone.Iterator; i++)
            {
                IKloop(IKbone);
            }
        }

        private void IKloop(PMXBone IKbone)
        {
            PMXBone ikTargetBone = IKbone.IkTargetBone;
            Vector3 targetGlobalPos = Vector3.TransformCoordinate(IKbone.Position, IKbone.GlobalPose);
            foreach (IkLink current in IKbone.ikLinks)
            {
                Vector3 link2Effector = GetLink2Effector(current, ikTargetBone);
                Vector3 link2Target = GetLink2Target(current, targetGlobalPos);
                IKLinkCalc(current, link2Effector, link2Target, IKbone.RotationLimited);
            }
        }

        private Vector3 GetLink2Effector(IkLink ikLink, PMXBone effector)
        {
            Matrix right = Matrix.Invert(ikLink.ikLinkBone.GlobalPose);
            Vector3 left = Vector3.TransformCoordinate(effector.Position, effector.GlobalPose * right);
            return Vector3.Normalize(left - ikLink.ikLinkBone.Position);
        }

        private Vector3 GetLink2Target(IkLink ikLink, Vector3 TargetGlobalPos)
        {
            Matrix matrix = Matrix.Invert(ikLink.ikLinkBone.GlobalPose);
            Vector3 left;
            Vector3.TransformCoordinate(ref TargetGlobalPos, ref matrix, out left);
            return Vector3.Normalize(left - ikLink.ikLinkBone.Position);
        }

        private void IKLinkCalc(IkLink ikLink, Vector3 link2Effector, Vector3 link2Target, float RotationLimited)
        {
            float num = Vector3.Dot(link2Effector, link2Target);
            if (num > 1f)
            {
                num = 1f;
            }
            float num2 = ClampFloat((float)System.Math.Acos(num), RotationLimited);
            if (!float.IsNaN(num2))
            {
                if (num2 > 0.001f)
                {
                    Vector3 axis = Vector3.Cross(link2Effector, link2Target);
                    ikLink.loopCount++;
                    Quaternion left = Quaternion.RotationAxis(axis, num2);
                    left.Normalize();
                    ikLink.ikLinkBone.Rotation = left * ikLink.ikLinkBone.Rotation;
                    RestrictRotation(ikLink);
                    ikLink.ikLinkBone.UpdateGrobalPose();
                }
            }
        }

        private float ClampFloat(float f, float limit)
        {
            return System.Math.Max(System.Math.Min(f, limit), -limit);
        }

        private void RestrictRotation(IkLink ikLink)
        {
            if (ikLink.isLimited)
            {
                float x;
                float y;
                float z;
                int num = SplitRotation(ikLink.ikLinkBone.Rotation, out x, out y, out z);
                Vector3 vector = Vector3.Clamp(new Vector3(x, y, z).NormalizeEular(), ikLink.minRot, ikLink.maxRot);
                x = vector.X;
                y = vector.Y;
                z = vector.Z;
                switch (num)
                {
                    case 0:
                        ikLink.ikLinkBone.Rotation = Quaternion.RotationMatrix(Matrix.RotationX(x) * Matrix.RotationY(y) * Matrix.RotationZ(z));
                        break;
                    case 1:
                        ikLink.ikLinkBone.Rotation = Quaternion.RotationMatrix(Matrix.RotationY(y) * Matrix.RotationZ(z) * Matrix.RotationX(x));
                        break;
                    case 2:
                        ikLink.ikLinkBone.Rotation = Quaternion.RotationYawPitchRoll(y, x, z);
                        break;
                }
            }
        }

        private int SplitRotation(Quaternion Rotation, out float xRotation, out float yRotation, out float zRotation)
        {
            int result;
            if (CGHelper.FactoringQuaternionXYZ(Rotation, out xRotation, out yRotation, out zRotation))
            {
                result = 0;
            }
            else if (CGHelper.FactoringQuaternionYZX(Rotation, out yRotation, out zRotation, out xRotation))
            {
                result = 1;
            }
            else
            {
                CGHelper.FactoringQuaternionZXY(Rotation, out zRotation, out xRotation, out yRotation);
                result = 2;
            }
            return result;
        }
    }
}
