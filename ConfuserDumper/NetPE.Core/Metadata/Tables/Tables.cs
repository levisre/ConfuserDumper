using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using NetPE.Core.Metadata.Heaps;
using System.IO;
using System.Collections;
using NetPE.Core.Metadata.Signature;
using NetPE.Core.Pe;
using NetPE.Core.Metadata.Methods;
using NetPE.Core.Metadata.Signature.Types;
using NetPE.Core.DataDirectories;

namespace NetPE.Core.Metadata.Tables
{
    internal class TableDef
    {
        internal TableDef(TableType type, string name, params KeyValuePair<string, object>[] cols)
        {
            t = type;
            n = name;
            cs = cols;
            TableDefs.MdTableDefs.Add(t, this);
        }

        TableType t;
        public TableType Type { get { return t; } }
        string n;
        public string Name { get { return n; } set { n = value; } }
        KeyValuePair<string, object>[] cs;
        public KeyValuePair<string, object>[] Columns { get { return cs; } }

        public int IndexOfColumn(string name)
        {
            for (int i = 0; i < cs.Length; i++)
                if (cs[i].Key.ToLower() == name.ToLower()) return i;
            return -1;
        }
    }
    internal static class TableDefs
    {
        public static Dictionary<TableType, TableDef> MdTableDefs = new Dictionary<TableType, TableDef>();

        public static TableDef Assembly = new TableDef(TableType.Assembly, "Assembly",
            new KeyValuePair<string, object>("HashAlgId", typeof(AssemblyHashAlgorithm)),
            new KeyValuePair<string, object>("MajorVersion", typeof(ushort)),
            new KeyValuePair<string, object>("MinorVersion", typeof(ushort)),
            new KeyValuePair<string, object>("BuildNumber", typeof(ushort)),
            new KeyValuePair<string, object>("RevisionNumber", typeof(ushort)),
            new KeyValuePair<string, object>("Flags", typeof(AssemblyFlags)),
            new KeyValuePair<string, object>("PublicKey", typeof(BlobToken)),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("Locale", typeof(StringToken)));

        public static TableDef AssemblyOS = new TableDef(TableType.AssemblyOS, "AssemblyOS",
            new KeyValuePair<string, object>("OSPlatformId", typeof(uint)),
            new KeyValuePair<string, object>("OSMajorVersion", typeof(uint)),
            new KeyValuePair<string, object>("OSMinorVersion", typeof(uint)));

        public static TableDef AssemblyProcessor = new TableDef(TableType.AssemblyProcessor, "AssemblyProcessor",
            new KeyValuePair<string, object>("Processor", typeof(uint)));

        public static TableDef AssemblyRef = new TableDef(TableType.AssemblyRef, "AssemblyRef",
            new KeyValuePair<string, object>("MajorVersion", typeof(ushort)),
            new KeyValuePair<string, object>("MinorVersion", typeof(ushort)),
            new KeyValuePair<string, object>("BuildNumber", typeof(ushort)),
            new KeyValuePair<string, object>("RevisionNumber", typeof(ushort)),
            new KeyValuePair<string, object>("Flags", typeof(AssemblyFlags)),
            new KeyValuePair<string, object>("PublicKeyOrToken", typeof(BlobToken)),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("Locale", typeof(StringToken)),
            new KeyValuePair<string, object>("HashValue", typeof(BlobToken)));

        public static TableDef AssemblyRefOS = new TableDef(TableType.AssemblyRefOS, "AssemblyRefOS",
            new KeyValuePair<string, object>("OSPlatformId", typeof(uint)),
            new KeyValuePair<string, object>("OSMajorVersion", typeof(uint)),
            new KeyValuePair<string, object>("OSMinorVersion", typeof(uint)),
            new KeyValuePair<string, object>("AssemblyRef", TableType.AssemblyRef));

        public static TableDef AssemblyRefProcessor = new TableDef(TableType.AssemblyRefProcessor, "AssemblyRefProcessor",
            new KeyValuePair<string, object>("Processor", typeof(uint)),
            new KeyValuePair<string, object>("AssemblyRef", TableType.AssemblyRef));

        public static TableDef ClassLayout = new TableDef(TableType.ClassLayout, "ClassLayout",
            new KeyValuePair<string, object>("PackingSize", typeof(ushort)),
            new KeyValuePair<string, object>("ClassSize", typeof(uint)),
            new KeyValuePair<string, object>("Parent", TableType.TypeDef));

