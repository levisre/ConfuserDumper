using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.DataDirectories;
using System.Collections;
using System.IO;
using System.Collections.ObjectModel;

namespace NetPE.Core.Pe
{
    public class OptionalHeader
    {
        PeFile file;
        internal OptionalHeader(PeFile file) { this.file = file; }

        public enum ExecutableType
        {
            PE32 = 0x10b,
            PE32Plus = 0x20b,
            ROM = 0x107
        }
        private ExecutableType t;
        public ExecutableType Type
        {
            get
            {
                return t;
            }
            set
            {
                t = value;
            }
        }


        public class StandardFieldsHeader : PeComponent
        {
            OptionalHeader hdr;
            internal StandardFieldsHeader(PeFile file, OptionalHeader hdr)
                : base(file)
            {
                this.hdr = hdr;
            }

            public ExecutableType Magic
            {
                get
                {
                    return hdr.Type;
                }
                set
                {
                    hdr.Type = value;
                }
            }

            private byte maL;
            public byte MajorLinkerVersion
            {
                get
                {
                    return maL;
                }
                set
                {
                    maL = value;
                }
            }

            private byte miL;
            public byte MinorLinkerVersion
            {
                get
                {
                    return miL;
                }
                set
                {
                    miL = value;
                }
            }

            private uint sC;
            public uint SizeOfCode
            {
                get
                {
                    return sC;
                }
                set
                {
                    sC = value;
                }
            }

            private uint sI;
            public uint SizeOfInitializedData
            {
                get
                {
                    return sI;
                }
                set
                {
                    sI = value;
                }
            }

            private uint sU;
            public uint SizeOfUninitializedData
            {
                get
                {
                    return sU;
                }
                set
                {
                    sU = value;
                }
            }

            private Rva entry;
            public Rva AddressOfEntryPoint
            {
                get
                {
                    return entry;
                }
                set
                {
                    entry = value;
                }
            }

            private uint bc;
            public uint BaseOfCode
            {
                get
                {
                    return bc;
                }
                set
                {
                    bc = value;
                }
            }

            private uint bd;
            public uint BaseOfData
            {
                get
                {
                    if (Magic == ExecutableType.PE32Plus) throw new InvalidOperationException();
                    return bd;
                }
                set
                {
                    if (Magic == ExecutableType.PE32Plus) throw new InvalidOperationException();
                    bd = value;
                }
            }

            public override void Read(PeReader rdr)
            {
                rdr.SetPosition(hdr.StandardFieldsOffset);
                rdr.ReadUInt16();
                maL = rdr.ReadByte();
                miL = rdr.ReadByte();
                sC = rdr.ReadUInt32();
                sI = rdr.ReadUInt32();
                sU = rdr.ReadUInt32();
                entry = rdr.ReadUInt32();
                bc = rdr.ReadUInt32();
                if (Magic != ExecutableType.PE32Plus)
                    bd = rdr.ReadUInt32();
            }

            public override void Write(PeWriter wtr)
            {
                wtr.SetPosition(hdr.StandardFieldsOffset);
                wtr.Write((ushort)hdr.t);
                wtr.Write(maL);
                wtr.Write(miL);
                wtr.Write(sC);
                wtr.Write(sI);
                wtr.Write(sU);
                wtr.Write(entry);
                wtr.Write(bc);
                if (Magic != ExecutableType.PE32Plus)
                    wtr.Write(bd);
            }
        }
        public class WindowsSpecificFieldsHeader : PeComponent
        {
            public enum WindowsSubsystem
            {
                Unknown = 0,
                Native = 1,
                Windows_GUI = 2,
                Windows_CUI = 3,
                Posix_CUI = 7,
                Windows_CE_GUI = 9,
                EFI_Application = 10,
                EFI_Boot_Service_Driver = 11,
                EFI_Runtime_Driver = 12,
                EFI_ROM = 13,
                XBOX = 14
            }
            [Flags]
            public enum DLLCharacteristics
            {
                Reserved0 = 0x0001,
                Reserved1 = 0x0002,
                Reserved2 = 0x0004,
                Reserved3 = 0x0008,
                Dynamic_Base = 0x0040,
                Force_Integrity_Check = 0x0080,
                NX_Compatible = 0x0100,
                No_Isolation = 0x0200,
                No_Structured_Exception_Handler = 0x0400,
                No_Bind = 0x0800,
                Reserved4 = 0x1000,
                WDM_Driver = 0x2000,
                Terminal_Server_Aware = 0x8000
            }


            OptionalHeader hdr;
            internal WindowsSpecificFieldsHeader(PeFile file, OptionalHeader hdr)
                : base(file)
            {
                this.hdr = hdr;
            }

            private ulong imgBas;
            public ulong ImageBase
            {
                get
                {
                    return imgBas;
                }
                set
                {
                    imgBas = value;
                }
            }

            private uint sectA;
            public uint SectionAlignment
            {
                get
                {
                    return sectA;
                }
                set
                {
                    sectA = value;
                }
            }

