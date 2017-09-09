using MMF.Sprite;
using NiTEWrapper;
using OpenNIWrapper;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System;

namespace MMF.Kinect
{
    public class ColorTexture : IDynamicTexture, IDisposable
    {
        private RenderContext context;

        private VideoStream videoStream;

        private short _trackId;

        private System.Collections.Generic.List<Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>> drawJoints = new System.Collections.Generic.List<Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>>
        {
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.HEAD, SkeletonJoint.JointType.NECK),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.NECK, SkeletonJoint.JointType.LEFT_SHOULDER),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.NECK, SkeletonJoint.JointType.RIGHT_SHOULDER),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.LEFT_SHOULDER, SkeletonJoint.JointType.LEFT_ELBOW),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.LEFT_ELBOW, SkeletonJoint.JointType.LEFT_HAND),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.RIGHT_SHOULDER, SkeletonJoint.JointType.RIGHT_ELBOW),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.RIGHT_ELBOW, SkeletonJoint.JointType.RIGHT_HAND),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.NECK, SkeletonJoint.JointType.TORSO),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.TORSO, SkeletonJoint.JointType.RIGHT_HIP),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.TORSO, SkeletonJoint.JointType.LEFT_HIP),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.LEFT_HIP, SkeletonJoint.JointType.LEFT_KNEE),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.RIGHT_HIP, SkeletonJoint.JointType.RIGHT_KNEE),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.LEFT_KNEE, SkeletonJoint.JointType.LEFT_FOOT),
            new Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType>(SkeletonJoint.JointType.RIGHT_KNEE, SkeletonJoint.JointType.RIGHT_FOOT)
        };

        private int PicWideth = 640;

        private int PicHeight = 480;

        public Texture2D TextureResource
        {
            get;
            set;
        }

        public short TrackId
        {
            get
            {
                return _trackId;
            }
            set
            {
                _trackId = value;
                if (_trackId > KinectDevice.CurrentUserTrackerFrameRef.Users.Length)
                {
                    _trackId = 0;
                }
            }
        }

        public ShaderResourceView TextureResourceView
        {
            get;
            private set;
        }

        public KinectDeviceManager KinectDevice
        {
            get;
            set;
        }

        public bool NeedUpdate
        {
            get;
            private set;
        }

        public ColorTexture(RenderContext context, KinectDeviceManager kinectDevice)
        {
            KinectDevice = kinectDevice;
            this.context = context;
            Texture2DDescription description = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.Write,
                Format = Format.R8G8B8A8_UNorm,
                Height = 480,
                Width = 640,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Dynamic
            };
            TextureResource = new Texture2D(context.DeviceManager.Device, description);
            TextureResourceView = new ShaderResourceView(context.DeviceManager.Device, TextureResource);
            videoStream = kinectDevice.KinnectDevice.CreateVideoStream(OpenNIWrapper.Device.SensorType.COLOR);
            videoStream.Start();
            NeedUpdate = true;
        }

        public void UpdateTexture()
        {
            int num = 640;
            int num2 = 480;
            DataBox dataBox = context.DeviceManager.Context.MapSubresource(TextureResource, 0, MapMode.WriteDiscard, SlimDX.Direct3D11.MapFlags.None);
            VideoFrameRef videoFrameRef = videoStream.readFrame();
            UserTrackerFrameRef currentUserTrackerFrameRef = KinectDevice.CurrentUserTrackerFrameRef;
            System.IntPtr pixels = currentUserTrackerFrameRef.UserMap.Pixels;
            byte[] array = new byte[num * num2 * 3];
            byte[] array2 = new byte[num * num2 * 2];
            System.Collections.Generic.List<byte> list = new System.Collections.Generic.List<byte>();
            System.Runtime.InteropServices.Marshal.Copy(videoFrameRef.Data, array, 0, num * num2 * 3);
            System.Runtime.InteropServices.Marshal.Copy(pixels, array2, 0, num * num2 * 2);
            dataBox.Data.Seek(0L, System.IO.SeekOrigin.Begin);
            UserData userById = KinectDevice.CurrentUserTrackerFrameRef.getUserById(KinectDevice.UserCursor);
            Skeleton skeleton = userById.Skeleton;
            int i = 0;
            while (i < num * num2)
            {
                short num3 = System.BitConverter.ToInt16(array2, i * 2);
                if (!userById.isValid || num3 != KinectDevice.UserCursor)
                {
                    goto SYMPHONY;
                }
                if (skeleton.State == Skeleton.SkeletonState.CALIBRATING)
                {
                    list.Add(255);
                    list.Add(255);
                    list.Add(0);
                    list.Add(255);
                }
                else if (skeleton.State == Skeleton.SkeletonState.TRACKED)
                {
                    list.Add(0);
                    list.Add(255);
                    list.Add(0);
                    list.Add(255);
                }
                else
                {
                    if (skeleton.State != Skeleton.SkeletonState.NONE)
                    {
                        goto SYMPHONY;
                    }
                    list.Add(0);
                    list.Add(255);
                    list.Add(255);
                    list.Add(255);
                }
                MMDMansei:
                i++;
                continue;
                SYMPHONY:
                list.Add(array[i * 3]);
                list.Add(array[i * 3 + 1]);
                list.Add(array[i * 3 + 2]);
                list.Add(255);
                goto MMDMansei;
            }
            foreach (System.Collections.Generic.KeyValuePair<short, UserData> current in KinectDevice.TrackedUsers)
            {
                UserData value = current.Value;
                foreach (Tuple<SkeletonJoint.JointType, SkeletonJoint.JointType> current2 in drawJoints)
                {
                    SkeletonJoint joint = value.Skeleton.getJoint(current2.Item1);
                    SkeletonJoint joint2 = value.Skeleton.getJoint(current2.Item2);
                    System.Drawing.PointF pointF = KinectDevice.NiteUserTracker.ConvertJointCoordinatesToDepth(joint.Position);
                    System.Drawing.PointF pointF2 = KinectDevice.NiteUserTracker.ConvertJointCoordinatesToDepth(joint2.Position);
                    byte b = (byte)(255f * (joint.PositionConfidence + joint2.PositionConfidence) / 2f);
                        DrawLine((int)pointF.X, (int)pointF.Y, (int)pointF2.X, (int)pointF2.Y, 2, new byte[]
                    {
                        b,
                        0,
                        b,
                        255
                    }, list);
                }
            }
            dataBox.Data.WriteRange(list.ToArray());
            context.DeviceManager.Context.UnmapSubresource(TextureResource, 0);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, int wideth, byte[] color, System.Collections.Generic.List<byte> drawed)
        {
            if (x1 >= -1 && PicWideth - 1 >= x1 && x2 >= -1 && PicWideth - 1 >= x2)
            {
                if (y1 >= -1 && PicHeight - 1 >= y1 && y2 >= -1 && PicHeight - 1 >= y2)
                {
                    if (x1 != x2 || y1 != y2)
                    {
                        if (x1 == x2)
                        {
                            int num = System.Math.Max(y1, y2);
                            int num2 = System.Math.Min(y1, y2);
                            for (int i = num2; i < num + 1; i++)
                            {
                                for (int j = x1 - wideth; j < x1 + wideth + 1; j++)
                                {
                                    for (int k = 0; k < 4; k++)
                                    {
                                        drawed[4 * (PicWideth * i + j) + k] = color[k];
                                    }
                                }
                            }
                        }
                        else if (y1 == y2)
                        {
                            int num3 = System.Math.Max(x1, x2);
                            int num4 = System.Math.Min(x1, x2);
                            for (int i = y1 - wideth; i < y1 + wideth + 1; i++)
                            {
                                for (int j = num4; j < num3 + 1; j++)
                                {
                                    for (int k = 0; k < 4; k++)
                                    {
                                        drawed[4 * (PicWideth * i + j) + k] = color[k];
                                    }
                                }
                            }
                        }
                        else
                        {
                            int num5 = y1 - y2;
                            int num6 = x2 - x1;
                            int num7 = x1 * y2 - x2 * y1;
                            int num8 = System.Math.Max(x1, x2);
                            int num9 = System.Math.Min(x1, x2);
                            int num10 = System.Math.Max(y1, y2);
                            int num11 = System.Math.Min(y1, y2);
                            for (int i = num11; i < num10 + 1; i++)
                            {
                                for (int j = num9; j < num8 + 1; j++)
                                {
                                    float num12 = System.Math.Abs((num5 * j + num6 * i + num7) / (float)num5);
                                    float num13 = System.Math.Abs((num5 * j + num6 * i + num7) / (float)num6);
                                    if (num12 * num13 / System.Math.Sqrt(num13 * num13 + num12 * num12) < wideth)
                                    {
                                        for (int k = 0; k < 4; k++)
                                        {
                                            drawed[4 * (PicWideth * i + j) + k] = color[k];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            context = null;

            TextureResource.Dispose();
            TextureResource = null;

            TextureResourceView.Dispose();
            TextureResourceView = null;
        }
    }
}