        public static TableDef Constant = new TableDef(TableType.Constant, "Constant",
            new KeyValuePair<string, object>("Type", typeof(ElementType)),
            new KeyValuePair<string, object>("Padding", typeof(byte)),
            new KeyValuePair<string, object>("Parent", CodedIndex.HasConstant),
            new KeyValuePair<string, object>("Value", typeof(BlobToken)));

        public static TableDef CustomAttribute = new TableDef(TableType.CustomAttribute, "CustomAttribute",
            new KeyValuePair<string, object>("Parent", CodedIndex.HasCustomAttribute),
            new KeyValuePair<string, object>("Type", CodedIndex.CustomAttributeType),
            new KeyValuePair<string, object>("Value", typeof(BlobToken)));

        public static TableDef DeclSecurity = new TableDef(TableType.DeclSecurity, "DeclSecurity",
            new KeyValuePair<string, object>("Action", typeof(SecurityAction)),
            new KeyValuePair<string, object>("Parent", CodedIndex.HasDeclSecurity),
            new KeyValuePair<string, object>("PermissionSet", typeof(BlobToken)));

        public static TableDef ENCLog = new TableDef(TableType.ENCLog, "ENCLog",
            new KeyValuePair<string, object>("Token", typeof(uint)),
            new KeyValuePair<string, object>("FuncCode", typeof(uint)));

        public static TableDef ENCMap = new TableDef(TableType.ENCMap, "ENCMap",
            new KeyValuePair<string, object>("Token", typeof(uint)));

        public static TableDef Event = new TableDef(TableType.Event, "Event",
            new KeyValuePair<string, object>("EventFlags", typeof(EventAttributes)),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("EventType", CodedIndex.TypeDefOrRef));

        public static TableDef EventMap = new TableDef(TableType.EventMap, "EventMap",
            new KeyValuePair<string, object>("Parent", TableType.TypeDef),
            new KeyValuePair<string, object>("EventList", TableType.Event));

        public static TableDef EventPtr = new TableDef(TableType.EventPtr, "EventPtr",
            new KeyValuePair<string, object>("Event", TableType.Event));

        public static TableDef ExportedType = new TableDef(TableType.ExportedType, "ExportedType",
            new KeyValuePair<string, object>("Flags", typeof(TypeAttributes)),
            new KeyValuePair<string, object>("TypeDefId", typeof(uint)),
            new KeyValuePair<string, object>("TypeName", typeof(StringToken)),
            new KeyValuePair<string, object>("TypeNamespace", typeof(StringToken)),
            new KeyValuePair<string, object>("Implementation", CodedIndex.Implementation));

        public static TableDef Field = new TableDef(TableType.Field, "Field",
            new KeyValuePair<string, object>("Flags", typeof(FieldAttributes)),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("Signature", typeof(BlobToken)));

        public static TableDef FieldLayout = new TableDef(TableType.FieldLayout, "FieldLayout",
            new KeyValuePair<string, object>("OffSet", typeof(uint)),
            new KeyValuePair<string, object>("Field", TableType.Field));

        public static TableDef FieldMarshal = new TableDef(TableType.FieldMarshal, "FieldMarshal",
            new KeyValuePair<string, object>("Parent", CodedIndex.HasFieldMarshall),
            new KeyValuePair<string, object>("NativeType", typeof(BlobToken)));

        public static TableDef FieldPtr = new TableDef(TableType.FieldPtr, "FieldPtr",
            new KeyValuePair<string, object>("Field", TableType.Field));

        public static TableDef FieldRVA = new TableDef(TableType.FieldRVA, "FieldRVA",
            new KeyValuePair<string, object>("Data", typeof(Rva)),
            new KeyValuePair<string, object>("Field", TableType.Field));

        public static TableDef File = new TableDef(TableType.File, "File",
            new KeyValuePair<string, object>("Flags", typeof(FileAttributes)),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("HashValue", typeof(BlobToken)));

        public static TableDef GenericParam = new TableDef(TableType.GenericParam, "GenericParam",
            new KeyValuePair<string, object>("Number", typeof(ushort)),
            new KeyValuePair<string, object>("Flags", typeof(GenericParamAttributes)),
            new KeyValuePair<string, object>("Owner", CodedIndex.TypeOrMethodDef),
            new KeyValuePair<string, object>("Name", typeof(StringToken)));

