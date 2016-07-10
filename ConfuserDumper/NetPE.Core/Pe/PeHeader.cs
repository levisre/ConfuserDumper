using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core.Pe
{
    public class PEHeader : PeComponent
    {
        public enum MachineType
        {
            Unknown = 0x0,
            AM33 = 0x1d3,
            x64 = 0x8664,
            ARM = 0x1c0,
            EFI = 0xebc,
            I386 = 0x14c,
            IA64 = 0x200,
            M32R = 0x9041,
            MIPS_16 = 0x266,
            MIPS_FPU = 0x366,
            MIPS_16_FPU = 0x466,
            PowerPC = 0x1f0,
            PowerPC_FP = 0x1f1,
            R4000 = 0x166,
            SH3 = 0x1a2,
            SH3_DSP = 0x1a3,
            SH4 = 0x1a6,
            SH5 = 0x1a8,
            Thumb = 0x1c2,
            WCE_MIPS_v2 = 0x169
        }
        [Flags]
        public enum PeCharacteristics
        {
            Relocations_Stripped = 0x0001,
            Executable_Image = 0x0002,
            Line_Numbers_Stripped = 0x0004,
            Local_Symbols_Stripped = 0x0008,
            Aggressively_Trim_Working_Set = 0x0010,
            Large_Address_Aware = 0x0020,
            Reserved = 0x0040,
            Bytes_Reversed_Low = 0x0080,
            Bit32_Machine = 0x0100,
            Debugging_Information_Stripped = 0x0200,
            Removable_Run_From_Swap = 0x0400,
            Network_Run_From_Swap = 0x0800,
            System_File = 0x1000,
            DLL_Image = 0x2000,
            Uniprocessor_System_Only = 0x4000,
            Bytes_Reversed_High = 0x8000
        }

        internal PEHeader(PeFile file) : base(file) { }

        public uint PEHeaderOffset
        {
            get
            {
                if (File.Type == PeFileType.Image)
                {
                    return File.DOSHeader.PEHeaderOffset + 4;
                }
                else
                {
                    return 0;
                }
            }
        }

        private MachineType m;
        public MachineType Machine
        {
            get
            {
                return m;
            }
            set
            {
                m = value;
            }
        }

        private ushort noSect;
        public ushort NumberOfSections
        {
            get
            {
                return noSect;
            }
            set
            {
                noSect = value;
            }
        }

        private DateTime stamp;
        public DateTime TimeDateStamp
        {
            get
            {
                return stamp;
            }
            set
            {
                stamp = value;
            }
        }

        private uint ptrSym;
        public uint PointerToSymbolTable
        {
            get
            {
                return ptrSym;
            }
            set
            {
                ptrSym = value;
            }
        }

        private uint noSym;
        public uint NumberOfSymbols
        {
            get
            {
                return noSym;
            }
            set
            {
                noSym = value;
            }
        }

        private ushort sOh;
        public ushort SizeOfOptionalHeader
        {
            get
            {
                return sOh;
            }
            set
            {
                sOh = value;
            }
        }

        private PeCharacteristics c;
        public PeCharacteristics Characteristics
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

        public override void Read(PeReader rdr)
        {
            rdr.SetPosition(PEHeaderOffset);
            m = (MachineType)rdr.ReadUInt16();
            noSect = rdr.ReadUInt16();
            stamp = rdr.ReadStamp();
            ptrSym = rdr.ReadUInt32();
            noSym = rdr.ReadUInt32();
            sOh = rdr.ReadUInt16();
            c = (PeCharacteristics)rdr.ReadUInt16();
        }

        public override void Write(PeWriter wtr)
        {
            wtr.SetPosition(PEHeaderOffset);
            wtr.Write((ushort)m);
            wtr.Write((ushort)noSect);
            wtr.WriteStamp(stamp);
            wtr.Write((uint)ptrSym);
            wtr.Write((uint)noSym);
            wtr.Write((ushort)sOh);
            wtr.Write((ushort)c);
        }
    }
}
