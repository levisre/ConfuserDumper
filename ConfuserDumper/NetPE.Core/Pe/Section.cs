using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core.Pe
{
    public class Section
    {
        [Flags]
        public enum SectionFlags : uint
        {
            Reserved0 = 0x00000000,
            Reserved1 = 0x00000001,
            Reserved2 = 0x00000002,
            Reserved3 = 0x00000004,
            Section_No_Padding = 0x00000008,
            Reserved4 = 0x00000010,
            Contains_Code = 0x00000020,
            Contains_Initialized_Data = 0x00000040,
            Contains_Uninitialized_Data = 0x00000080,
            Linker_Other = 0x00000100,
            Linker_Information = 0x00000200,
            Reserved5 = 0x00000400,
            Linker_Remove = 0x00000800,
            Linker_COMDAT = 0x00001000,
            Global_Pointer_Referenced = 0x00008000,
            Memory_Purgeable = 0x00020000,
            Memory_16Bit = 0x00020000,
            Memory_Locked = 0x00040000,
            Memory_Preload = 0x00080000,
            Align_1Bytes = 0x00100000,
            Align_2Bytes = 0x00200000,
            Align_4Bytes = 0x00300000,
            Align_8Bytes = 0x00400000,
            Align_16Bytes = 0x00500000,
            Align_32Bytes = 0x00600000,
            Align_64Bytes = 0x00700000,
            Align_128Bytes = 0x00800000,
            Align_256Bytes = 0x00900000,
            Align_512Bytes = 0x00A00000,
            Align_1024Bytes = 0x00B00000,
            Align_2048Bytes = 0x00C00000,
            Align_4096Bytes = 0x00D00000,
            Align_8192Bytes = 0x00E00000,
            Linker_Extended_Relocations = 0x01000000,
            Memory_Discardable = 0x02000000,
            Memory_Not_Cached = 0x04000000,
            Memory_Not_Paged = 0x08000000,
            Memory_Shared = 0x10000000,
            Memory_Executable = 0x20000000,
            Memory_Readable = 0x40000000,
            Memory_Writable = 0x80000000
        }

        SectionHeaders hdr;
        public Section(SectionHeaders hdr) { this.hdr = hdr; }

        private string n;
        public string Name
        {
            get
            {
                return n;
            }
            set
            {
                n = value;
            }
        }
        private uint vSize;
        public uint VirtualSize
        {
            get
            {
                return vSize;
            }
            set
            {
                vSize = value;
            }
        }
        private uint vPtr;
        public uint VirtualAddress
        {
            get
            {
                return vPtr;
            }
            set
            {
                vPtr = value;
            }
        }
        private uint relocPtr;
        public uint PointerToRelocations
        {
            get
            {
                return relocPtr;
            }
            set
            {
                relocPtr = value;
            }
        }
        private uint lnPtr;
        public uint PointerToLinenumbers
        {
            get
            {
                return lnPtr;
            }
            set
            {
                lnPtr = value;
            }
        }
        private ushort relocNo;
        public ushort NumberOfRelocations
        {
            get
            {
                return relocNo;
            }
            set
            {
                relocNo = value;
            }
        }
        private ushort lnNo;
        public ushort NumberOfLinenumbers
        {
            get
            {
                return lnNo;
            }
            set
            {
                lnNo = value;
            }
        }
        private SectionFlags c;
        public SectionFlags Characteristics
        {
            get
            {
                return c;
            }
            set
            {
                c = value;
            }
        }
        private byte[] dat = new byte[0];
        public byte[] Data { get { return dat; } set { dat = value; } }



        internal void Read(PeReader rdr)
        {
            n = new string(rdr.ReadChars(8)).Trim('\0');
            vSize = rdr.ReadUInt32();
            vPtr = rdr.ReadUInt32();
            uint rSize = rdr.ReadUInt32();
            uint rPtr = rdr.ReadUInt32();
            relocPtr = rdr.ReadUInt32();
            lnPtr = rdr.ReadUInt32();
            relocNo = rdr.ReadUInt16();
            lnNo = rdr.ReadUInt16();
            c = (SectionFlags)rdr.ReadUInt32();
            rdr.SaveLocation();
            rdr.SetPosition(rPtr);
            dat = rdr.ReadBytes((int)rSize);
            rdr.LoadLocation();
        }

        internal void Write(PeWriter wtr, ref uint datPtr)
        {
            for (int i = 0; i < 8; i++)
            {
                if (i < n.Length) wtr.Write((byte)n[i]); else wtr.Write((byte)0);
            }
            wtr.Write((uint)vSize);
            wtr.Write((uint)vPtr);
            wtr.Write((uint)dat.Length);
            wtr.Write((uint)datPtr); 
            wtr.Write((uint)relocPtr);
            wtr.Write((uint)lnPtr);
            wtr.Write((ushort)relocNo);
            wtr.Write((ushort)lnNo);
            wtr.Write((uint)c);
            wtr.SaveLocation();
            wtr.SetPosition(datPtr);
            wtr.Write(dat);
            wtr.LoadLocation();
            datPtr += (uint)dat.Length;
        }

        public override string ToString()
        {
            return n;
        }
    }
}
