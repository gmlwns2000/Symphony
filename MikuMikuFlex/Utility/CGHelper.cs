using MMDFileParser.MotionParser;
using MMF.MME;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using System.Linq;

namespace MMF.Utility
{
    public static class CGHelper
	{
		public const string ToonDir = "Resource\\Toon\\";

		public static readonly Vector3 EularMaximum = new Vector3(3.14159274f, 1.57079637f, 3.14159274f);

		public static readonly Vector3 EularMinimum = -CGHelper.EularMaximum;

		public static bool Between(this float value, float min, float max)
		{
			return min <= value && max >= value;
		}

		public static Vector3 NormalizeEular(this Vector3 source)
		{
			if (!source.X.Between(-3.14159274f, 3.14159274f))
			{
				if (source.X > 0f)
				{
					source.X -= 6.28318548f;
				}
				else
				{
					source.X += 6.28318548f;
				}
			}
			if (!source.Y.Between(-1.57079637f, 1.57079637f))
			{
				if (source.Y > 0f)
				{
					source.Y -= 6.28318548f;
				}
				else
				{
					source.Y += 6.28318548f;
				}
			}
			if (!source.Z.Between(-3.14159274f, 3.14159274f))
			{
				if (source.Z > 0f)
				{
					source.Z -= 6.28318548f;
				}
				else
				{
					source.Z += 6.28318548f;
				}
			}
			return source;
		}

		public static Buffer CreateBuffer(System.Collections.Generic.IEnumerable<byte> dataList, Device device, BindFlags flag)
		{
            Buffer result;
			using (DataStream dataStream = new DataStream(dataList.ToArray<byte>(), true, true))
			{
				BufferDescription description = new BufferDescription
				{
					BindFlags = flag,
					SizeInBytes = (int)dataStream.Length
				};
				result = new Buffer(device, dataStream, description);
			}
			return result;
		}

		public static Buffer CreateBuffer<T>(System.Collections.Generic.IEnumerable<T> dataList, Device device, BindFlags flag)
		{
            Buffer result;
			using (DataStream dataStream = new DataStream(dataList.ToArray<T>(), true, true))
			{
				BufferDescription description = new BufferDescription
				{
					BindFlags = flag,
					SizeInBytes = (int)dataStream.Length
				};
				result = new Buffer(device, dataStream, description);
			}
			return result;
		}

		public static Buffer CreateBuffer(Device device, int size, BindFlags flag)
		{
			BufferDescription description = new BufferDescription
			{
				BindFlags = flag,
				SizeInBytes = size
			};
			return new Buffer(device, description);
		}

		public static Effect CreateEffectFx5(string filePath, Device device)
		{
			Effect result;
			using (System.IO.FileStream fileStream = System.IO.File.OpenRead(filePath))
			{
				using (ShaderBytecode shaderBytecode = EffectLoader.Instance.GetShaderBytecode(filePath, fileStream))
				{
					result = new Effect(device, shaderBytecode);
				}
			}
			return result;
		}

		internal static Effect CreateEffectFx5FromResource(string resourcePath, Device device)
		{
			System.Reflection.Assembly executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
			Effect result;
			using (System.IO.Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(resourcePath))
			{
				using (ShaderBytecode shaderBytecode = EffectLoader.Instance.GetShaderBytecode(resourcePath, manifestResourceStream))
				{
					result = new Effect(device, shaderBytecode);
				}
			}
			return result;
		}

		public static Vector3 GetTranslation(Matrix matrix)
		{
			return new Vector3(matrix.M41, matrix.M42, matrix.M43);
		}

		public static Quaternion getRotationQuaternion(Vector3 rotation)
		{
			return Quaternion.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
		}

