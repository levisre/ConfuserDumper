using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using NetPE.Core.Pe;

namespace NetPE.Core.DataDirectories
{
    public class ResourceDirectory : DataDirectory, IList<ResourceDirectory.ResourceEntry>
    {
        public ResourceDirectory(DataDirectoryEntry entry) : base(entry) { }

        public class Subdirectory : Collection<ResourceEntry>
        {
            ResourceEntry par;
            public Subdirectory(ResourceEntry parent) { par = parent; }

            public ResourceEntry Parent { get { return par; } }

            uint c;
            public uint Characteristics { get { return c; } set { c = value; } }

            DateTime stamp;
            public DateTime DateTimeStamp { get { return stamp; } set { stamp = value; } }

            ushort maVer;
            public ushort MajorVersion { get { return maVer; } set { maVer = value; } }

            ushort miVer;
            public ushort MinorVersion { get { return miVer; } set { miVer = value; } }

            ushort noName;
            public ushort NumberOfNameEntries { get { return noName; } set { noName = value; } }

            ushort noID;
            public ushort NumberOfIDEntries { get { return noID; } set { noID = value; } }




            public uint GetTotalSize()
            {
                uint ret = GetHeaderSize();
                foreach (ResourceEntry i in Items)
                {
                    if ((i.Type & EntryType.Name) == EntryType.Name)
                        ret += (uint)((i.Name.Length * 2 + 2 + 3) & ~3);
                    if ((i.Type & EntryType.Subdirectory) == EntryType.Subdirectory)
                        ret += (i.Data as Subdirectory).GetTotalSize();
                    else
                    {
                        ret += (i.Data as DataEntry).GetSize();
                        ret += (uint)(((i.Data as DataEntry).Datas.Length + 3) & ~3);
                    }
                }
                return ret;
            }
            public uint GetHeaderSize()
            {
                return 16 + (uint)Items.Count * 8;
            }

            internal void Load(VirtualReader rdr, Rva root)
            {
                c = rdr.ReadUInt32();
                stamp = rdr.ReadStamp();
                maVer = rdr.ReadUInt16();
                miVer = rdr.ReadUInt16();
                noName = rdr.ReadUInt16();
                noID = rdr.ReadUInt16();
                for (int i = 0; i < noName; i++)
                {
                    ResourceEntry e = new ResourceEntry();
                    e.Type = EntryType.Name;
                    Rva nRva = rdr.ReadRva() & ~0x80000000;
                    rdr.SaveLocation();
                    rdr.SetPosition(nRva + root);
                    e.Name = rdr.ReadUnicodeString();
                    rdr.LoadLocation();
                    Rva datAdr = rdr.ReadUInt32();
                    if ((datAdr & 0x80000000) == 0x80000000)
                    {
                        datAdr &= ~0x80000000;
                        e.Type |= EntryType.Subdirectory;
                        e.Data = new Subdirectory(e);
                        rdr.SaveLocation();
                        rdr.SetPosition(datAdr + root);
                        (e.Data as Subdirectory).Load(rdr, root);
                        rdr.LoadLocation();
                    }
                    else
                    {
                        e.Type |= EntryType.DataEntry;
                        e.Data = new DataEntry(e);
                        rdr.SaveLocation();
                        rdr.SetPosition(datAdr + root);
                        (e.Data as DataEntry).Load(rdr);
                        rdr.LoadLocation();
                    }
                    this.Add(e);
                }
                for (int i = 0; i < noID; i++)
                {
                    ResourceEntry e = new ResourceEntry();
                    e.Type = EntryType.ID;
                    e.ID = rdr.ReadUInt32() & ~0x80000000;
                    Rva datAdr = rdr.ReadUInt32();
                    if ((datAdr & 0x80000000) == 0x80000000)
                    {
                        datAdr &= ~0x80000000;
                        e.Type |= EntryType.Subdirectory;
                        e.Data = new Subdirectory(e);
                        rdr.SaveLocation();
                        rdr.SetPosition(datAdr + root);
                        (e.Data as Subdirectory).Load(rdr, root);
                        rdr.LoadLocation();
                    }
                    else
                    {
                        e.Type |= EntryType.DataEntry;
                        e.Data = new DataEntry(e);
                        rdr.SaveLocation();
                        rdr.SetPosition(datAdr + root);
                        (e.Data as DataEntry).Load(rdr);
                        rdr.LoadLocation();
                    }
                    this.Add(e);
                }
            }

