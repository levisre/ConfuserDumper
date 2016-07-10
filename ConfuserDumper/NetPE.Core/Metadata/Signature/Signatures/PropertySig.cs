using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature
{
    public class PropertySig : ISignature
    {
        bool ht;
        public bool HasThis { get { return ht; } set { ht = value; } }
        CustomModifierCollection mods;
        public CustomModifierCollection Modifiers { get { return mods; } }
        TypeElement t;
        public TypeElement Type { get { return t; } set { t = value; } }
        ParamCollection ps;
        public ParamCollection Params { get { return ps; } }

        public void Read(SignatureReader rdr)
        {
            byte p = rdr.ReadByte();
            if (p == 0x8)       //Normal  PROPERTY prolog
                ht = false;
            else if (p == 0x28) //HASTHIS PROPERTY prolog
                ht = true;
            else throw new InvalidOperationException();

            int pc = rdr.ReadCompressedInt();
            mods = new CustomModifierCollection();
            mods.Read(rdr);
            t = TypeElement.ReadType(rdr);
            ps = new ParamCollection();
            ps.Read(rdr, pc);
        }

        public uint GetSize()
        {
            return 1 + SignatureHelper.GetCompressedSize(mods.Count) + mods.GetSize() + t.GetSize() + ps.GetSize();
        }

        public void Write(SignatureWriter wtr)
        {
            if (ht)
                wtr.Write((byte)0x28);
            else
                wtr.Write((byte)0x08);
            wtr.WriteCompressedInt(ps.Count);
            mods.Write(wtr);
            t.Write(wtr);
            ps.Write(wtr);
        }
    }
}
