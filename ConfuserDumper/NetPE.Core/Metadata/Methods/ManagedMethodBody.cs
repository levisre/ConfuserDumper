using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetPE.Core.Metadata.Tables;

namespace NetPE.Core.Metadata.Methods
{
    public class ManagedMethodBody : MethodBody
    {
        public ManagedMethodBody(MetadataRow par) : base(par) { }

        HeaderFlags f;
        public HeaderFlags Flags { get { return f; } set { f = value; } }
        ushort ms;
        public ushort MaxStack { get { return ms; } set { ms = value; } }
        TableToken lvs;
        public TableToken LocalVarSig { get { return lvs; } set { lvs = value; } }
        byte[] c;
        public byte[] Codes { get { return c; } set { c = value; } }
        DataSectionCollection sects;
        public DataSectionCollection DataSections { get { return sects; } }

        public bool IsTinyHeader()
        {
            if (lvs.Token.Index == 0 &&
                sects.Count == 0     &&
                ms <= 8              &&
                c.Length < 64)
                return true;
            else
                return false;
        }

        public override uint Size
        {
            get
            {
                uint ret = (uint)c.Length;
                if (IsTinyHeader())
                    ret += 1;
                else
                    ret += 12;
                if (sects.Count != 0)
                {
                    ret += (uint)(((ret + 3) & ~3) - ret);
                    ret += sects.GetSize();
                }
                return ret;
            }
        }

        public override void Load(BinaryReader rdr)
        {
            byte fmt = rdr.ReadByte();
            switch ((HeaderFlags)(fmt & 0x3))
            {
                case HeaderFlags.TinyFormat:
                    f = HeaderFlags.TinyFormat;
                    ms = 8;
                    lvs = new TableToken(Parent.Container.Heap);
                    lvs.Token = 0;
                    c = rdr.ReadBytes(fmt >> 2);
                    break;
                case HeaderFlags.FatFormat:
                    rdr.BaseStream.Seek(-1, SeekOrigin.Current);
                    ushort ff = rdr.ReadUInt16();
                    f = (HeaderFlags)(ff & 0xfff);
                    if ((ff >> 12) != 3) throw new InvalidOperationException();
                    ms = rdr.ReadUInt16();
                    uint s = rdr.ReadUInt32();
                    lvs = new TableToken(Parent.Container.Heap);
                    lvs.Token = rdr.ReadUInt32();
                    c = rdr.ReadBytes((int)s);
                    break;
            }
            rdr.ReadBytes((int)(((rdr.BaseStream.Position + 3) & ~3) - rdr.BaseStream.Position));
            sects = new DataSectionCollection(Parent);
            if ((f & HeaderFlags.MoreSects) == HeaderFlags.MoreSects)
            {
                sects.Load(rdr);
            }
        }

        public override void Save(BinaryWriter wtr)
        {
            if (IsTinyHeader())
            {
                wtr.Write((byte)((byte)HeaderFlags.TinyFormat) | (c.Length << 2));
                wtr.Write(c);
            }
            else
            {
                wtr.Write((ushort)((byte)f | 3 << 12));
                wtr.Write(ms);
                wtr.Write((uint)c.Length);
                wtr.Write(lvs.Token.Value);
                wtr.Write(c);
                sects.Save(wtr);
            }
        }
    }
}