            internal void Save(VirtualWriter wtr, ResourceDirectory dir)
            {
                wtr.Write(c);
                wtr.WriteStamp(stamp);
                wtr.Write(maVer);
                wtr.Write(miVer);
                wtr.Write(noName);
                wtr.Write(noID);
                foreach (ResourceEntry i in Items)
                {
                    if ((i.Type & EntryType.Name) == EntryType.Name)
                    {
                        uint strOffset = dir.strO[i.Name];
                        wtr.Write(strOffset | 0x80000000);

                        wtr.SaveLocation();
                        wtr.SetPosition(dir.root + strOffset);
                        wtr.WriteUnicodeString(i.Name);
                        wtr.LoadLocation();
                    }
                    else if ((i.Type & EntryType.ID) == EntryType.ID)
                    {
                        wtr.Write(i.ID);
                    }

                    if ((i.Type & EntryType.Subdirectory) == EntryType.Subdirectory)
                    {
                        uint dirOffset = dir.dirO[i.Data as Subdirectory];
                        wtr.Write(dirOffset | 0x80000000);

                        wtr.SaveLocation();
                        wtr.SetPosition(dir.root + dirOffset);
                        (i.Data as Subdirectory).Save(wtr, dir);
                        wtr.LoadLocation();
                    }
                    else if ((i.Type & EntryType.DataEntry) == EntryType.DataEntry)
                    {
                        uint dateOffset = dir.dateO[i.Data as DataEntry];
                        wtr.Write(dateOffset);

                        wtr.SaveLocation();
                        wtr.SetPosition(dir.root + dateOffset);
                        (i.Data as DataEntry).Save(wtr, dir);
                        wtr.LoadLocation();
                    }
                }
            }
        }
        [Flags]
        public enum EntryType
        {
            Subdirectory = 0x1,
            DataEntry = 0x2,
            Name = 0x4,
            ID = 0x8
        }
        public class ResourceEntry
        {
            EntryType t;
            public EntryType Type { get { return t; } set { t = value; } }

            string n = "";
            uint id;
            public string Name { get { return n; } set { n = value; } }
            public uint ID { get { return id; } set { id = value; } }

            object dat;
            public object Data { get { return dat; } set { dat = value; } }
        }

        public class DataEntry
        {
            ResourceEntry par;
            public DataEntry(ResourceEntry parent) { par = parent; }

            public ResourceEntry Parent { get { return par; } }

            public uint GetSize()
            {
                return 16;
            }

            internal void Load(VirtualReader rdr)
            {
                Rva datAdr = rdr.ReadRva();
                uint s = rdr.ReadUInt32();
                cp = rdr.ReadUInt32();
                rdr.ReadUInt32();

                rdr.SaveLocation();
                rdr.SetPosition(datAdr);
                dat = rdr.ReadBytes((int)s);
                rdr.LoadLocation();
            }

            internal void Save(VirtualWriter wtr, ResourceDirectory dir)
            {
                uint datOffset = dir.datO[dat];
                wtr.Write(datOffset);
                wtr.Write(dat.Length);
                wtr.Write(cp);
                wtr.Write((uint)0);

                wtr.SaveLocation();
                wtr.SetPosition(datOffset);
                wtr.Write(dat);
                wtr.LoadLocation();
            }


            byte[] dat;
            public byte[] Datas { get { return dat; } set { dat = value; } }

            uint cp;
            public uint CodePage { get { return cp; } set { cp = value; } }
        }

        uint c;
        public uint Characteristics { get { return c; } set { c = value; } }

        DateTime stamp;
        public DateTime DateTimeStamp { get { return stamp; } set { stamp = value; } }

        ushort maVer;
        public ushort MajorVersion { get { return maVer; } set { maVer = value; } }

        ushort miVer;
        public ushort MinorVersion { get { return miVer; } set { miVer = value; } }

        ushort noName;
        public ushort NumberOfNameEntries { get { return noName; } set { noName = value; } }

        ushort noID;
        public ushort NumberOfIDEntries { get { return noID; } set { noID = value; } }



        public override DataDirectoryType Type
        {
            get { return DataDirectoryType.Resource; }
        }


