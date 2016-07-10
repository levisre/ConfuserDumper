using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Pe;
using System.Collections.ObjectModel;

namespace NetPE.Core.DataDirectories
{
    public class ImportDirectory : DataDirectory, IList<ImportDirectory.ImportDirectoryEntry>
    {
        public ImportDirectory(DataDirectoryEntry entry) : base(entry) { }

        public class ImportDirectoryEntry
        {
            ImportDirectory dd;
            public ImportDirectoryEntry(ImportDirectory dd) { this.dd = dd; }
            internal ImportDirectory Directory { get { return dd; } set { dd = value; } }

            ImportFunctionTable lt;
            public ImportFunctionTable LookupTable
            {
                get { return lt; }
                set { lt = value; }
            }

            DateTime stamp;
            public DateTime DateTimeStamp { get { return stamp; } set { stamp = value; } }

            uint f;
            public uint ForwarderChain { get { return f; } set { f = value; } }

            string n;
            public string Name
            {
                get { return n; }
                set { n = value; }
            }

            ImportFunctionTable at;
            public ImportFunctionTable AddressTable
            {
                get { return at; }
                set { at = value; }
            }

            public override string ToString()
            {
                return n;
            }
        }
        public enum ImportFunctionType
        {
            Name,
            Ordinal,
            Binded
        }
        public class ImportFunction
        {
            ImportFunctionType t;
            public ImportFunctionType Type { get { return t; } set { t = value; } }
            ushort h;
            public ushort Hint { get { return h; } set { h = value; } }
            string n = "";
            public string Name { get { return n; } set { n = value; } }
            ushort o;
            public ushort Ordinal { get { return o; } set { o = value; } }
            Rva func;
            public Rva FunctionAddress { get { return func; } set { func = value; } }

            public override string ToString()
            {
                if (t == ImportFunctionType.Name)
                    return n;
                else if (t == ImportFunctionType.Binded)
                    return "[Binded Address: " + o + "]";
                else
                    return "[Ordinal: " + o + "]";
            }
        }
        public class ImportFunctionTable : Collection<ImportFunction>
        {
            ImportDirectoryEntry entry;
            public ImportFunctionTable(ImportDirectoryEntry entry) { this.entry = entry; }

            bool is8 = false;

            public uint GetPointersSize()
            {
                uint s = is8 ? 8U : 4;
                return (uint)this.Count * s + s;
            }

            public uint GetFunctionsSize()
            {
                uint ret = 0;
                foreach (ImportFunction f in this)
                {
                    if (f.Type == ImportFunctionType.Name)
                        ret += (uint)(2 + f.Name.Length + 1);
                }
                return ret;
            }

            internal void Load(VirtualReader rdr, bool binded)
            {
                if (rdr.BaseStream.File.OptionalHeader.Type == OptionalHeader.ExecutableType.PE32Plus)
                {
                    ulong n;
                    while ((n = rdr.ReadUInt64()) != 0)
                    {
                        ReadPointer(rdr, n, binded);
                    }
                    is8 = true;
                }
                else
                {
                    uint n;
                    while ((n = rdr.ReadUInt32()) != 0)
                    {
                        ReadPointer(rdr, n, binded);
                    }
                    is8 = false;
                }
            }

            internal void Save(VirtualWriter wtr)
            {
                if (wtr.BaseStream.File.OptionalHeader.Type == OptionalHeader.ExecutableType.PE32Plus)
                {
                    foreach (ImportFunction func in Items)
                    {
                        switch (func.Type)
                        {
                            case ImportFunctionType.Binded:
                                wtr.Write((ulong)func.FunctionAddress); break;
                            case ImportFunctionType.Name:
                                wtr.Write((ulong)entry.Directory.GetFunctionAddress(this, func)); break;
                            case ImportFunctionType.Ordinal:
                                wtr.Write(0x8000000000000000 | func.Ordinal); break;
                        }
                    }
                    wtr.Write(0UL);
                }
                else
                {
                    foreach (ImportFunction func in Items)
                    {
                        switch (func.Type)
                        {
                            case ImportFunctionType.Binded:
                                wtr.Write((uint)func.FunctionAddress); break;
                            case ImportFunctionType.Name:
                                wtr.Write((uint)entry.Directory.GetFunctionAddress(this, func)); break;
                            case ImportFunctionType.Ordinal:
                                wtr.Write(0x80000000 | func.Ordinal); break;
                        }
                    }
                    wtr.Write(0U);
                }
            }