        public static TableDef GenericParamConstraint = new TableDef(TableType.GenericParamConstraint, "GenericParamConstraint",
            new KeyValuePair<string, object>("Owner", TableType.GenericParam),
            new KeyValuePair<string, object>("Constraint", CodedIndex.TypeDefOrRef));

        public static TableDef ImplMap = new TableDef(TableType.ImplMap, "ImplMap",
            new KeyValuePair<string, object>("MappingFlags", typeof(PInvokeAttributes)),
            new KeyValuePair<string, object>("MemberForwarded", CodedIndex.MemberForwarded),
            new KeyValuePair<string, object>("ImportName", typeof(StringToken)),
            new KeyValuePair<string, object>("ImportScope", TableType.ModuleRef));

        public static TableDef InterfaceImpl = new TableDef(TableType.InterfaceImpl, "InterfaceImpl",
            new KeyValuePair<string, object>("Class", TableType.TypeDef),
            new KeyValuePair<string, object>("Interface", CodedIndex.TypeDefOrRef));

        public static TableDef ManifestResource = new TableDef(TableType.ManifestResource, "ManifestResource",
            new KeyValuePair<string, object>("Offset", typeof(uint)),
            new KeyValuePair<string, object>("Flags", typeof(ManifestResourceAttributes)),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("Implementation", CodedIndex.Implementation));

        public static TableDef MemberRef = new TableDef(TableType.MemberRef, "MemberRef",
            new KeyValuePair<string, object>("Class", CodedIndex.MemberRefParent),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("Signature", typeof(BlobToken)));

        public static TableDef MethodDef = new TableDef(TableType.MethodDef, "Method",
            new KeyValuePair<string, object>("Rva", typeof(Rva)),
            new KeyValuePair<string, object>("ImplFlags", typeof(MethodImplAttributes)),
            new KeyValuePair<string, object>("Flags", typeof(MethodAttributes)),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("Signature", typeof(BlobToken)),
            new KeyValuePair<string, object>("ParamList", TableType.Param));

        public static TableDef MethodImpl = new TableDef(TableType.MethodImpl, "MethodImpl",
            new KeyValuePair<string, object>("Class", TableType.TypeDef),
            new KeyValuePair<string, object>("MethodBody", CodedIndex.MethodDefOrRef),
            new KeyValuePair<string, object>("MethodDeclaration", CodedIndex.MethodDefOrRef));

        public static TableDef MethodPtr = new TableDef(TableType.MethodPtr, "MethodPtr",
            new KeyValuePair<string, object>("Method", TableType.MethodDef));

        public static TableDef MethodSemantics = new TableDef(TableType.MethodSemantics, "MethodSemantics",
            new KeyValuePair<string, object>("Semantic", typeof(MethodSemanticsAttributes)),
            new KeyValuePair<string, object>("Method", TableType.MethodDef),
            new KeyValuePair<string, object>("Association", CodedIndex.HasSemantics));

        public static TableDef MethodSpec = new TableDef(TableType.MethodSpec, "MethodSpec",
            new KeyValuePair<string, object>("Method", CodedIndex.MethodDefOrRef),
            new KeyValuePair<string, object>("Instantiation", typeof(BlobToken)));

        public static TableDef Module = new TableDef(TableType.Module, "Module",
            new KeyValuePair<string, object>("Generation", typeof(ushort)),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("Mvid", typeof(GUIDToken)),
            new KeyValuePair<string, object>("EncId", typeof(GUIDToken)),
            new KeyValuePair<string, object>("EncBaseId", typeof(GUIDToken)));

        public static TableDef ModuleRef = new TableDef(TableType.ModuleRef, "ModuleRef",
            new KeyValuePair<string, object>("Name", typeof(StringToken)));

        public static TableDef NestedClass = new TableDef(TableType.NestedClass, "NestedClass",
            new KeyValuePair<string, object>("NestedClass", TableType.TypeDef),
            new KeyValuePair<string, object>("EnclosingClass", TableType.TypeDef));

        public static TableDef Param = new TableDef(TableType.Param, "Param",
            new KeyValuePair<string, object>("Flags", typeof(ParamAttributes)),
            new KeyValuePair<string, object>("Sequence", typeof(ushort)),
            new KeyValuePair<string, object>("Name", typeof(StringToken)));

