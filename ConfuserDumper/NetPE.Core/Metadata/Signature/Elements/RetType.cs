using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature
{
    public class RetTypeElement : ISignature
    {
        CustomModifierCollection mods;
        public CustomModifierCollection Modifiers { get { return mods; } }
        bool br;
        public bool IsByRef { get { return br; } set { br = value; } }
        TypeElement t;
        public TypeElement Type { get { return t; } set { t = value; } }

        public void Read(SignatureReader rdr)
        {
            mods = new CustomModifierCollection();
            mods.Read(rdr);
            if (rdr.GetElementTypeAhead() == ElementType.ByRef)
                br = true;
            else
                br = false;
            t = TypeElement.ReadType(rdr);
        }

        public uint GetSize()
        {
            return mods.GetSize() + (br ? 1U : 0) + t.GetSize();
        }

        public void Write(SignatureWriter wtr)
        {
            mods.Write(wtr);
            if (br) wtr.Write(ElementType.ByRef);
            t.Write(wtr);
        }
    }
}
