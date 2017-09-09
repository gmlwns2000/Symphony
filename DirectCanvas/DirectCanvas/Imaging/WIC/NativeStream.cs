using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DirectCanvas.Imaging.WIC
{
    internal class NativeStream : IStream, IDisposable
    {
        private readonly Stream m_stream;

        public NativeStream(Stream stream)
        {
            m_stream = stream;
        }

        #region IStream Members

        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            int cbRead = m_stream.Read(pv, 0, cb);
            if (pcbRead != IntPtr.Zero)
                Marshal.WriteInt32(pcbRead, cbRead);
        }

        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            m_stream.Write(pv, 0, cb);
            if (pcbWritten != IntPtr.Zero)
                Marshal.WriteInt32(pcbWritten, cb);
        }

        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            long lPos = m_stream.Seek(dlibMove, (SeekOrigin)dwOrigin);
            if (plibNewPosition != IntPtr.Zero)
                Marshal.WriteInt64(plibNewPosition, lPos);
        }

        public void SetSize(long libNewSize)
        {
            throw new NotImplementedException("SetSize is not implemented.");
        }

        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            throw new NotImplementedException("CopyTo is not implemented.");
        }

        public void Commit(int grfCommitFlags)
        {
            throw new NotImplementedException("Commit is not implemented.");
        }

        public void Revert()
        {
            throw new NotImplementedException("Revert is not implemented.");
        }

        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException("LockRegion is not implemented.");
        }

        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotImplementedException("UnlockRegion is not implemented.");
        }

        public void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new System.Runtime.InteropServices.ComTypes.STATSTG { cbSize = m_stream.Length };
        }

        public void Clone(out IStream ppstm)
        {
            throw new NotImplementedException("Clone is not implemented.");
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_stream != null)
                m_stream.Close();
        }

        #endregion
    }
}