        public static TableDef ParamPtr = new TableDef(TableType.ParamPtr, "ParamPtr",
            new KeyValuePair<string, object>("Param", TableType.Param));

        public static TableDef Property = new TableDef(TableType.Property, "Property",
            new KeyValuePair<string, object>("Flags", typeof(PropertyAttributes)),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("Type", typeof(BlobToken)));

        public static TableDef PropertyMap = new TableDef(TableType.PropertyMap, "PropertyMap",
            new KeyValuePair<string, object>("Parent", TableType.TypeDef),
            new KeyValuePair<string, object>("PropertyList", TableType.Property));

        public static TableDef PropertyPtr = new TableDef(TableType.PropertyPtr, "PropertyPtr",
            new KeyValuePair<string, object>("Property", TableType.Property));

        public static TableDef StandAloneSig = new TableDef(TableType.StandAloneSig, "StandAloneSig",
            new KeyValuePair<string, object>("Signature", typeof(BlobToken)));

        public static TableDef TypeDef = new TableDef(TableType.TypeDef, "TypeDef",
            new KeyValuePair<string, object>("Flags", typeof(TypeAttributes)),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("Namespace", typeof(StringToken)),
            new KeyValuePair<string, object>("Extends", CodedIndex.TypeDefOrRef),
            new KeyValuePair<string, object>("FieldList", TableType.Field),
            new KeyValuePair<string, object>("MethodList", TableType.MethodDef));

        public static TableDef TypeRef = new TableDef(TableType.TypeRef, "TypeRef",
            new KeyValuePair<string, object>("ResolutionScope", CodedIndex.ResolutionScope),
            new KeyValuePair<string, object>("Name", typeof(StringToken)),
            new KeyValuePair<string, object>("Namespace", typeof(StringToken)));

        public static TableDef TypeSpec = new TableDef(TableType.TypeSpec, "TypeSpec",
            new KeyValuePair<string, object>("Signature", typeof(BlobToken)));
    }

    internal class CodedIndexDef
    {
        internal CodedIndexDef(CodedIndex idx, string name, int bit, params KeyValuePair<TableType, uint>[] ts)
        {
            ci = idx;
            n = name;
            this.bit = bit;
            tt = ts;
            CodedIndexDefs.MdCodedIndexDefs.Add(idx, this);
        }

        CodedIndex ci;
        public CodedIndex CodedIndex { get { return ci; } }
        string n;
        public string Name { get { return n; } set { n = value; } }
        int bit;
        public int Bits { get { return bit; } }
        KeyValuePair<TableType, uint>[] tt;
        public KeyValuePair<TableType, uint>[] TableTypes { get { return tt; } }

        public MetadataToken ReadToken(BinaryReader rdr, uint[] lens)
        {
            uint maxLen = 0;
            foreach (KeyValuePair<TableType, uint> i in tt)
                if (lens[(int)i.Key] > maxLen) maxLen = lens[(int)i.Key];

            uint tkn;
            var is2BytesSize = maxLen < (1 << (16 - bit));
            if (is2BytesSize)
                tkn = rdr.ReadUInt16();
            else
                tkn = rdr.ReadUInt32();

            uint mask = Convert.ToUInt32(new string('1', bit), 2);
            foreach (KeyValuePair<TableType, uint> i in tt)
                if ((tkn & mask) == i.Value)
                {
                    TableType t = i.Key;
                    uint idx = tkn >> bit;
                    return new MetadataToken((MetadataTokenType)t, idx);
                }
            return new MetadataToken((MetadataTokenType)tt[0].Key, 0);
        }
        public void WriteToken(BinaryWriter wtr, MetadataToken tkn, uint[] lens)
        {
            uint maxLen = 0;
            foreach (KeyValuePair<TableType, uint> i in tt)
                if (lens[(int)i.Key] > maxLen) maxLen = lens[(int)i.Key];

            foreach (KeyValuePair<TableType, uint> i in tt)
                if ((MetadataTokenType)i.Key == tkn.Type)
                {
                    if (maxLen > ushort.MaxValue)
                        wtr.Write((uint)(i.Value | (tkn.Index << bit)));
                    else
                        wtr.Write((ushort)(i.Value | (tkn.Index << bit)));
                    return;
                }
            throw new InvalidOperationException();
        }
        public bool HaveTableType(TableType t)
        {
            foreach (KeyValuePair<TableType, uint> i in tt)
                if (i.Key == t) return true;
            return false;
        }
        public uint GetSize(uint[] lens)
        {
            uint maxLen = 0;
            foreach (KeyValuePair<TableType, uint> i in tt)
                if (lens[(int)i.Key] > maxLen) maxLen = lens[(int)i.Key];
            return maxLen > ushort.MaxValue ? 4U : 2U;
        }

    }
    internal static class CodedIndexDefs
    {
        public static Dictionary<CodedIndex, CodedIndexDef> MdCodedIndexDefs = new Dictionary<CodedIndex, CodedIndexDef>();
      
