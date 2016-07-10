using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Metadata.Tables;
using NetPE.Core.Metadata.Heaps;

namespace NetPE.Core.Metadata.Signature
{

    public class SignatureReader : MetadataReader
    {
        public SignatureReader(MetadataReader rdr) : base(rdr.BaseStream) { }

        public ElementType GetElementTypeAhead()
        {
            ElementType t = (ElementType)ReadByte();
            BaseStream.Position -= 1;
            return t;
        }

        public bool IsCustomModifierAhead()
        {
            ElementType t = GetElementTypeAhead();
            return (t == ElementType.Modifier_Optional) || (t == ElementType.Modifier_Required);
        }

        public ElementType ReadElementType()
        {
            return (ElementType)ReadByte();
        }

        public TableToken ReadTypeDefOrRefEncoded()
        {
            TablesHeap h = BaseStream.Root[MetadataStreamType.Tables].Heap as TablesHeap;
            uint tkn = (uint)base.Read7BitEncodedInt();
            switch (tkn & 0x3)
            {
                case 0:
                    return new TableToken(h) { Token = new MetadataToken(MetadataTokenType.TypeDef, (uint)(tkn >> 2)) };
                case 1:
                    return new TableToken(h) { Token = new MetadataToken(MetadataTokenType.TypeRef, (uint)(tkn >> 2)) };
                case 2:
                    return new TableToken(h) { Token = new MetadataToken(MetadataTokenType.TypeSpec, (uint)(tkn >> 2)) };
            }
            throw new InvalidOperationException();
        }
    }
    public class SignatureWriter : MetadataWriter
    {
        public SignatureWriter(MetadataWriter rdr) : base(rdr.BaseStream) { }

        public void Write(ElementType val)
        {
            Write((byte)val);
        }

        public void WriteTypeDefOrRefEncoded(TableToken tkn)
        {
            TablesHeap h = BaseStream.Root[MetadataStreamType.Tables].Heap as TablesHeap;
            switch (tkn.Token.Type)
            {
                case MetadataTokenType.TypeDef:
                    base.Write7BitEncodedInt((int)(tkn.Token.Index << 2 | 0)); break;
                case MetadataTokenType.TypeRef:
                    base.Write7BitEncodedInt((int)(tkn.Token.Index << 2 | 1)); break;
                case MetadataTokenType.TypeSpec:
                    base.Write7BitEncodedInt((int)(tkn.Token.Index << 2 | 2)); break;
            }
        }
    }
}