        public uint GetHeaderSize()
        {
            return 16 + (uint)es.Count * 8;
        }

        public override uint GetTotalSize()
        {
            uint ret = GetHeaderSize();
            foreach (ResourceEntry i in es)
            {
                if ((i.Type & EntryType.Name) == EntryType.Name)
                    ret += (uint)((i.Name.Length * 2 + 2 + 3) & ~3);
                if ((i.Type & EntryType.Subdirectory) == EntryType.Subdirectory)
                    ret += (i.Data as Subdirectory).GetTotalSize();
                else
                {
                    ret += (i.Data as DataEntry).GetSize();
                    ret += (uint)(((i.Data as DataEntry).Datas.Length + 3) & ~3);
                }
            }
            return ret;
        }
        public override uint GetDirectorySize()
        {
            uint ret = GetHeaderSize();
            foreach (ResourceEntry i in es)
            {
                if ((i.Type & EntryType.Name) == EntryType.Name)
                    ret += (uint)((i.Name.Length * 2 + 2 + 3) & ~3);
                if ((i.Type & EntryType.Subdirectory) == EntryType.Subdirectory)
                    ret += (i.Data as Subdirectory).GetTotalSize();
                else
                {
                    ret += (i.Data as DataEntry).GetSize();
                    ret += (uint)(((i.Data as DataEntry).Datas.Length + 3) & ~3);
                }
            }
            return ret;
        }

        private Dictionary<Subdirectory, uint> dirO;
        private Dictionary<string, uint> strO;
        private Dictionary<DataEntry, uint> dateO;
        private Dictionary<byte[], uint> datO;
        private Rva root;
        private void ComputeOffset()
        {
            dirO = new Dictionary<Subdirectory, uint>();
            strO = new Dictionary<string, uint>();
            dateO = new Dictionary<DataEntry, uint>();
            datO = new Dictionary<byte[], uint>();

            uint offset = 16;
            Queue<Subdirectory> dirs = new Queue<Subdirectory>();
            foreach (ResourceEntry i in es)
            {
                offset += 8;
                if ((i.Type & EntryType.Name) == EntryType.Name)
                    strO[i.Name] = 0;
                if ((i.Type & EntryType.Subdirectory) == EntryType.Subdirectory)
                    dirs.Enqueue(i.Data as Subdirectory);
                else
                    dateO[i.Data as DataEntry] = 0;
            }
            while (dirs.Count != 0)
            {
                Subdirectory dir = dirs.Dequeue();
                dirO[dir] = offset;
                offset += 16;
                foreach (ResourceEntry i in dir)
                {
                    offset += 8;
                    if ((i.Type & EntryType.Name) == EntryType.Name)
                        strO[i.Name] = 0;
                    if ((i.Type & EntryType.Subdirectory) == EntryType.Subdirectory)
                        dirs.Enqueue(i.Data as Subdirectory);
                    else
                        dateO[i.Data as DataEntry] = 0;
                }
            }

            foreach (DataEntry i in dateO.Keys.ToArray())
            {
                dateO[i] = offset;
                offset += 16;
            }

            foreach (string i in strO.Keys.ToArray())
            {
                strO[i] = offset;
                offset += (uint)((i.Length * 2 + 2 + 3) & ~3);
            }

            foreach (DataEntry i in dateO.Keys.ToArray())
            {
                datO[i.Datas] = offset;
                offset += (uint)((i.Datas.Length + 3) & ~3);
            }
        }

        protected override void SaveInternal(VirtualWriter wtr)
        {
            root = Location.Address;
            ComputeOffset();

            wtr.Write(c);
            wtr.WriteStamp(stamp);
            wtr.Write(maVer);
            wtr.Write(miVer);
            wtr.Write(noName);
            wtr.Write(noID);
            foreach (ResourceEntry i in es)
            {
                if ((i.Type & EntryType.Name) == EntryType.Name)
                {
                    uint strOffset = strO[i.Name];
                    wtr.Write(strOffset | 0x80000000);

                    wtr.SaveLocation();
                    wtr.SetPosition(root + strOffset);
                    wtr.WriteUnicodeString(i.Name);
                    wtr.LoadLocation();
                }
                else if ((i.Type & EntryType.ID) == EntryType.ID)
                {
                    wtr.Write(i.ID);
                }

                if ((i.Type & EntryType.Subdirectory) == EntryType.Subdirectory)
                {
                    uint dirOffset = dirO[i.Data as Subdirectory];
                    wtr.Write(dirOffset | 0x80000000);

                    wtr.SaveLocation();
                    wtr.SetPosition(root + dirOffset);
                    (i.Data as Subdirectory).Save(wtr, this);
                    wtr.LoadLocation();
                }
                else if ((i.Type & EntryType.DataEntry) == EntryType.DataEntry)
                {
                    uint dateOffset = dateO[i.Data as DataEntry];
                    wtr.Write(dateOffset);

                    wtr.SaveLocation();
                    wtr.SetPosition(root + dateOffset);
                    (i.Data as DataEntry).Save(wtr, this);
                    wtr.LoadLocation();
                }
            }
        }

