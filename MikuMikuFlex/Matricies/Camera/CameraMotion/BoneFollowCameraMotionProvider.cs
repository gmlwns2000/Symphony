using MMF.Bone;
using MMF.Matricies.Projection;
using MMF.Model.PMX;
using SlimDX;
using System.Linq;

namespace MMF.Matricies.Camera.CameraMotion
{
    public class BoneFollowCameraMotionProvider : ICameraMotionProvider
    {
        private readonly PMXModel followModel;

        private readonly PMXBone followBone;

        public float Distance
        {
            get;
            set;
        }

        public bool IsRotationZAxis
        {
            get;
            set;
        }

        public Vector3 ViewFrom
        {
            get;
            set;
        }

        public BoneFollowCameraMotionProvider(PMXModel model, string boneName, float distance, Vector3 viewFrom, bool rotationZaxis = false)
        {
            followModel = model;
            PMXBone[] array = (from bone in model.Skinning.Bone
                               where bone.BoneName == boneName
                               select bone).ToArray<PMXBone>();
            if (array.Length == 0)
            {
                throw new System.InvalidOperationException(string.Format("ボーン\"{0}\"は見つかりませんでした。", boneName));
            }
            followBone = array[0];
            Distance = distance;
            IsRotationZAxis = rotationZaxis;
            ViewFrom = viewFrom;
        }

        void ICameraMotionProvider.UpdateCamera(CameraProvider cp, IProjectionMatrixProvider proj)
        {
            Matrix transformation = followBone.GlobalPose * Matrix.Scaling(followModel.Transformer.Scale) * Matrix.RotationQuaternion(followModel.Transformer.Rotation) * Matrix.Translation(followModel.Transformer.Position);
            Vector3 vector = Vector3.TransformCoordinate(followBone.Position, transformation);
            Vector3 vector2 = Vector3.TransformNormal(-ViewFrom, transformation);
            vector2.Normalize();
            cp.CameraPosition = vector + Distance * vector2;
            cp.CameraLookAt = vector;
            if (IsRotationZAxis)
            {
                Vector3 cameraUpVec = Vector3.TransformNormal(new Vector3(0f, 1f, 0f), transformation);
                cameraUpVec.Normalize();
                cp.CameraUpVec = cameraUpVec;
            }
        }
    }
}
