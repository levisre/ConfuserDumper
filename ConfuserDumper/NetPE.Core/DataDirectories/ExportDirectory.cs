using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Pe;
using System.Collections.ObjectModel;

namespace NetPE.Core.DataDirectories
{
    public class ExportDirectory : DataDirectory
    {
        public enum ExportAddressType
        {
            RealExport,
            ForwarderExport
        }
        public class ExportAddressEntry
        {
            Rva rva;
            public Rva Address { get {  return rva; } set { rva = value; } }
            string fdr = "";
            public string Forwarder { get { return fdr; } set { fdr = value; } }
            ExportAddressType t;
            public ExportAddressType Type { get { return t; } set { t = value; } }

            public override string ToString()
            {
                return t == ExportAddressType.ForwarderExport ? fdr : rva.ToString();
            }
        }

        public class ExportAddressTable : Collection<ExportAddressEntry>
        {
            ExportDirectory dd;
            public ExportAddressTable(ExportDirectory dd)
            {
                this.dd = dd;
            }

            public uint GetPointersSize()
            {
                return (uint)Items.Count * 4;
            }

            public uint GetForwardersSize()
            {
                uint ret = 0;
                foreach (ExportAddressEntry i in Items)
                {
                    if (i.Type == ExportAddressType.ForwarderExport)
                        ret += (uint)i.Forwarder.Length + 1;
                }
                return ret;
            }

            internal void Load(VirtualReader rdr)
            {
                for (int i = 0; i < dd.AddressTableEntries; i++)
                {
                    ExportAddressEntry d = new ExportAddressEntry();
                    Rva rva = rdr.ReadRva();
                    d.Address = rva;
                    if (!(rva > dd.Location.Address && rva < dd.Location.Address + dd.Location.Size))
                    {
                        d.Type = ExportAddressType.RealExport;
                        d.Forwarder = "";
                    }
                    else
                    {
                        d.Type = ExportAddressType.ForwarderExport;
                        rdr.SaveLocation();
                        rdr.SetPosition(rva);
                        d.Forwarder = rdr.ReadString();
                        rdr.LoadLocation();
                    }
                    this.Items.Add(d);
                }
            }

            internal void Save(VirtualWriter wtr)
            {
                Rva fdrAdr = (Rva)wtr.GetPosition() + GetPointersSize();
                foreach (ExportAddressEntry i in Items)
                {
                    if (i.Type == ExportAddressType.RealExport)
                    {
                        wtr.Write(i.Address);
                    }
                    else
                    {
                        wtr.Write(fdrAdr);
                        fdrAdr += (uint)i.Forwarder.Length + 1;
                        wtr.SaveLocation();
                        wtr.SetPosition(fdrAdr);
                        wtr.Write(i.Forwarder);
                        wtr.LoadLocation();
                    }
                }
            }
        }

        public class ExportNameTable : Collection<string>
        {
            ExportDirectory dd;
            public ExportNameTable(ExportDirectory dd)
            {
                this.dd = dd;
            }

            public uint GetPointersSize()
            {
                return (uint)Items.Count * 4;
            }

            public uint GetDataSize()
            {
                uint ret = 0;
                foreach (string i in this)
                {
                    ret += (uint)(i.Length + 1);
                }
                return ret;
            }

            internal void Load(VirtualReader rdr)
            {
                for (int i = 0; i < dd.NumberOfNamePointers; i++)
                {
                    Rva name = rdr.ReadRva();
                    rdr.SaveLocation();
                    rdr.BaseStream.Position = name;
                    this.Add(rdr.ReadString());
                    rdr.LoadLocation();
                }
            }

            internal void Save(VirtualWriter wtr)
            {
                foreach (string i in Items)
                {
                    wtr.Write(i);
                }
            }
        }

        public class ExportOrdinalTable : Collection<ushort>
        {
            ExportDirectory dd;
            public ExportOrdinalTable(ExportDirectory dd)
            {
                this.dd = dd;
            }

            public uint GetSize()
            {
                return (uint)this.Items.Count * 2;
            }

            internal void Load(VirtualReader rdr)
            {
                for (int i = 0; i < dd.NumberOfNamePointers; i++)
                {
                    Items.Add(rdr.ReadUInt16());
                }
            }

            internal void Save(VirtualWriter wtr)
            {
                foreach (ushort i in Items)
                {
                    wtr.Write(i);
                }
            }
        }