            public void ReadPointer(VirtualReader rdr, ulong val, bool binded)
            {
                ImportFunction f = new ImportFunction();
                if (binded)
                {
                    f.Type = ImportFunctionType.Binded;
                    f.FunctionAddress = (Rva)val;
                }
                else if (rdr.BaseStream.File.OptionalHeader.Type == OptionalHeader.ExecutableType.PE32Plus)
                {
                    if ((val & 0x8000000000000000) == 0x8000000000000000)
                    {
                        f.Type = ImportFunctionType.Ordinal;
                        f.Ordinal = (ushort)(val & ~0x8000000000000000);
                    }
                    else
                    {
                        f.Type = ImportFunctionType.Name;
                        rdr.SaveLocation();
                        rdr.SetPosition((Rva)val);
                        f.Hint = rdr.ReadUInt16();
                        f.Name = rdr.ReadString();
                        rdr.LoadLocation();
                    }
                }
                else
                {
                    if ((val & 0x80000000) == 0x80000000)
                    {
                        f.Type = ImportFunctionType.Ordinal;
                        f.Ordinal = (ushort)(val & ~0x80000000);
                    }
                    else
                    {
                        f.Type = ImportFunctionType.Name;
                        rdr.SaveLocation();
                        rdr.SetPosition((Rva)val);
                        f.Hint = rdr.ReadUInt16();
                        f.Name = rdr.ReadString();
                        rdr.LoadLocation();
                    }
                }
                Items.Add(f);
            }
        }

        public override DataDirectoryType Type { get { return DataDirectoryType.Import; } }

        public override uint GetTotalSize()
        {
            uint ret = (uint)this.items.Count * 20 + 20;
            foreach (ImportDirectoryEntry i in items)
            {
                ret += i.LookupTable.GetPointersSize();
                ret += (uint)i.Name.Length + 1;
                ret += i.LookupTable.GetFunctionsSize();
            }
            return ret;
        }

        public override uint GetDirectorySize()
        {
            uint ret = (uint)this.items.Count * 20 + 20;
            foreach (ImportDirectoryEntry i in items)
            {
                ret += i.LookupTable.GetPointersSize();
                ret += (uint)i.Name.Length + 1;
                ret += i.LookupTable.GetFunctionsSize();
            }
            return ret;
        }

        private Rva GetFunctionsAddress()
        {
            uint ret = Location.Address + (uint)this.items.Count * 20 + 20;
            foreach (ImportDirectoryEntry i in items)
            {
                ret += i.LookupTable.GetPointersSize();
            }
            return ret;
        }
        private Rva GetNamesAddress()
        {
            uint ret = Location.Address + (uint)this.items.Count * 20 + 20;
            foreach (ImportDirectoryEntry i in items)
            {
                ret += i.LookupTable.GetPointersSize();
                ret += i.LookupTable.GetFunctionsSize();
            }
            return ret;
        }
        private Rva GetFunctionAddress(ImportFunctionTable tbl, ImportFunction func)
        {
            uint ret = Location.Address + (uint)this.items.Count * 20 + 20;
            foreach (ImportDirectoryEntry i in items)
                ret += i.LookupTable.GetPointersSize();
            foreach (ImportDirectoryEntry i in items)
            {
                foreach (ImportFunction f in i.LookupTable)
                {
                    if (f.Type == ImportFunctionType.Name)
                        if ((i.LookupTable == tbl || i.AddressTable == tbl) && f == func)
                            return ret;
                        else
                            ret += 2 + (uint)f.Name.Length + 1;
                }
            }
            return ret;
        }