        public static CodedIndexDef TypeDefOrRef = new CodedIndexDef(CodedIndex.TypeDefOrRef, "TypeDefOrRef", 2,
                new KeyValuePair<TableType, uint>(TableType.TypeDef, 0),
                new KeyValuePair<TableType, uint>(TableType.TypeRef, 1),
                new KeyValuePair<TableType, uint>(TableType.TypeSpec, 2));

        public static CodedIndexDef HasConstant = new CodedIndexDef(CodedIndex.HasConstant, "HasConstant", 2,
                new KeyValuePair<TableType, uint>(TableType.Field, 0),
                new KeyValuePair<TableType, uint>(TableType.Param, 1),
                new KeyValuePair<TableType, uint>(TableType.Property, 2));

        public static CodedIndexDef HasCustomAttribute = new CodedIndexDef(CodedIndex.HasCustomAttribute, "HasCustomAttribute", 5,
                new KeyValuePair<TableType, uint>(TableType.MethodDef, 0),
                new KeyValuePair<TableType, uint>(TableType.Field, 1),
                new KeyValuePair<TableType, uint>(TableType.TypeRef, 2),
                new KeyValuePair<TableType, uint>(TableType.TypeDef, 3),
                new KeyValuePair<TableType, uint>(TableType.Param, 4),
                new KeyValuePair<TableType, uint>(TableType.InterfaceImpl, 5),
                new KeyValuePair<TableType, uint>(TableType.MemberRef, 6),
                new KeyValuePair<TableType, uint>(TableType.Module, 7),
                new KeyValuePair<TableType, uint>(TableType.DeclSecurity, 8),
                new KeyValuePair<TableType, uint>(TableType.Property, 9),
                new KeyValuePair<TableType, uint>(TableType.Event, 10),
                new KeyValuePair<TableType, uint>(TableType.StandAloneSig, 11),
                new KeyValuePair<TableType, uint>(TableType.ModuleRef, 12),
                new KeyValuePair<TableType, uint>(TableType.TypeSpec, 13),
                new KeyValuePair<TableType, uint>(TableType.Assembly, 14),
                new KeyValuePair<TableType, uint>(TableType.AssemblyRef, 15),
                new KeyValuePair<TableType, uint>(TableType.File, 16),
                new KeyValuePair<TableType, uint>(TableType.ExportedType, 17),
                new KeyValuePair<TableType, uint>(TableType.ManifestResource, 18));

        public static CodedIndexDef HasFieldMarshall = new CodedIndexDef(CodedIndex.HasFieldMarshall, "HasFieldMarshall", 1,
                new KeyValuePair<TableType, uint>(TableType.Field, 0),
                new KeyValuePair<TableType, uint>(TableType.Param, 1));

        public static CodedIndexDef HasDeclSecurity = new CodedIndexDef(CodedIndex.HasDeclSecurity, "HasDeclSecurity", 2,
                new KeyValuePair<TableType, uint>(TableType.TypeDef, 0),
                new KeyValuePair<TableType, uint>(TableType.MethodDef, 1),
                new KeyValuePair<TableType, uint>(TableType.Assembly, 2));

        public static CodedIndexDef MemberRefParent = new CodedIndexDef(CodedIndex.MemberRefParent, "MemberRefParent", 3,
                new KeyValuePair<TableType, uint>(TableType.TypeDef, 0),
                new KeyValuePair<TableType, uint>(TableType.TypeRef, 1),
                new KeyValuePair<TableType, uint>(TableType.ModuleRef, 2),
                new KeyValuePair<TableType, uint>(TableType.MethodDef, 3),
                new KeyValuePair<TableType, uint>(TableType.TypeSpec, 4));

