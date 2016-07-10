using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Pe;

namespace NetPE.Core.DataDirectories
{
    public class DebugDirectory : DataDirectory
    {
        public DebugDirectory(DataDirectoryEntry entry) : base(entry) { }

        public enum DebugType
        {
            Unknown = 0,
            COFF = 1,
            CodeView = 2,
            FramePointerOmission = 3,
            Misc = 4,
            Exception = 5,
            Fixup = 6,
            Map_To_Source = 7,
            Map_From_Source = 8,
            Borland = 9,
            Reserved10 = 10,
            CLSID = 11
        }

        uint c;
        public uint Characteristics { get { return c; } set { c = value; } }
        DateTime stamp;
        public DateTime DateTimeStamp { get { return stamp; } set { stamp = value; } }
        ushort maVer;
        public ushort MajorVersion { get { return maVer; } set { maVer = value; } }
        ushort miVer;
        public ushort MinorVersion { get { return miVer; } set { miVer = value; } }
        DebugType t;
        public DebugType DebugInfoType { get { return t; } set { t = value; } }
        uint s;
        public uint SizeOfData { get { return s; } set { s = value; } }
        Rva adr;
        public Rva AddressOfRawData { get { return adr; } set { adr = value; } }
        uint ptr;
        public uint PointerToRawData { get { return ptr; } set { ptr = value; } }

        public override DataDirectoryType Type
        {
            get { return DataDirectoryType.Debug; }
        }

        public override uint GetTotalSize()
        {
            return 28;
        }

        public override uint GetDirectorySize()
        {
            return 28;
        }

        protected override void SaveInternal(VirtualWriter wtr)
        {
            wtr.Write((uint)c);
            wtr.WriteStamp(stamp);
            wtr.Write((ushort)maVer);
            wtr.Write((ushort)miVer);
            wtr.Write((uint)t);
            wtr.Write((uint)s);
            wtr.Write(this.adr);
            wtr.Write((uint)ptr);
        }

        protected override void LoadInternal(VirtualReader rdr)
        {
            c = rdr.ReadUInt32();
            stamp = rdr.ReadStamp();
            maVer = rdr.ReadUInt16();
            miVer = rdr.ReadUInt16();
            t = (DebugType)rdr.ReadUInt32();
            s = rdr.ReadUInt32();
            this.adr = rdr.ReadRva();
            ptr = rdr.ReadUInt32();
        }
    }
}