        protected override void SaveInternal(VirtualWriter wtr)
        {
            Rva iats = wtr.BaseStream.File.OptionalHeader.DataDirectories[DataDirectoryType.IAT].Address;
            Rva ptr = (uint)this.items.Count * 20 + 20;
            Rva funcs = GetFunctionsAddress();
            Rva names = GetNamesAddress();
            uint iatFuncsOffset = 0;
            uint iltFuncsOffset = 0;
            foreach (ImportDirectoryEntry i in items)
            {
                wtr.Write(Location.Address + ptr + iltFuncsOffset);
                wtr.WriteStamp(i.DateTimeStamp);
                wtr.Write(i.ForwarderChain);
                wtr.Write(names);
                if (iats == 0)
                    wtr.Write(Location.Address + ptr + iltFuncsOffset);
                else
                    wtr.Write(iats + ptr + iatFuncsOffset);

                iltFuncsOffset += i.LookupTable.GetPointersSize();
                iatFuncsOffset += i.AddressTable.GetPointersSize();
                names += (uint)i.Name.Length + 1;
            }

            wtr.Write(new byte[20]);

            foreach (ImportDirectoryEntry i in items)
            {
                i.LookupTable.Save(wtr);
            }

            foreach (ImportDirectoryEntry i in items)
            {
                foreach (ImportFunction func in i.LookupTable)
                {
                    if (func.Type == ImportFunctionType.Name)
                    {
                        wtr.Write(func.Hint);
                        wtr.Write(func.Name);
                    }
                }
            }

            foreach (ImportDirectoryEntry i in items)
            {
                wtr.Write(i.Name);
            }

            if (iats != 0)
            {
                wtr.SetPosition(iats);
                foreach (ImportDirectoryEntry i in items)
                {
                    i.AddressTable.Save(wtr);
                }
            }
        }

        protected override void LoadInternal(VirtualReader rdr)
        {
            ImportDirectoryEntry entry;
            while (true)
            {
                entry = new ImportDirectoryEntry(this);
                Rva ltAdr = rdr.ReadRva();
                entry.DateTimeStamp = rdr.ReadStamp();
                entry.ForwarderChain = rdr.ReadUInt32();
                Rva nAdr = rdr.ReadRva();
                Rva atAdr = rdr.ReadRva();
                if (ltAdr == 0 && atAdr == 0 && nAdr == 0)
                    break;

                rdr.SaveLocation();
                rdr.SetPosition(ltAdr);
                entry.LookupTable = new ImportFunctionTable(entry);
                entry.LookupTable.Load(rdr, false);
                rdr.LoadLocation();

                rdr.SaveLocation();
                rdr.SetPosition(nAdr);
                entry.Name = rdr.ReadString();
                rdr.LoadLocation();

                rdr.SaveLocation();
                rdr.SetPosition(atAdr);
                entry.AddressTable = new ImportFunctionTable(entry);
                entry.AddressTable.Load(rdr, StampsHelper.UIntFromStamp(entry.DateTimeStamp) == 0xffffffff);
                rdr.LoadLocation();

                items.Add(entry);
            }
        }

        public uint GetIATSize()
        {
            uint ret = 0;
            foreach (ImportDirectoryEntry i in this)
            {
                ret += i.AddressTable.GetPointersSize();
            }
            return ret;
        }


        List<ImportDirectoryEntry> items = new List<ImportDirectoryEntry>();
        public void Add(ImportDirectoryEntry item)
        {
            items.Add(item);
        }
        public void Clear()
        {
            items.Clear();
        }
        public bool Contains(ImportDirectoryEntry item)
        {
            return items.Contains(item);
        }
        public void CopyTo(ImportDirectoryEntry[] array, int arrayIndex)
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
        public bool Remove(ImportDirectoryEntry item)
        {
            return items.Remove(item);
        }
        public IEnumerator<ImportDirectoryEntry> GetEnumerator()
        {
            return items.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public int IndexOf(ImportDirectoryEntry item)
        {
            return items.IndexOf(item);
        }
        public void Insert(int index, ImportDirectoryEntry item)
        {
            items.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }
        public ImportDirectory.ImportDirectoryEntry this[int index]
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