        public static CodedIndexDef HasSemantics = new CodedIndexDef(CodedIndex.HasSemantics, "HasSemantics", 1,
                new KeyValuePair<TableType, uint>(TableType.Event, 0),
                new KeyValuePair<TableType, uint>(TableType.Property, 1));

        public static CodedIndexDef MethodDefOrRef = new CodedIndexDef(CodedIndex.MethodDefOrRef, "MethodDefOrRef", 1,
                new KeyValuePair<TableType, uint>(TableType.MethodDef, 0),
                new KeyValuePair<TableType, uint>(TableType.MemberRef, 1));

        public static CodedIndexDef MemberForwarded = new CodedIndexDef(CodedIndex.MemberForwarded, "MemberForwarded", 1,
                new KeyValuePair<TableType, uint>(TableType.Field, 0),
                new KeyValuePair<TableType, uint>(TableType.MethodDef, 1));

        public static CodedIndexDef Implementation = new CodedIndexDef(CodedIndex.Implementation, "Implementation", 2,
                new KeyValuePair<TableType, uint>(TableType.File, 0),
                new KeyValuePair<TableType, uint>(TableType.AssemblyRef, 1),
                new KeyValuePair<TableType, uint>(TableType.ExportedType, 2));

        public static CodedIndexDef CustomAttributeType = new CodedIndexDef(CodedIndex.CustomAttributeType, "CustomAttributeType", 3,
                new KeyValuePair<TableType, uint>(TableType.MethodDef, 2),
                new KeyValuePair<TableType, uint>(TableType.MemberRef, 3));

        public static CodedIndexDef ResolutionScope = new CodedIndexDef(CodedIndex.ResolutionScope, "ResolutionScope", 2,
                new KeyValuePair<TableType, uint>(TableType.Module, 0),
                new KeyValuePair<TableType, uint>(TableType.ModuleRef, 1),
                new KeyValuePair<TableType, uint>(TableType.AssemblyRef, 2),
                new KeyValuePair<TableType, uint>(TableType.TypeRef, 3));

        public static CodedIndexDef TypeOrMethodDef = new CodedIndexDef(CodedIndex.TypeOrMethodDef, "TypeOrMethodDef", 1,
                new KeyValuePair<TableType, uint>(TableType.TypeDef, 0),
                new KeyValuePair<TableType, uint>(TableType.MethodDef, 1));
    }

    public enum TableType
    {
        Assembly = 0x20,
        AssemblyOS = 0x22,
        AssemblyProcessor = 0x21,
        AssemblyRef = 0x23,
        AssemblyRefOS = 0x25,
        AssemblyRefProcessor = 0x24,
        ClassLayout = 0x0f,
        Constant = 0x0b,
        CustomAttribute = 0x0c,
        DeclSecurity = 0x0e,
        ENCLog = 0x1e,
        ENCMap = 0x1f,
        EventMap = 0x12,
        Event = 0x14,
        EventPtr = 0x13,
        ExportedType = 0x27,
        Field = 0x04,
        FieldLayout = 0x10,
        FieldMarshal = 0x0d,
        FieldPtr = 0x03,
        FieldRVA = 0x1d,
        File = 0x26,
        GenericParam = 0x2a,
        GenericParamConstraint = 0x2c,
        ImplMap = 0x1c,
        InterfaceImpl = 0x09,
        ManifestResource = 0x28,
        MemberRef = 0x0a,
        MethodDef = 0x06,
        MethodImpl = 0x19,
        MethodPtr = 0x05,
        MethodSemantics = 0x18,
        MethodSpec = 0x2b,
        Module = 0x00,
        ModuleRef = 0x1a,
        NestedClass = 0x29,
        Param = 0x08,
        ParamPtr = 0x07,
        Property = 0x17,
        PropertyMap = 0x15,
        PropertyPtr = 0x16,
        StandAloneSig = 0x11,
        TypeDef = 0x02,
        TypeRef = 0x01,
        TypeSpec = 0x1b
    }
    public enum CodedIndex
    {
        TypeDefOrRef = 0,
        HasConstant = 1,
        HasCustomAttribute = 2,
        HasFieldMarshall = 3,
        HasDeclSecurity = 4,
        MemberRefParent = 5,
        HasSemantics = 6,
        MethodDefOrRef = 7,
        MemberForwarded = 8,
        Implementation = 9,
        CustomAttributeType = 10,
        ResolutionScope = 11,
        TypeOrMethodDef = 12
    }