        protected override void LoadInternal(VirtualReader rdr)
        {
            Rva root = rdr.GetPosition();
            c = rdr.ReadUInt32();
            stamp = rdr.ReadStamp();
            maVer = rdr.ReadUInt16();
            miVer = rdr.ReadUInt16();
            noName = rdr.ReadUInt16();
            noID = rdr.ReadUInt16();
            for (int i = 0; i < noName; i++)
            {
                ResourceEntry e = new ResourceEntry();
                e.Type = EntryType.Name;
                Rva nRva = rdr.ReadRva();
                rdr.SaveLocation();
                rdr.SetPosition(nRva);
                e.Name = rdr.ReadUnicodeString();
                rdr.LoadLocation();
                Rva datAdr = rdr.ReadUInt32();
                if ((datAdr & 0x80000000) == 0x80000000)
                {
                    datAdr &= ~0x80000000;
                    e.Type |= EntryType.Subdirectory;
                    e.Data = new Subdirectory(e);
                    rdr.SaveLocation();
                    rdr.SetPosition(datAdr + root);
                    (e.Data as Subdirectory).Load(rdr, root);
                    rdr.LoadLocation();
                }
                else
                {
                    e.Type |= EntryType.DataEntry;
                    e.Data = new DataEntry(e);
                    rdr.SaveLocation();
                    rdr.SetPosition(datAdr + root);
                    (e.Data as DataEntry).Load(rdr);
                    rdr.LoadLocation();
                }
                this.Add(e);
            }
            for (int i = 0; i < noID; i++)
            {
                ResourceEntry e = new ResourceEntry();
                e.Type = EntryType.ID;
                e.ID = rdr.ReadUInt32() & ~0x80000000;
                Rva datAdr = rdr.ReadUInt32();
                if ((datAdr & 0x80000000) == 0x80000000)
                {
                    datAdr &= ~0x80000000;
                    e.Type |= EntryType.Subdirectory;
                    e.Data = new Subdirectory(e);
                    rdr.SaveLocation();
                    rdr.SetPosition(datAdr + root);
                    (e.Data as Subdirectory).Load(rdr, root);
                    rdr.LoadLocation();
                }
                else
                {
                    e.Type |= EntryType.DataEntry;
                    e.Data = new DataEntry(e);
                    rdr.SaveLocation();
                    rdr.SetPosition(datAdr + root);
                    (e.Data as DataEntry).Load(rdr);
                    rdr.LoadLocation();
                }
                this.Add(e);
            }
        }


        List<ResourceEntry> es = new List<ResourceEntry>();
        public int IndexOf(ResourceEntry item)
        {
            return es.IndexOf(item);
        }
        public void Insert(int index, ResourceEntry item)
        {
            es.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            es.RemoveAt(index);
        }
        public ResourceEntry this[int index]
        {
            get
            {
                return es[index];
            }
            set
            {
                es[index] = value;
            }
        }
        public void Add(ResourceEntry item)
        {
            es.Add(item);
        }
        public void Clear()
        {
            es.Clear();
        }
        public bool Contains(ResourceEntry item)
        {
            return es.Contains(item);
        }
        public void CopyTo(ResourceEntry[] array, int arrayIndex)
        {
            es.CopyTo(array, arrayIndex);
        }
        public int Count
        {
            get { return es.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        public bool Remove(ResourceEntry item)
        {
            return es.Remove(item);
        }
        public IEnumerator<ResourceEntry> GetEnumerator()
        {
            return es.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
