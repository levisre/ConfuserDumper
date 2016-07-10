using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Metadata.Tables;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace NetPE.Core.Metadata.Heaps
{
    [Flags]
    public enum TableFlags : ulong
    {
        Assembly = 0x100000000,
        AssemblyOS = 0x400000000,
        AssemblyProcessor = 0x200000000,
        AssemblyRef = 0x800000000,
        AssemblyRefOS = 0x2000000000,
        AssemblyRefProcessor = 0x1000000000,
        ClassLayout = 0x8000,
        Constant = 0x800,
        CustomAttribute = 0x1000,
        DeclSecurity = 0x4000,
        ENCLog = 0x40000000,
        ENCMap = 0x80000000,
        EventMap = 0x40000,
        Event = 0x100000,
        EventPtr = 0x80000,
        ExportedType = 0x8000000000,
        Field = 0x10,
        FieldLayout = 0x10000,
        FieldMarshal = 0x2000,
        FieldPtr = 0x08,
        FieldRVA = 0x20000000,
        File = 0x4000000000,
        GenericParam = 0x40000000000,
        GenericParamConstraint = 0x100000000000,
        ImplMap = 0x10000000,
        InterfaceImpl = 0x200,
        ManifestResource = 0x10000000000,
        MemberRef = 0x400,
        MethodDef = 0x40,
        MethodImpl = 0x2000000,
        MethodPtr = 0x20,
        MethodSemantics = 0x1000000,
        MethodSpec = 0x80000000000,
        Module = 0x1,
        ModuleRef = 0x4000000,
        NestedClass = 0x20000000000,
        Param = 0x100,
        ParamPtr = 0x80,
        Property = 0x800000,
        PropertyMap = 0x200000,
        PropertyPtr = 0x400000,
        StandAloneSig = 0x20000,
        TypeDef = 0x4,
        TypeRef = 0x2,
        TypeSpec = 0x8000000
    }

    [Flags]
    public enum HeapSizeFlags : byte
    {
        LargeStrings = 0x01,
        LargeGUID = 0x02,
        LargeBlob = 0x04
    }

    public class TablesHeap
    {
        MetadataStream str;
        public TablesHeap(MetadataStream str) { this.str = str; }
        public MetadataStream Stream { get { return str; } }

        uint res;
        public uint Reserved { get { return res; } set { res = value; } }
        byte maVer;
        public byte MajorVersion { get { return maVer; } set { maVer = value; } }
        byte miVer;
        public byte MinorVersion { get { return miVer; } set { miVer = value; } }
        HeapSizeFlags f;    
        public HeapSizeFlags HeapSize { get { return f; } set { f = value; } }
        byte res2;
        public byte Reserved2 { get { return res2; } set { res2 = value; } }
        TableFlags v;
        public TableFlags Valid { get { return v; } set { v = value; } }
        TableFlags s;
        public TableFlags Sorted { get { return s; } set { s = value; } }


        private uint[] len;
        public void Load()
        {
            MetadataReader rdr = new MetadataReader(str);
            str.Position = 0;
            res = rdr.ReadUInt32();
            maVer = rdr.ReadByte();
            miVer = rdr.ReadByte();
            f = (HeapSizeFlags)rdr.ReadByte();
            res2 = rdr.ReadByte();
            v = (TableFlags)rdr.ReadUInt64();
            s = (TableFlags)rdr.ReadUInt64();
            FillLens(rdr);
            FillTables(rdr);
        }

        public void Save()
        {
            MetadataWriter wtr = new MetadataWriter(str);
            str.Position = 0;
            wtr.Write(res);
            wtr.Write(maVer);
            wtr.Write(miVer);
            wtr.Write((byte)f);
            wtr.Write(res2);
            wtr.Write((ulong)v);
            wtr.Write((ulong)s);
            FillLens();
            foreach (MetadataTable i in mt)
            {
                if (i != null) wtr.Write((uint)i.Rows.Count);
            }
            TableRw rw = new TableRw();
            foreach (MetadataTable i in mt)
            {
                if (i != null) rw.Save(i, wtr, len);
            }
        }

        MetadataTable[] mt;
        public MetadataTable this[TableType t]
        {
            get
            {
                return mt[(int)t];
            }
            set
            {
                mt[(int)t] = value;
            }
        }

        public MetadataRow Resolve(MetadataToken tkn)
        {
            //TODO: Check if correct is
            return mt[(int)tkn.Type].Rows[(int)tkn.Index];
            //return mt[(int)tkn.Type].Rows[(int)tkn.Value];
        }

        private void FillLens(MetadataReader rdr)
        {
            len = new uint[0x2d];
            mt = new MetadataTable[0x2d];
            for (int i = 0; i < 0x2d; i++)
            {
                TableFlags mask = (TableFlags)((long)1 << i);
                if ((v & mask) == mask)
                {
                    len[i] = rdr.ReadUInt32();
                    mt[i] = new MetadataTable(this, (TableType)i);
                }
                else
                {
                    mt[i] = null;
                }
            }
        }

        private void FillLens()
        {
            len = new uint[0x2d];
            for (int i = 0; i < 0x2d; i++)
                if (mt[i] != null)
                    len[i] = (uint)mt[i].Rows.Count;
        }

        private void FillTables(MetadataReader rdr)
        {
            TableRw rw = new TableRw();
            for (int i = 0; i < 0x2d; i++)
            {
                if (mt[i] != null)
                    mt[i] = rw.Load(this, rdr, len, (TableType)i);
            }
        }
    }
}
