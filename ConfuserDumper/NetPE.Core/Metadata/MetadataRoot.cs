using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using NetPE.Core.DataDirectories;
using NetPE.Core.Pe;

namespace NetPE.Core.Metadata
{
    public class MetadataRoot : Collection<MetadataStream>
    {
        CLRDirectory d;
        public MetadataRoot(CLRDirectory md) { d = md; }
        public CLRDirectory Directory { get { return d; } }

        uint sig;
        public uint Signature { get { return sig; } set { sig = value; } }
        ushort maVer;
        public ushort MajorVersion { get { return maVer; } set { maVer = value; } }
        ushort miVer;
        public ushort MinorVersion { get { return miVer; } set { miVer = value; } }
        uint res;
        public uint Reserved { get { return res; } set { res = value; } }
        string ver;
        public string Version { get { return ver; } set { ver = value; } }
        ushort f;
        public ushort Flags { get { return f; } set { f = value; } }

        public uint GetSize()
        {
            uint ret = (uint)(20 + ((ver.Length + 3) & ~3) + this.Count * 8);
            foreach (MetadataStream str in Items)
            {
                ret += (uint)((str.Name.Length + 1 + 3) & ~3);
                ret += (uint)((str.Length + 3) & ~3);
            }
            return ret;
        }

        public void Load(VirtualReader rdr, Rva adr)
        {
            rdr.SetPosition(adr);
            sig = rdr.ReadUInt32();
            maVer = rdr.ReadUInt16();
            miVer = rdr.ReadUInt16();
            res = rdr.ReadUInt32();
            uint len = rdr.ReadUInt32();
            ver = new string(rdr.ReadChars((int)len)).Trim('\0');
            f = rdr.ReadUInt16();
            ushort s = rdr.ReadUInt16();
            for (int i = 0; i < s; i++)
            {
                MetadataStream str = new MetadataStream(this);
                Rva datAdr = adr + rdr.ReadUInt32();
                Rva datSze = rdr.ReadUInt32();
                str.Name = "";
                str.File = rdr.BaseStream.File;
                for (int ii = 0; ii < 32; ii++)
                {
                    byte[] bs = rdr.ReadBytes(4);
                    foreach (byte b in bs)
                        if (b != 0) str.Name += (char)b; else break;
                    if (bs[3] == '\0') break;
                }
                if (str.Type != MetadataStreamType.Unknown) tStrs[str.Type] = str;
                nStrs[str.Name] = str;
                rdr.SaveLocation();
                rdr.SetPosition(datAdr);
                str.Data = rdr.ReadBytes((int)datSze.Value);
                rdr.LoadLocation();
                Items.Add(str);
            }
            foreach (MetadataStream i in Items)
            {
                i.Load();
            }
        }

        public void Save(VirtualWriter wtr, Rva adr)
        {
            wtr.SetPosition(adr);
            wtr.Write(sig);
            wtr.Write(maVer);
            wtr.Write(miVer);
            wtr.Write(res);
            wtr.Write((ver.Length + 3) & ~3);
            wtr.Write(Encoding.ASCII.GetBytes(ver));
            wtr.Write(new byte[((ver.Length + 1 + 3) & ~3) - ver.Length]);
            wtr.Write(f);
            wtr.Write((ushort)Items.Count);
            uint datOffset = (uint)(20 + ((ver.Length + 3) & ~3) + this.Count * 8);
            foreach (MetadataStream str in Items) datOffset += (uint)((str.Name.Length + 1 + 3) & ~3);
            foreach (MetadataStream str in Items)
            {
                wtr.Write(datOffset);
                datOffset += (uint)((str.Length + 3) & ~3);
                wtr.Write((uint)((str.Length + 3) & ~3));
                wtr.Write(Encoding.ASCII.GetBytes(str.Name));
                wtr.Write(new byte[((str.Name.Length + 1 + 3) & ~3) - str.Name.Length]);
            }
            foreach (MetadataStream str in Items)
            {
                wtr.Write(str.Data);
                wtr.Write(new byte[((str.Length + 3) & ~3) - str.Length]);
            }
        }

        Dictionary<MetadataStreamType, MetadataStream> tStrs = new Dictionary<MetadataStreamType, MetadataStream>();
        public MetadataStream this[MetadataStreamType t] { get { if (!tStrs.ContainsKey(t))return null; else return tStrs[t]; } }

        Dictionary<string, MetadataStream> nStrs = new Dictionary<string, MetadataStream>();
        public MetadataStream this[string name] { get { if (!nStrs.ContainsKey(name))return null; else return nStrs[name]; } }

        protected override void ClearItems()
        {
            base.ClearItems();
            tStrs.Clear();
            nStrs.Clear();
        }
        protected override void InsertItem(int index, MetadataStream item)
        {
            base.InsertItem(index, item);
            if (item.Type != MetadataStreamType.Unknown) tStrs[item.Type] = item;
            nStrs[item.Name] = item;
        }
        protected override void RemoveItem(int index)
        {
            MetadataStream item = this[index];
            base.RemoveItem(index);
            if (item.Type != MetadataStreamType.Unknown) tStrs.Remove(item.Type);
            nStrs.Remove(item.Name);
        }
        protected override void SetItem(int index, MetadataStream item)
        {
            MetadataStream old = this[index];
            if (old.Type != MetadataStreamType.Unknown) tStrs.Remove(old.Type);
            nStrs.Remove(old.Name);
            base.SetItem(index, item);
            if (item.Type != MetadataStreamType.Unknown) tStrs[item.Type] = item;
            nStrs[item.Name] = item;
        }
    }
}
