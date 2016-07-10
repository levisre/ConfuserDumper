using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Pe;

namespace NetPE.Core.DataDirectories
{
    public class TLSDirectory : DataDirectory
    {
        public TLSDirectory(DataDirectoryEntry entry) : base(entry) { }

        public override DataDirectoryType Type
        {
            get { return DataDirectoryType.TLS; }
        }

        Rva s;
        public Rva StartOfRawData { get { return s; } set { s = value; } }
        Rva e;
        public Rva EndOfRawData { get { return e; } set { e = value; } }
        Rva idx;
        public Rva AddressOfIndex { get { return idx; } set { idx = value; } }
        Rva cb;
        public Rva AddressOfCallbacks { get { return cb; } set { cb = value; } }
        uint sz;
        public uint SizeOfZeroFill { get { return sz; } set { sz = value; } }
        uint c;
        public uint Characteristics { get { return c; } set { c = value; } }


        bool isPe32Plus = false;

        public override uint GetTotalSize()
        {
            return isPe32Plus ? 40U : 24U;
        }

        public override uint GetDirectorySize()
        {
            return isPe32Plus ? 40U : 24U;
        }

        protected override void SaveInternal(VirtualWriter wtr)
        {
            ulong bas = wtr.BaseStream.File.OptionalHeader.WindowsSpecificFields.ImageBase;
            if (wtr.BaseStream.File.OptionalHeader.Type == OptionalHeader.ExecutableType.PE32Plus)
            {
                wtr.Write((ulong)s + bas);
                wtr.Write((ulong)e + bas);
                wtr.Write((ulong)idx + bas);
                wtr.Write((ulong)s + bas);
                wtr.Write((ulong)cb + bas);
            }
            else
            {
                wtr.Write((uint)(s + bas));
                wtr.Write((uint)(e + bas));
                wtr.Write((uint)(idx + bas));
                wtr.Write((uint)(s + bas));
                wtr.Write((uint)(cb + bas));
            }
            wtr.Write(sz);
            wtr.Write(c);
        }

        protected override void LoadInternal(VirtualReader rdr)
        {
            ulong bas = rdr.BaseStream.File.OptionalHeader.WindowsSpecificFields.ImageBase;
            if (rdr.BaseStream.File.OptionalHeader.Type == OptionalHeader.ExecutableType.PE32Plus)
            {
                isPe32Plus = true;
                s = (uint)(rdr.ReadUInt64() - bas);
                e = (uint)(rdr.ReadUInt64() - bas);
                idx = (uint)(rdr.ReadUInt64() - bas);
                cb = (uint)(rdr.ReadUInt64() - bas);
            }
            else
            {
                isPe32Plus = false;
                s = (uint)(rdr.ReadUInt32() - bas);
                e = (uint)(rdr.ReadUInt32() - bas);
                idx = (uint)(rdr.ReadUInt32() - bas);
                cb = (uint)(rdr.ReadUInt32() - bas);
            }
            sz = rdr.ReadUInt32();
            c = rdr.ReadUInt32();
        }
    }
}
