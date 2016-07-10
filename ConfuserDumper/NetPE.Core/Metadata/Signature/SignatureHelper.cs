using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature
{
    internal static class SignatureHelper
    {
        public static void ValidateTypeDefOfRef(TableToken tkn)
        {
            if (tkn.Token.Type != MetadataTokenType.TypeDef && tkn.Token.Type != MetadataTokenType.TypeRef && tkn.Token.Type != MetadataTokenType.TypeSpec)
                throw new InvalidOperationException();
        }

        public static void ValidateModifier(ElementType t)
        {
            if (t != ElementType.Modifier_Optional && t != ElementType.Modifier_Required)
                throw new InvalidOperationException();
        }

        public static uint GetCompressedSize(int s)
        {
            return GetCompressedSize((uint)s);
        }

        public static uint GetCompressedSize(uint s)
        {
            if ((0 < s) && (s <= 0x7f))
                return 1;
            else if ((0x80 < s) && (s <= 0x3fff))
                return 2;
            else
                return 4;
        }
    }
}