            private uint fA;
            public uint FileAlignment
            {
                get
                {
                    return fA;
                }
                set
                {
                    fA = value;
                }
            }

            private ushort maOs;
            public ushort MajorOperatingSystemVersion
            {
                get
                {
                    return maOs;
                }
                set
                {
                    maOs = value;
                }
            }

            private ushort miOs;
            public ushort MinorOperatingSystemVersion
            {
                get
                {
                    return miOs;
                }
                set
                {
                    miOs = value;
                }
            }

            private ushort maImg;
            public ushort MajorImageVersion
            {
                get
                {
                    return maImg;
                }
                set
                {
                    maImg = value;
                }
            }

            private ushort miImg;
            public ushort MinorImageVersion
            {
                get
                {
                    return miImg;
                }
                set
                {
                    miImg = value;
                }
            }

            private ushort maSs;
            public ushort MajorSubsystemVersion
            {
                get
                {
                    return maSs;
                }
                set
                {
                    maSs = value;
                }
            }

            private ushort miSs;
            public ushort MinorSubsystemVersion
            {
                get
                {
                    return miSs;
                }
                set
                {
                    miSs = value;
                }
            }

            private uint winVer;
            public uint Win32VersionValue
            {
                get
                {
                    return winVer;
                }
                set
                {
                    winVer = value;
                }
            }

            private uint sImg;
            public uint SizeOfImage
            {
                get
                {
                    return sImg;
                }
                set
                {
                    sImg = value;
                }
            }

            private uint sHdr;
            public uint SizeOfHeaders
            {
                get
                {
                    return sHdr;
                }
                set
                {
                    sHdr = value;
                }
            }

            private uint cs;
            public uint CheckSum
            {
                get
                {
                    return cs;
                }
                set
                {
                    cs = value;
                }
            }

            private WindowsSubsystem Ss;
            public WindowsSubsystem Subsystem
            {
                get
                {
                    return Ss;
                }
                set
                {
                    Ss = value;
                }
            }

            private DLLCharacteristics dll;
            public DLLCharacteristics DllCharacteristics
            {
                get
                {
                    return dll;
                }
                set
                {
                    dll = value;
                }
            }

            private ulong sSr;
            public ulong SizeOfStackReserve
            {
                get
                {
                    return sSr;
                }
                set
                {
                    sSr = value;
                }
            }

            private ulong sSc;
            public ulong SizeOfStackCommit
            {
                get
                {
                    return sSc;
                }
                set
                {
                    sSc = value;
                }
            }

            private ulong sHr;
            public ulong SizeOfHeapReserve
            {
                get
                {
                    return sHr;
                }
                set
                {
                    sHr = value;
                }
            }

            private ulong sHc;
            public ulong SizeOfHeapCommit
            {
                get
                {
                    return sHc;
                }
                set
                {
                    sHc = value;
                }
            }

            private uint ldrF;
            public uint LoaderFlags
            {
                get
                {
                    return ldrF;
                }
                set
                {
                    ldrF = value;
                }
            }

            private uint noDd;
            public uint NumberOfRvaAndSizes
            {
                get
                {
                    return noDd;
                }
                set
                {
                    noDd = value;
                }
            }

            public override void Read(PeReader rdr)
            {
                rdr.SetPosition(hdr.WindowsSpecificFieldsOffset);
                if (hdr.Type == ExecutableType.PE32Plus)
                {
                    imgBas = rdr.ReadUInt64();
                    sectA = rdr.ReadUInt32();
                    fA = rdr.ReadUInt32();
                    maOs = rdr.ReadUInt16();
                    miOs = rdr.ReadUInt16();
                    maImg = rdr.ReadUInt16();
                    miImg = rdr.ReadUInt16();
                    maSs = rdr.ReadUInt16();
                    miSs = rdr.ReadUInt16();
                    winVer = rdr.ReadUInt32();
                    sImg = rdr.ReadUInt32();
                    sHdr = rdr.ReadUInt32();
                    cs = rdr.ReadUInt32();
                    Ss = (WindowsSubsystem)rdr.ReadUInt16();
                    dll = (DLLCharacteristics)rdr.ReadUInt16();
                    sSr = rdr.ReadUInt64();
                    sSc = rdr.ReadUInt64();
                    sHr = rdr.ReadUInt64();
                    sHc = rdr.ReadUInt64();
                    ldrF = rdr.ReadUInt32();
                    noDd = rdr.ReadUInt32();
                }
                else
                {
                    imgBas = rdr.ReadUInt32();
                    sectA = rdr.ReadUInt32();
                    fA = rdr.ReadUInt32();
                    maOs = rdr.ReadUInt16();
                    miOs = rdr.ReadUInt16();
                    maImg = rdr.ReadUInt16();
                    miImg = rdr.ReadUInt16();
                    maSs = rdr.ReadUInt16();
                    miSs = rdr.ReadUInt16();
                    winVer = rdr.ReadUInt32();
                    sImg = rdr.ReadUInt32();
                    sHdr = rdr.ReadUInt32();
                    cs = rdr.ReadUInt32();
                    Ss = (WindowsSubsystem)rdr.ReadUInt16();
                    dll = (DLLCharacteristics)rdr.ReadUInt16();
                    sSr = rdr.ReadUInt32();
                    sSc = rdr.ReadUInt32();
                    sHr = rdr.ReadUInt32();
                    sHc = rdr.ReadUInt32();
                    ldrF = rdr.ReadUInt32();
                    noDd = rdr.ReadUInt32();
                }
            }

