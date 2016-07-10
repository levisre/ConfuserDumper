using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core.Pe
{
    public class DOSHeader : PeComponent
    {
        internal DOSHeader(PeFile file) : base(file) { }

        private ushort m;
        public ushort Magic
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

        private byte[] hc;
        public byte[] HeaderContext
        {
            get
            {
                return hc;
            }
            set
            {
                hc = value;
            }
        }

        private uint o;
        public uint PEHeaderOffset
        {
            get
            {
                return o;
            }
            set
            {
                o = value;
            }
        }

        private byte[] ds;
        public byte[] DosStub
        {
            get
            {
                return ds;
            }
            set
            {
                ds = value;
            }
        }


        public override void Read(PeReader rdr)
        {
            rdr.BaseStream.Position = 0;
            m = rdr.ReadUInt16();
            hc = rdr.ReadBytes(0x3a);
            o = rdr.ReadUInt32();
            ds = rdr.ReadBytes((int)(o - 0x40));
        }

        public override void Write(PeWriter wtr)
        {
            wtr.BaseStream.Position = 0;
            wtr.Write((ushort)m);
            wtr.Write(hc);
            wtr.Write((uint)o);
            wtr.Write(ds);
        }
    }
}
