using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature.Types
{
    public class PTR : TypeElement
    {
        CustomModifierCollection mods;
        public CustomModifierCollection Modifiers { get { return mods; } }
        TypeElement t;
        public TypeElement Type { get { return t; } set { t = value; } }

        public override void Read(SignatureReader rdr)
        {
            Element = rdr.ReadElementType();
            mods = new CustomModifierCollection();
            mods.Read(rdr);
            t = TypeElement.ReadType(rdr);
        }

        public override uint GetSize()
        {
            return 1 + mods.GetSize() + t.GetSize();
        }

        public override void Write(SignatureWriter wtr)
        {
            wtr.Write(Element);
            mods.Write(wtr);
            t.Write(wtr);
        }
    }
}
