using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using NetPE.Core.DataDirectories;
using NetPE.Core.Pe;

namespace NetPE.Core.Metadata
{
    public class VTableFixups : Collection<VTableFixups.FixupEntry>
    {
        public class VTable : Collection<MetadataToken>, IMetadataData
        {
            FixupEntry e;
            public VTable(FixupEntry en) { e = en; }
            public FixupEntry Directory { get { return e; } }

            public uint GetSize()
            {
                return (uint)Items.Count * 4;
            }

            internal void Write(VirtualWriter wtr)
            {
                foreach (MetadataToken tkn in Items)
                {
                    wtr.Write(tkn);
                }
            }

            public void Load(byte[] data)
            {
                for (int i = 0; i < e.Size; i++)
                {
                    this.Add(BitConverter.ToUInt32(data, i * 4));
                }
            }

            public void Save(out byte[] data)
            {
                data = new byte[this.Count * 4];
                for (int i = 0; i < Items.Count; i++)
                {
                    data[i * 4] = BitConverter.GetBytes(Items[i])[0];
                    data[i * 4 + 1] = BitConverter.GetBytes(Items[i])[1];
                    data[i * 4 + 2] = BitConverter.GetBytes(Items[i])[2];
                    data[i * 4 + 3] = BitConverter.GetBytes(Items[i])[3];
                }
            }
        }

        CLRDirectory d;
        public VTableFixups(CLRDirectory md) { d = md; }
        public CLRDirectory Directory { get { return d; } }

        [Flags]
        public enum FixupType
        {
            Bits32 = 0x01,
            Bits64 = 0x02,
            FromUnmanaged = 0x04,
            FromUnmanagedRetainAppDomain = 0x08,
            CallMostDerived = 0x10
        }
        public class FixupEntry
        {
            Rva vt;
            public Rva VTable { get { return vt; } set { vt = value; } }
            uint s;
            public uint Size { get { return s; } set { s = value; } }
            FixupType t;
            public FixupType Type { get { return t; } set { t = value; } }
        }

        public uint GetSize()
        {
            uint ret = 0;
            foreach (FixupEntry i in Items)
            {
                ret += 8;
            }
            return ret;
        }

        public void Load(VirtualReader rdr, Rva adr, uint size)
        {
            rdr.SetPosition(adr);
            while (rdr.BaseStream.Position - adr < size)
            {
                FixupEntry e = new FixupEntry();
                e.VTable = rdr.ReadRva();
                e.Size = rdr.ReadUInt16();
                e.Type = (FixupType)rdr.ReadUInt16();
                rdr.SaveLocation();
                rdr.SetPosition(e.VTable);
                d.Datas.Add(new CLRData() { Address = e.VTable, Data = rdr.ReadBytes((int)e.Size * 4) });
                rdr.LoadLocation();
                this.Add(e);
            }
        }

        public void Save(VirtualWriter wtr, Rva adr)
        {
            wtr.SetPosition(adr);
            foreach (FixupEntry i in Items)
            {
                wtr.Write(i.VTable);
                wtr.Write((ushort)i.Size);
                wtr.Write((ushort)i.Type);
            }
        }
    }
}
