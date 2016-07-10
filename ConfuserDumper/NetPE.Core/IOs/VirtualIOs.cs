using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core.Pe
{
    public class VirtualStream : Stream
    {
        internal VirtualStream(PeFile file)
        {
            this.file = file;
        }
        PeFile file;
        Section current;
        Rva rva;

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            return;
        }

        public override long Length
        {
            get
            {
                uint ret = 0;
                foreach (Section sect in file.SectionHeaders)
                {
                    if (sect.VirtualAddress + sect.VirtualSize > ret)
                        ret = sect.VirtualAddress + sect.VirtualSize;
                }
                return ret + 16;
            }
        }

        void UpdateSection()
        {
            int unused;
            file.SectionHeaders.Resolve(rva, out current, out unused);
        }

        public override long Position
        {
            get
            {
                return rva;
            }
            set
            {
                rva = (Rva)value;
                UpdateSection();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int ret = 0;
            for (int i = 0; i < count; i++)
            {
                if (current == null)
                    buffer[i + offset] = 0;
                else
                {
                    var tmpOff = rva - current.VirtualAddress;
                    if (tmpOff < current.Data.Length)
                        buffer[i + offset] = current.Data[tmpOff];
                    else
                    {
                        //Offset out of RawSize is padding by zero (Pecoff)
                        buffer[i + offset] = 0;
                    }
                    //buffer[i + offset] = current.Data[rva - current.VirtualAddress];
                }
                ret++;
                rva++;
                if (current == null || rva >= (current.VirtualAddress + current.VirtualSize))
                    UpdateSection();
            }
            return ret;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    rva = (Rva)offset; break;
                case SeekOrigin.Current:
                    rva += (Rva)offset; break;
                case SeekOrigin.End:
                    throw new InvalidOperationException();
            }
            return rva;
        }

        public override void SetLength(long value)
        {
            return;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (current != null)
                    current.Data[rva - current.VirtualAddress] = buffer[offset + i];
                if (current == null || rva >= (current.VirtualAddress + current.VirtualSize))
                    UpdateSection();
                rva++;
            }
        }

        public PeFile File { get { return file; } }
    }

    public class VirtualReader : BinaryReader
    {
        public VirtualReader(VirtualStream str) : base(str,Encoding.ASCII) { }

        public new VirtualStream BaseStream
        {
            get
            {
                return base.BaseStream as VirtualStream;
            }
        }

        public override string ReadString()
        {
            StringBuilder ret = new StringBuilder();
            char tmp;
            while ((tmp = this.ReadChar()) != '\0') ret.Append(tmp);
            return ret.ToString();
        }

        public string ReadUnicodeString()
        {
            char[] ret = new char[base.ReadUInt16()];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (char)base.ReadUInt16();
            }
            return new string(ret);
        }

        public string Read7BitsEncodeString()
        {
            return base.ReadString();
        }

        public Rva ReadRva()
        {
            return new Rva(this.ReadUInt32());
        }

        public DateTime ReadStamp()
        {
            return StampsHelper.StampFromUInt(this.ReadUInt32());
        }


        Stack<long> s = new Stack<long>();

        public void SaveLocation()
        {
            s.Push(BaseStream.Position);
        }
        public void LoadLocation()
        {
            BaseStream.Position = s.Pop();
        }

        public void SetPosition(Rva pos)
        {
            BaseStream.Position = pos;
        }
        public Rva GetPosition()
        {
            return (Rva)BaseStream.Position;
        }
    }

    public class VirtualWriter : BinaryWriter
    {
        public VirtualWriter(VirtualStream str) : base(str, Encoding.ASCII) { }

        public new VirtualStream BaseStream
        {
            get
            {
                return base.BaseStream as VirtualStream;
            }
        }

        public override void Write(string value)
        {
            foreach (char i in value)
            {
                base.Write((byte)i);
            }
            base.Write((byte)0);
        }

        public void WriteUnicodeString(string value)
        {
            base.Write((ushort)value.Length);
            foreach (char i in value)
            {
                base.Write((ushort)i);
            }
        }
        public void Write7BitsEncodeString(string value)
        {
            base.Write(value);
        }

        public void WriteStamp(DateTime dt)
        {
            base.Write(StampsHelper.UIntFromStamp(dt));
        }


        Stack<long> s = new Stack<long>();

        public void SaveLocation()
        {
            s.Push(BaseStream.Position);
        }
        public void LoadLocation()
        {
            BaseStream.Position = s.Pop();
        }

        public void SetPosition(Rva pos)
        {
            BaseStream.Position = pos;
        }
        public Rva GetPosition()
        {
            return (Rva)BaseStream.Position;
        }
    }
}