            public override void Write(PeWriter wtr)
            {
                wtr.SetPosition(hdr.WindowsSpecificFieldsOffset);
                if (hdr.Type == ExecutableType.PE32Plus)
                {
                    wtr.Write((ulong)imgBas);
                    wtr.Write((uint)sectA);
                    wtr.Write((uint)fA);
                    wtr.Write((ushort)maOs);
                    wtr.Write((ushort)miOs);
                    wtr.Write((ushort)maImg);
                    wtr.Write((ushort)miImg);
                    wtr.Write((ushort)maSs);
                    wtr.Write((ushort)miSs);
                    wtr.Write((uint)winVer);
                    wtr.Write((uint)sImg);
                    wtr.Write((uint)sHdr);
                    wtr.Write((uint)cs);
                    wtr.Write((ushort)Ss);
                    wtr.Write((ushort)dll);
                    wtr.Write((ulong)sSr);
                    wtr.Write((ulong)sSc);
                    wtr.Write((ulong)sHr);
                    wtr.Write((ulong)sHc);
                    wtr.Write((uint)ldrF);
                    wtr.Write((uint)noDd);
                }
                else
                {
                    wtr.Write((uint)imgBas);
                    wtr.Write((uint)sectA);
                    wtr.Write((uint)fA);
                    wtr.Write((ushort)maOs);
                    wtr.Write((ushort)miOs);
                    wtr.Write((ushort)maImg);
                    wtr.Write((ushort)miImg);
                    wtr.Write((ushort)maSs);
                    wtr.Write((ushort)miSs);
                    wtr.Write((uint)winVer);
                    wtr.Write((uint)sImg);
                    wtr.Write((uint)sHdr);
                    wtr.Write((uint)cs);
                    wtr.Write((ushort)Ss);
                    wtr.Write((ushort)dll);
                    wtr.Write((uint)sSr);
                    wtr.Write((uint)sSc);
                    wtr.Write((uint)sHr);
                    wtr.Write((uint)sHc);
                    wtr.Write((uint)ldrF);
                    wtr.Write((uint)noDd);
                }
            }
        }
        public class DataDirectoriesHeader : Collection<DataDirectoryEntry>
        {
            OptionalHeader hdr;
            internal DataDirectoriesHeader(OptionalHeader hdr)
            {
                this.hdr = hdr;
            }
            public DataDirectoryEntry this[DataDirectoryType idx]
            {
                get
                {
                    return Items[(int)idx];
                }
                set
                {
                    Items[(int)idx] = value;
                }
            }

            public void Read(PeReader rdr)
            {
                rdr.SetPosition(hdr.DataDirectoriesOffset);
                for (int i = 0; i < 16; i++)
                {
                    Items.Add(new DataDirectoryEntry((DataDirectoryType)i, rdr.ReadUInt32(), rdr.ReadUInt32()));
                }
            }

            public void Write(PeWriter wtr)
            {
                wtr.SetPosition(hdr.DataDirectoriesOffset);
                for (int i = 0; i < 16; i++)
                {
                    wtr.Write(Items[i].Address);
                    wtr.Write(Items[i].Size);
                }
            }
        }

        private StandardFieldsHeader sf;
        private WindowsSpecificFieldsHeader wsf;
        private DataDirectoriesHeader dd;
        public StandardFieldsHeader StandardFields { get { return sf; } }
        public WindowsSpecificFieldsHeader WindowsSpecificFields { get { return wsf; } }
        public DataDirectoriesHeader DataDirectories { get { return dd; } }

        public uint StandardFieldsOffset
        {
            get
            {
                return file.PEHeader.PEHeaderOffset + 20;
            }
        }
        public uint WindowsSpecificFieldsOffset
        {
            get
            {
                return StandardFieldsOffset + (Type == ExecutableType.PE32Plus ? 24U : 28U);
            }
        }
        public uint DataDirectoriesOffset
        {
            get
            {
                return WindowsSpecificFieldsOffset + (Type == ExecutableType.PE32Plus ? 84U : 68U);
            }
        }

        public void Read(PeReader rdr)
        {
            rdr.SetPosition(StandardFieldsOffset);
            t = (ExecutableType)rdr.ReadUInt16();
            sf = new StandardFieldsHeader(file, this);
            sf.Read(rdr);
            wsf = new WindowsSpecificFieldsHeader(file, this);
            wsf.Read(rdr);
            dd = new DataDirectoriesHeader(this);
            dd.Read(rdr);
        }

        public void Write(PeWriter wtr)
        {
            wtr.SetPosition(StandardFieldsOffset);
            sf.Write(wtr);
            wsf.Write(wtr);
            dd.Write(wtr);
        }
    }
}