        private uint f;
        public uint ExportFlags { get { return f; } set { f = value; } }

        private DateTime stamp;
        public DateTime DateTimeStamp { get { return stamp; } set { stamp = value; } }

        private ushort maVer;
        public ushort MajorVersion { get { return maVer; } set { maVer = value; } }

        private ushort miVer;
        public ushort MinorVersion { get { return miVer; } set { miVer = value; } }

        private string n;
        public string Name { get { return n; } set { n = value; } }

        private uint bas;
        public uint OrdinalBase { get { return bas; } set { bas = value; } }

        private uint noAdr;
        public uint AddressTableEntries { get { return noAdr; } set { noAdr = value; } }

        private uint noName;
        public uint NumberOfNamePointers { get { return noName; } set { noName = value; } }

        private ExportAddressTable adr;
        public ExportAddressTable AddressTable { get { return adr; } }

        private ExportNameTable nt;
        public ExportNameTable NameTable { get { return nt; } }

        private ExportOrdinalTable ot;
        public ExportOrdinalTable OrdinalTable { get { return ot; } }


        public override DataDirectoryType Type { get { return DataDirectoryType.Export; } }

        public ExportDirectory(DataDirectoryEntry entry) : base(entry) { }

        public override uint GetTotalSize()
        {
            return 40 + adr.GetPointersSize() + adr.GetForwardersSize() + nt.GetPointersSize() + nt.GetDataSize() + ot.GetSize();
        }

        public override uint GetDirectorySize()
        {
            return 40 + adr.GetPointersSize() + adr.GetForwardersSize() + nt.GetPointersSize() + nt.GetDataSize() + ot.GetSize();
        }

        protected override void SaveInternal(VirtualWriter wtr)
        {
            Rva baseAdr = wtr.GetPosition();
            wtr.Write(f);
            wtr.WriteStamp(stamp);
            wtr.Write((ushort)maVer);
            wtr.Write((ushort)miVer);
            wtr.Write(baseAdr + 40);
            wtr.Write((uint)bas);
            wtr.Write((uint)noAdr);
            wtr.Write((uint)noName);
            Rva adrsAdr = (Rva)(baseAdr + 40 + n.Length + 1);
            Rva nptAdr = adrsAdr + adr.GetPointersSize() + adr.GetForwardersSize();
            Rva otAdr = nptAdr + nt.GetPointersSize() + nt.GetDataSize();
            wtr.Write(adrsAdr);
            wtr.Write(nptAdr);
            wtr.Write(otAdr);

            wtr.SetPosition(baseAdr + 40);
            wtr.Write(n);

            wtr.SetPosition(adrsAdr);
            adr.Save(wtr);

            wtr.SetPosition(nptAdr);
            uint nameOffset = nt.GetPointersSize();
            foreach (string str in nt)
            {
                wtr.Write(nptAdr + nameOffset);
                nameOffset += (uint)str.Length + 1;
            }
            nt.Save(wtr);

            wtr.SetPosition(otAdr);
            ot.Save(wtr);
        }

        protected override void LoadInternal(VirtualReader rdr)
        {
            f = rdr.ReadUInt32();
            stamp = rdr.ReadStamp();
            maVer = rdr.ReadUInt16();
            miVer = rdr.ReadUInt16();

            Rva nr = rdr.ReadRva();

            bas = rdr.ReadUInt32();
            noAdr = rdr.ReadUInt32();
            noName = rdr.ReadUInt32();
            Rva adrPtr = rdr.ReadRva();
            Rva ntPtr = rdr.ReadRva();
            Rva otPtr = rdr.ReadRva();

            rdr.SaveLocation();
            rdr.SetPosition(nr);
            n = rdr.ReadString();
            rdr.LoadLocation();

            rdr.SaveLocation();
            rdr.SetPosition(adrPtr);
            adr = new ExportAddressTable(this);
            adr.Load(rdr);
            rdr.LoadLocation();

            rdr.SaveLocation();
            rdr.SetPosition(ntPtr);
            nt = new ExportNameTable(this);
            nt.Load(rdr);
            rdr.LoadLocation();

            rdr.SaveLocation();
            rdr.SetPosition(otPtr);
            ot = new ExportOrdinalTable(this);
            ot.Load(rdr);
            rdr.LoadLocation();
        }
    }
}
