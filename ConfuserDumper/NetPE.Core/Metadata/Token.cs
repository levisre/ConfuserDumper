using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata
{
    public enum MetadataTokenType
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
        Permission = 0x0e,
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
        Method = 0x06,
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
        Signature = 0x11,
        TypeDef = 0x02,
        TypeRef = 0x01,
        TypeSpec = 0x1b,

        String = 0x70,
        Name = 0x71,
        Unknown = 0x00
    }

    public class MetadataToken
    {
        public MetadataToken(uint tkn) { this.tkn = tkn; }
        public MetadataToken(MetadataTokenType t, uint idx) { Type = t; Index = idx; }

        uint tkn;
        public uint Value { get { return tkn; } set { tkn = value; } }
        public MetadataTokenType Type { get { return (MetadataTokenType)(tkn >> 24); } set { tkn = (tkn | 0xff000000) & ((uint)value << 24); } }
        public uint Index { get { return tkn & 0x00ffffff; } set { if (value > 0x00ffffff)throw new InvalidOperationException(); tkn = ((tkn & 0xff000000) | (0x00ffffff & value)); } }
            
        public static implicit operator uint(MetadataToken tkn)
        {
            return tkn.tkn;
        }

        public static implicit operator MetadataToken(uint tkn)
        {
            return new MetadataToken(tkn);
        }

        public override string ToString()
        {
            return string.Format("[{0} 0x{1}] {2}", Type.ToString(), Index.ToString("x"), tkn.ToString("x8"));
        }
    }
}
