using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using NetPE.Core.Pe;

namespace NetPE.Core.DataDirectories
{
    public class RelocationDirectory : DataDirectory, IList<RelocationDirectory.RelocationBlock>
    {
        public RelocationDirectory(DataDirectoryEntry entry) : base(entry) { }

        public class RelocationBlock : Collection<RelocationDirectory.RelocationEntry>
        {
            private Rva pr;
            public Rva PageRva { get { return pr; } set { pr = value; } }
            private uint s;
            public uint BlockSize { get { return s; } set { s = value; } }

            public uint GetSize()
            {
                return (uint)(8 + this.Items.Count * 2);
            }

            internal void Load(VirtualReader rdr)
            {
                pr = rdr.ReadRva();
                s = rdr.ReadUInt32();
                uint c = (s - 8) / 2;
                for (int i = 0; i < c; i++)
                {
                    RelocationEntry e = new RelocationEntry();
                    ushort item = rdr.ReadUInt16();
                    e.Type = (RelocationType)((item & 0xf000U) >> 12);
                    e.Offest = (item & 0x0fffU) + pr;
                    this.Add(e);
                }
            }

            internal void Save(VirtualWriter wtr)
            {
                wtr.Write(pr);
                wtr.Write(s);
                foreach (RelocationEntry i in Items)
                {
                    wtr.Write((ushort)(((uint)i.Type << 12) | (i.Offest - pr)));
                }
            }

            public override string ToString()
            {
                return pr.ToString();
            }
        }

        [Flags]
        public enum RelocationType
        {
            Absolute = 0,
            High = 1,
            Low = 2,
            HighLow = 3,
            HighAdj = 4,
            MIPS_JumpAddress = 5,
            Reserved0 = 6,
            Reserved1 = 7,
            MIPS_JumpAddress16 = 9,
            Difference_64Bit = 10
        }

        public class RelocationEntry
        {
            RelocationType t;
            Rva r;
            public RelocationType Type { get { return t; } set { t = value; } }
            public Rva Offest { get { return r; } set { r = value; } }

            public override string ToString()
            {
                return r.ToString();
            }
        }

        public override DataDirectoryType Type
        {
            get { return DataDirectoryType.Relocation; }
        }

        public override uint GetTotalSize()
        {
            uint ret = 0;
            foreach (RelocationBlock b in bs)
            {
                ret += b.GetSize();
            }
            return ret;
        }

        public override uint GetDirectorySize()
        {
            uint ret = 0;
            foreach (RelocationBlock b in bs)
            {
                ret += b.GetSize();
            }
            return ret;
        }

        protected override void SaveInternal(VirtualWriter wtr)
        {
            foreach (RelocationBlock i in bs)
            {
                i.Save(wtr);
            }
        }

        protected override void LoadInternal(VirtualReader rdr)
        {
            for (; rdr.BaseStream.Position < Location.Size + Location.Address; )
            {
                RelocationBlock b = new RelocationBlock();
                b.Load(rdr);
                bs.Add(b);
            }
        }

        List<RelocationBlock> bs = new List<RelocationBlock>();
        public int IndexOf(RelocationBlock item)
        {
            return bs.IndexOf(item);
        }
        public void Insert(int index, RelocationBlock item)
        {
            bs.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            bs.RemoveAt(index);
        }
        public RelocationBlock this[int index]
        {
            get
            {
                return bs[index];
            }
            set
            {
                bs[index] = value;
            }
        }
        public void Add(RelocationBlock item)
        {
            bs.Add(item);
        }
        public void Clear()
        {
            bs.Clear();
        }
        public bool Contains(RelocationBlock item)
        {
            return bs.Contains(item);
        }
        public void CopyTo(RelocationBlock[] array, int arrayIndex)
        {
            bs.CopyTo(array, arrayIndex);
        }
        public int Count
        {
            get { return bs.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        public bool Remove(RelocationBlock item)
        {
            return bs.Remove(item);
        }
        public IEnumerator<RelocationBlock> GetEnumerator()
        {
            return bs.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