    public class MetadataColumn
    {
        internal MetadataColumn(int idx, string n, object t)
        {
            this.idx = idx;
            this.n = n;
            this.t = t;
        }
        int idx;
        public int Index { get { return idx; } }
        string n;
        public string Name { get { return n; } }
        object t;
        public object Type { get { return t; } }
    }
    public class MetadataTable : IEnumerable<MetadataRow>
    {
        TablesHeap h;
        TableDef def;
        public MetadataTable(TablesHeap h, TableType t)
        {
            this.h = h;
            def = TableDefs.MdTableDefs[t];
            rs = new MetadataRowCollection(this);
            cols = new MetadataColumn[def.Columns.Length];
            for (int i = 0; i < def.Columns.Length; i++)
            {
                cols[i] = new MetadataColumn(i, def.Columns[i].Key, def.Columns[i].Value);
            }
        }

        public TablesHeap Heap { get { return h; } }

        internal TableDef TableDef { get { return def; } }
        public TableType Type { get { return def.Type; } }
        MetadataColumn[] cols;
        public MetadataColumn[] Columns { get { return cols; } }

        MetadataRowCollection rs;
        public MetadataRowCollection Rows { get { return rs; } }

        public IEnumerator<MetadataRow> GetEnumerator()
        {
            return rs.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return rs.GetEnumerator();
        }

        public uint GetSize(uint[] lens)
        {
            uint ret = 0;
            for (int i = 0; i < def.Columns.Length; i++)
            {
                if (def.Columns[i].Value is Type)
                {
                    if ((def.Columns[i].Value as Type).IsSubclassOf(typeof(Enum)))
                    {
                        switch (Enum.GetUnderlyingType(def.Columns[i].Value as Type).FullName)
                        {
                            case "System.Byte":
                                ret++;
                                break;
                            case "System.UInt16":
                                ret += 2;
                                break;
                            case "System.UInt32":
                                ret += 4;
                                break;
                        }
                    }
                    else
                        switch ((def.Columns[i].Value as Type).FullName)
                        {
                            case "System.Byte":
                                ret++;
                                break;
                            case "System.UInt16":
                                ret += 2;
                                break;
                            case "System.UInt32":
                                ret += 4;
                                break;
                            case "NetPE.Core.Rva":
                                ret += 4;
                                break;
                            case "NetPE.Core.Metadata.BlobToken":
                                if (Heap.Stream.Root[MetadataStreamType.Blob].Length > ushort.MaxValue)
                                    ret += 4;
                                else
                                    ret += 2;
                                break;
                            case "NetPE.Core.Metadata.GUIDToken":
                                if (Heap.Stream.Root[MetadataStreamType.GUID].Length > ushort.MaxValue)
                                    ret += 4;
                                else
                                    ret += 2;
                                break;
                            case "NetPE.Core.Metadata.StringToken":
                                if (Heap.Stream.Root[MetadataStreamType.Strings].Length > ushort.MaxValue)
                                    ret += 4;
                                else
                                    ret += 2;
                                break;
                        }
                }
                else if (def.Columns[i].Value is TableType)
                {
                    //TODO: Need check like: var·is2BytesSize·=·maxLen·<·(1·<<·(16·-·bit));??

                    if (lens[(int)def.Columns[i].Value] > ushort.MaxValue)
                        ret += 4;
                    else
                        ret += 2;
                }
                else if (def.Columns[i].Value is CodedIndex)
                {
                    ret += CodedIndexDefs.MdCodedIndexDefs[(CodedIndex)def.Columns[i].Value].GetSize(lens);
                }
            }
            return (uint)(ret * rs.Count + 4);
        }

        public MetadataRow this[int idx]
        {
            get { return rs[idx]; }
            set { rs[idx] = value; }
        }
    }
    public class MetadataRowCollection : Collection<MetadataRow>
    {
        MetadataTable tbl;
        public MetadataRowCollection(MetadataTable tbl)
        {
            this.tbl = tbl;
        }

        public MetadataTable Table { get { return tbl; } }