		public static bool FactoringQuaternionZXY(Quaternion input, out float ZRot, out float XRot, out float YRot)
		{
			Quaternion quaternion = new Quaternion(input.X, input.Y, input.Z, input.W);
			quaternion.Normalize();
			Matrix matrix;
			Matrix.RotationQuaternion(ref quaternion, out matrix);
			bool result;
			if (matrix.M32 > 0.9999 || matrix.M32 < -0.9999)
			{
				XRot = (float)((matrix.M32 < 0f) ? 1.5707963267948966 : -1.5707963267948966);
				ZRot = 0f;
				YRot = (float)System.Math.Atan2(-matrix.M13, matrix.M11);
				result = false;
			}
			else
			{
				XRot = -(float)System.Math.Asin(matrix.M32);
				ZRot = (float)System.Math.Asin(matrix.M12 / System.Math.Cos(XRot));
				if (float.IsNaN(ZRot))
				{
					XRot = (float)((matrix.M32 < 0f) ? 1.5707963267948966 : -1.5707963267948966);
					ZRot = 0f;
					YRot = (float)System.Math.Atan2(-matrix.M13, matrix.M11);
					result = false;
				}
				else
				{
					if (matrix.M22 < 0f)
					{
						ZRot = (float)(3.1415926535897931 - ZRot);
					}
					YRot = (float)System.Math.Atan2(matrix.M31, matrix.M33);
					result = true;
				}
			}
			return result;
		}

		public static bool FactoringQuaternionXYZ(Quaternion input, out float XRot, out float YRot, out float ZRot)
		{
			Quaternion quaternion = new Quaternion(input.X, input.Y, input.Z, input.W);
			quaternion.Normalize();
			Matrix matrix;
			Matrix.RotationQuaternion(ref quaternion, out matrix);
			bool result;
			if (matrix.M13 > 0.9999 || matrix.M13 < -0.9999)
			{
				XRot = 0f;
				YRot = (float)((matrix.M13 < 0f) ? 1.5707963267948966 : -1.5707963267948966);
				ZRot = -(float)System.Math.Atan2(-matrix.M21, matrix.M22);
				result = false;
			}
			else
			{
				YRot = -(float)System.Math.Asin(matrix.M13);
				XRot = (float)System.Math.Asin(matrix.M23 / System.Math.Cos(YRot));
				if (float.IsNaN(XRot))
				{
					XRot = 0f;
					YRot = (float)((matrix.M13 < 0f) ? 1.5707963267948966 : -1.5707963267948966);
					ZRot = -(float)System.Math.Atan2(-matrix.M21, matrix.M22);
					result = false;
				}
				else
				{
					if (matrix.M33 < 0f)
					{
						XRot = (float)(3.1415926535897931 - XRot);
					}
					ZRot = (float)System.Math.Atan2(matrix.M12, matrix.M11);
					result = true;
				}
			}
			return result;
		}

		public static bool FactoringQuaternionYZX(Quaternion input, out float YRot, out float ZRot, out float XRot)
		{
			Quaternion quaternion = new Quaternion(input.X, input.Y, input.Z, input.W);
			quaternion.Normalize();
			Matrix matrix;
			Matrix.RotationQuaternion(ref quaternion, out matrix);
			bool result;
			if (matrix.M21 > 0.9999 || matrix.M21 < -0.9999)
			{
				YRot = 0f;
				ZRot = (float)((matrix.M21 < 0f) ? 1.5707963267948966 : -1.5707963267948966);
				XRot = -(float)System.Math.Atan2(-matrix.M32, matrix.M33);
				result = false;
			}
			else
			{
				ZRot = -(float)System.Math.Asin(matrix.M21);
				YRot = (float)System.Math.Asin(matrix.M31 / System.Math.Cos(ZRot));
				if (float.IsNaN(YRot))
				{
					YRot = 0f;
					ZRot = (float)((matrix.M21 < 0f) ? 1.5707963267948966 : -1.5707963267948966);
					XRot = -(float)System.Math.Atan2(-matrix.M32, matrix.M33);
					result = false;
				}
				else
				{
					if (matrix.M11 < 0f)
					{
						YRot = (float)(3.1415926535897931 - YRot);
					}
					XRot = (float)System.Math.Atan2(matrix.M23, matrix.M22);
					result = true;
				}
			}
			return result;
		}

		public static Vector3 MulEachMember(Vector3 vec1, Vector3 vec2)
		{
			return new Vector3(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z);
		}

		public static Vector4 MulEachMember(Vector4 vec1, Vector4 vec2)
		{
			return new Vector4(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z, vec1.W * vec2.W);
		}

		public static Vector3 ToRadians(Vector3 degree)
		{
			return new Vector3(CGHelper.ToRadians(degree.X), CGHelper.ToRadians(degree.Y), CGHelper.ToRadians(degree.Z));
		}

		public static float ToRadians(float degree)
		{
			return degree * 3.14159274f / 180f;
		}

		public static float ToDegree(float radians)
		{
			return 180f * radians / 3.14159274f;
		}

