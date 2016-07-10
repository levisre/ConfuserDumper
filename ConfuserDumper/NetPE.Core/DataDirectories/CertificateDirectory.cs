using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Pe;
using System.IO;

namespace NetPE.Core.DataDirectories
{
    public class CertificateDirectory : DataDirectory, IList<CertificateDirectory.CertificateEntry>
    {
        public CertificateDirectory(DataDirectoryEntry entry) : base(entry) { }

        public abstract class Certificate
        {
            CertificateEntry entry;
            public Certificate(CertificateEntry entry) { this.entry = entry; }

            public CertificateEntry Entry { get { return entry; } set { entry = value; } }

            public abstract uint GetSize();

            public abstract void Load(byte[] dat);

            public abstract void Save(out byte[] dat);
        }

        public class DefaultCertificate : Certificate
        {
            public DefaultCertificate(CertificateEntry entry) : base(entry) { }

            byte[] dat;

            public byte[] Data { get { return dat; } set { dat = value; } }

            public override uint GetSize()
            {
                return (uint)dat.Length;
            }

            public override void Load(byte[] dat)
            {
                this.dat = dat;
            }

            public override void Save(out byte[] dat)
            {
                dat = this.dat;
            }
        }

        public enum CertificateRevision
        {
            Revision_1_0 = 0x0100,
            Revision_2_0 = 0x0200
        }
        public enum CertificateType
        {
            X509=0x0001,
            PKCS_Signed_Data=0x0002,
            Reserved=0x0003,
            Ts_Stack_Signed=0x0004
        }
        public class CertificateEntry
        {
            CertificateRevision ver;
            public CertificateRevision Revision { get { return ver; } set { ver = value; } }
            CertificateType t;
            public CertificateType CertificateType { get { return t; } set { t = value; } }
            byte[] dat;
            public byte[] CertificateData { get { return dat; } set { dat = value; } }

            public Certificate LoadCertificate()
            {
                Certificate ret;
                switch (t)
                {
                    default:
                        ret = new DefaultCertificate(this);
                        ret.Load(dat);
                        break;
                }
                return ret;
            }
        }

        public override DataDirectoryType Type
        {
            get { return DataDirectoryType.Certificate; }
        }

        public override uint GetTotalSize()
        {
            uint ret = 0;
            foreach (CertificateEntry e in items)
            {
                ret += 8 + (uint)e.CertificateData.Length + (uint)(e.CertificateData.Length % 8);
            }
            return ret;
        }

        public override uint GetDirectorySize()
        {
            uint ret = 0;
            foreach (CertificateEntry e in items)
            {
                ret += 8 + (uint)e.CertificateData.Length + (uint)(e.CertificateData.Length % 8);
            }
            return ret;
        }

        protected override void SaveInternal(VirtualWriter wtr)
        {
            //
        }

        protected override void LoadInternal(VirtualReader rdr)
        {
            //
        }

        public void Save(PeWriter wtr, uint adr)
        {
            wtr.SetPosition(adr);
            foreach (CertificateEntry i in items)
            {
                wtr.Write((uint)i.CertificateData.Length + 8);
                wtr.Write((ushort)i.Revision);
                wtr.Write((ushort)i.CertificateType);
                wtr.Write(i.CertificateData);
            }
        }

        public void Load(PeReader rdr, uint adr)
        {
            rdr.SetPosition(adr);
            uint s = Location.Size;
            while (rdr.BaseStream.Position - adr < s)
            {
                CertificateEntry e = new CertificateEntry();
                uint len = rdr.ReadUInt32();
                e.Revision = (CertificateRevision)rdr.ReadUInt16();
                e.CertificateType = (CertificateType)rdr.ReadUInt16();
                e.CertificateData = rdr.ReadBytes((int)len - 8);
                items.Add(e);
            }
        }

        
        List<CertificateEntry> items = new List<CertificateEntry>();
        public void Add(CertificateEntry item)
        {
            items.Add(item);
        }
        public void Clear()
        {
            items.Clear();
        }
        public bool Contains(CertificateEntry item)
        {
            return items.Contains(item);
        }
        public void CopyTo(CertificateEntry[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }
        public int Count
        {
            get { return items.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        public bool Remove(CertificateEntry item)
        {
            return items.Remove(item);
        }
        public IEnumerator<CertificateEntry> GetEnumerator()
        {
            return items.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public int IndexOf(CertificateEntry item)
        {
            return items.IndexOf(item);
        }
        public void Insert(int index, CertificateEntry item)
        {
            items.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }
        public CertificateEntry this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }
    }
}