        protected override void ClearItems()
        {
            foreach (MetadataRow r in Items)
            {
                r.Container = null;
            }
            base.ClearItems();
        }
        protected override void InsertItem(int index, MetadataRow item)
        {
            item.Container = tbl;
            base.InsertItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            Items[index].Container = null;
            base.RemoveItem(index);
        }
        protected override void SetItem(int index, MetadataRow item)
        {
            Items[index].Container = null;
            item.Container = tbl;
            base.SetItem(index, item);
        }

        public new MetadataRow this[int idx]
        {
            get { if (idx == 0)return null; else return base[idx - 1]; }
            set { if (idx != 0)base[idx - 1] = value; }
        }
    }
    public class MetadataRow : MetadataTokenProvider
    {
        public MetadataRow(MetadataTable tbl)
            : this(tbl, true) { }
        internal MetadataRow(MetadataTable tbl,bool needInit)
            : base(tbl)
        {
            vals = new object[tbl.TableDef.Columns.Length];
            par = tbl;
            if (!needInit) return;

            BlobHeap bh = tbl.Heap.Stream.Root[MetadataStreamType.Blob].Heap as BlobHeap;
            StringsHeap sh = tbl.Heap.Stream.Root[MetadataStreamType.Strings].Heap as StringsHeap;
            GUIDHeap gh = tbl.Heap.Stream.Root[MetadataStreamType.GUID].Heap as GUIDHeap;
            foreach (MetadataColumn col in tbl.Columns)
            {
                if (col.Type is Type)
                {
                    if ((col.Type as Type).IsEnum)
                    {
                        this[col.Index] = Enum.ToObject(col.Type as Type, 0);
                    }else
                        switch ((col.Type as Type).FullName)
                        {
                            case "System.Byte":
                                this[col.Index] = (byte)0;
                                break;
                            case "System.UInt16":
                                this[col.Index] = (ushort)0;
                                break;
                            case "System.UInt32":
                                this[col.Index] = (uint)0;
                                break;
                            case "NetPE.Core.Rva":
                                this[col.Index] = (Rva)0;
                                break;
                            case "NetPE.Core.Metadata.BlobToken":
                                this[col.Index] = bh.Resolve(new MetadataToken(MetadataTokenType.Unknown, 0));
                                break;
                            case "NetPE.Core.Metadata.GUIDToken":
                                this[col.Index] = gh.Resolve(new MetadataToken(MetadataTokenType.Unknown, 0));
                                break;
                            case "NetPE.Core.Metadata.StringToken":
                                this[col.Index] = sh.Resolve(new MetadataToken(MetadataTokenType.Unknown, 0));
                                break;
                        }
                }
                else if (col.Type is TableType)
                {
                    this[col.Index] = new TableToken(tbl.Heap, new MetadataToken((MetadataTokenType)(TableType)col.Type, 0));
                }
                else if (col.Type is CodedIndex)
                {
                    this[col.Index] = new TableToken(tbl.Heap, new MetadataToken((MetadataTokenType)CodedIndexDefs.MdCodedIndexDefs[(CodedIndex)col.Type].TableTypes[0].Key, 0));
                }
            }
        }
        MetadataTable par;
        public new MetadataTable Container { get { return par; } internal set { par = value; } }
       
        object[] vals;
        public object this[int idx]
        {
            get { return vals[idx]; }
            set
            {
                switch (par.TableDef.Columns[idx].Value.GetType().FullName)
                {
                    case "System.Type":
                        if (value.GetType() != par.TableDef.Columns[idx].Value as Type)
                            throw new InvalidOperationException();
                        break;
                    case "NetPE.Core.Metadata.Tables.TableType":
                        if (!(value is TableToken) || ((value as TableToken).Token.Type != (MetadataTokenType)par.TableDef.Columns[idx].Value))
                            throw new InvalidOperationException();
                        break;
                    case "NetPE.Core.Metadata.Tables.CodedIndex":
                        if (!(value is TableToken) || !CodedIndexDefs.MdCodedIndexDefs[(CodedIndex)par.TableDef.Columns[idx].Value].HaveTableType((TableType)(value as TableToken).Token.Type))
                            throw new InvalidOperationException();
                        break;
                }
                vals[idx] = value;
            }
        }
        public object this[string name]
        {
            get { return this[par.TableDef.IndexOfColumn(name)]; }
            set { this[par.TableDef.IndexOfColumn(name)] = value; }
        }
    }
}
