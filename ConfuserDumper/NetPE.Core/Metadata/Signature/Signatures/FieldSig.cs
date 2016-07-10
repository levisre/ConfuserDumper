using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature
{
    public class FieldSig : ISignature
    {
        byte pl;
        public byte Prolog { get { return pl; } set { pl = value; } }
        CustomModifierCollection mods;
        public CustomModifierCollection Modifiers { get { return mods; } set { mods = value; } }
        TypeElement t;
        public TypeElement Type { get { return t; } set { t = value; } }

        public void Read(SignatureReader rdr)
        {
            pl = rdr.ReadByte();
            mods = new CustomModifierCollection();
            mods.Read(rdr);
            t = TypeElement.ReadType(rdr);
        }

        public uint GetSize()
        {
            return 1 + mods.GetSize() + t.GetSize();
        }

        public void Write(SignatureWriter wtr)
        {
            wtr.Write(pl);
            mods.Write(wtr);
            t.Write(wtr);
        }
    }
}
