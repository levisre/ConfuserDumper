using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core
{
    public class PeReader : BinaryReader
    {
        public PeReader(Stream str) : base(str) { }

        Stack<long> s = new Stack<long>();

        long offset;
        public void SaveLocation()
        {
            s.Push(BaseStream.Position);
        }
        public void LoadLocation()
        {
            BaseStream.Position = s.Pop();
        }
        public long FileOffset { get { return offset; } set { offset = value; } }

        public void SetPosition(long pos)
        {
            BaseStream.Position = offset + pos;
        }
        public long GetPosition()
        {
            return BaseStream.Position - offset;
        }

        public DateTime ReadStamp()
        {
            return StampsHelper.StampFromUInt(ReadUInt32());
        }
    }

    public class PeWriter : BinaryWriter
    {
        public PeWriter(Stream str) : base(str) { }

        Stack<long> s = new Stack<long>();

        long offset;
        public void SaveLocation()
        {
            s.Push(BaseStream.Position);
        }
        public void LoadLocation()
        {
            BaseStream.Position = s.Pop();
        }
        public long FileOffset { get { return offset; } set { offset = value; } }

        public void SetPosition(long pos)
        {
            BaseStream.Position = offset + pos;
        }
        public long GetPosition()
        {
            return BaseStream.Position - offset;
        }

        public void WriteStamp(DateTime dt)
        {
            Write(StampsHelper.UIntFromStamp(dt));
        }
    }
}