		public static Vector3 ToDegree(Vector3 radian)
		{
			return new Vector3(CGHelper.ToDegree(radian.X), CGHelper.ToDegree(radian.Y), CGHelper.ToDegree(radian.Z));
		}

		public static float Clamp(float value, float min, float max)
		{
			float result;
			if (min > value)
			{
				result = min;
			}
			else if (max < value)
			{
				result = max;
			}
			else
			{
				result = value;
			}
			return result;
		}

		public static void LoadIndex(int surface, System.Collections.Generic.List<byte> InputIndexBuffer)
		{
			ushort value = (ushort)surface;
			byte[] bytes = System.BitConverter.GetBytes(value);
			for (int i = 0; i < bytes.Length; i++)
			{
				InputIndexBuffer.Add(bytes[i]);
			}
		}

		public static void AddListBuffer(float value, System.Collections.Generic.List<byte> InputBuffer)
		{
			byte[] bytes = System.BitConverter.GetBytes(value);
			for (int i = 0; i < bytes.Length; i++)
			{
				InputBuffer.Add(bytes[i]);
			}
		}

		public static void AddListBuffer(int value, System.Collections.Generic.List<byte> InputBuffer)
		{
			CGHelper.AddListBuffer((ushort)value, InputBuffer);
		}

		public static void AddListBuffer(ushort value, System.Collections.Generic.List<byte> InputBuffer)
		{
			byte[] bytes = System.BitConverter.GetBytes(value);
			for (int i = 0; i < bytes.Length; i++)
			{
				InputBuffer.Add(bytes[i]);
			}
		}

		public static void AddListBuffer(Vector2 value, System.Collections.Generic.List<byte> InputBuffer)
		{
			CGHelper.AddListBuffer(value.X, InputBuffer);
			CGHelper.AddListBuffer(value.Y, InputBuffer);
		}

		public static void AddListBuffer(Vector3 value, System.Collections.Generic.List<byte> InputBuffer)
		{
			CGHelper.AddListBuffer(value.X, InputBuffer);
			CGHelper.AddListBuffer(value.Y, InputBuffer);
			CGHelper.AddListBuffer(value.Z, InputBuffer);
		}

		public static void AddListBuffer(Vector4 value, System.Collections.Generic.List<byte> InputBuffer)
		{
			CGHelper.AddListBuffer(value.X, InputBuffer);
			CGHelper.AddListBuffer(value.Y, InputBuffer);
			CGHelper.AddListBuffer(value.Z, InputBuffer);
			CGHelper.AddListBuffer(value.W, InputBuffer);
		}

		public static Quaternion ComplementRotateQuaternion(BoneFrameData frame1, BoneFrameData frame2, float progress)
		{
			return Quaternion.Slerp(frame1.BoneRotatingQuaternion, frame2.BoneRotatingQuaternion, progress);
		}

		public static Quaternion ComplementRotateQuaternion(CameraFrameData frame1, CameraFrameData frame2, float progress)
		{
			Quaternion start = Quaternion.RotationYawPitchRoll(frame1.CameraRotation.Y, frame1.CameraRotation.X, frame1.CameraRotation.Z);
			Quaternion end = Quaternion.RotationYawPitchRoll(frame2.CameraRotation.Y, frame2.CameraRotation.X, frame2.CameraRotation.Z);
			return Quaternion.Slerp(start, end, progress);
		}

		public static Vector3 ComplementTranslate(BoneFrameData frame1, BoneFrameData frame2, Vector3 progress)
		{
			return new Vector3(CGHelper.Lerp(frame1.BonePosition.X, frame2.BonePosition.X, progress.X), CGHelper.Lerp(frame1.BonePosition.Y, frame2.BonePosition.Y, progress.Y), CGHelper.Lerp(frame1.BonePosition.Z, frame2.BonePosition.Z, progress.Z));
		}

		public static Vector3 ComplementTranslate(CameraFrameData frame1, CameraFrameData frame2, Vector3 progress)
		{
			return new Vector3(CGHelper.Lerp(frame1.CameraPosition.X, frame2.CameraPosition.X, progress.X), CGHelper.Lerp(frame1.CameraPosition.Y, frame2.CameraPosition.Y, progress.Y), CGHelper.Lerp(frame1.CameraPosition.Z, frame2.CameraPosition.Z, progress.Z));
		}

		public static float Lerp(float start, float end, float factor)
		{
			return start + (end - start) * factor;
		}
	}
}