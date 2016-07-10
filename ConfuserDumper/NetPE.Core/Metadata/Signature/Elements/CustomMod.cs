using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetPE.Core.Metadata.Tables;
using System.Collections.ObjectModel;
using NetPE.Core.Metadata.Heaps;

namespace NetPE.Core.Metadata.Signature
{
    public class CustomModifierCollection : Collection<CustomModElement>, ISignature
    {
        public void Read(SignatureReader rdr)
        {
            while (rdr.IsCustomModifierAhead())
            {
                CustomModElement mod = new CustomModElement();
                mod.Read(rdr);
                Items.Add(mod);
            }
        }

        public uint GetSize()
        {
            uint ret = 0;
            foreach (CustomModElement mod in Items)
            {
                ret += mod.GetSize();
            }
            return ret;
        }

        public void Write(SignatureWriter wtr)
        {
            foreach (CustomModElement mod in Items)
            {
                mod.Write(wtr);
            }
        }
    }

    public class CustomModElement : ISignature
    {
        ElementType m;
        TableToken t;
        public ElementType Modifier { get { return m; } set { SignatureHelper.ValidateModifier(value); m = value; } }
        public TableToken Type { get { return t; } set { SignatureHelper.ValidateTypeDefOfRef(value); t = value; } }

        public void Read(SignatureReader rdr)
        {
            m = rdr.ReadElementType();
            if (m != ElementType.Modifier_Optional && m != ElementType.Modifier_Required)
                throw new InvalidOperationException();
            TablesHeap h = rdr.BaseStream.Root[MetadataStreamType.Tables].Heap as TablesHeap;
            t = new TableToken(h);

            int len = Math.Max(h[TableType.TypeDef].Rows.Count, Math.Max(h[TableType.TypeRef].Rows.Count, h[TableType.TypeSpec].Rows.Count));
            uint tkn;
            if (len > ushort.MaxValue)
                tkn = rdr.ReadUInt32();
            else
                tkn = rdr.ReadUInt16();

            switch (tkn & 0x3)
            {
                case 0:
                    t.Token = new MetadataToken(MetadataTokenType.TypeDef, (uint)tkn >> 2); break;
                case 1:
                    t.Token = new MetadataToken(MetadataTokenType.TypeRef, (uint)tkn >> 2); break;
                case 2:
                    t.Token = new MetadataToken(MetadataTokenType.TypeSpec, (uint)tkn >> 2); break;
            }
        }
        public uint GetSize()
        {
            return 1 + SignatureHelper.GetCompressedSize(t.Token.Value);
        }
        public void Write(SignatureWriter wtr)
        {
            wtr.Write(m);

            TablesHeap h = wtr.BaseStream.Root[MetadataStreamType.Tables].Heap as TablesHeap;
            int len = Math.Max(h[TableType.TypeDef].Rows.Count, Math.Max(h[TableType.TypeRef].Rows.Count, h[TableType.TypeSpec].Rows.Count));
            if (len > ushort.MaxValue)
                switch (t.Token.Type)
                {
                    case MetadataTokenType.TypeDef:
                        wtr.Write((uint)(t.Token.Index << 2 | 0)); break;
                    case MetadataTokenType.TypeRef:
                        wtr.Write((uint)(t.Token.Index << 2 | 1)); break;
                    case MetadataTokenType.TypeSpec:
                        wtr.Write((uint)(t.Token.Index << 2 | 2)); break;
                }
            else
                switch (t.Token.Type)
                {
                    case MetadataTokenType.TypeDef:
                        wtr.Write((ushort)(t.Token.Index << 2 | 0)); break;
                    case MetadataTokenType.TypeRef:
                        wtr.Write((ushort)(t.Token.Index << 2 | 1)); break;
                    case MetadataTokenType.TypeSpec:
                        wtr.Write((ushort)(t.Token.Index << 2 | 2)); break;
                }

        }
    }
}
