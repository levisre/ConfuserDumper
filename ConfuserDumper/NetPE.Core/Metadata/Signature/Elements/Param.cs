using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace NetPE.Core.Metadata.Signature
{
    public class ParamCollection : Collection<ParamElement>
    {
        int sen;
        public int Sentinel { get { return sen; } set { sen = value; } }  //optional beginning, including

        public void Read(SignatureReader rdr, int c)
        {
            sen = -1;
            for (int i = 0; i < c; i++)
            {
                if ((byte)rdr.GetElementTypeAhead() == 0x41)
                {
                    sen = i;
                    rdr.ReadElementType();
                }
                ParamElement p = new ParamElement();
                p.Read(rdr);
                Items.Add(p);
            }
        }

        public uint GetSize()
        {
            uint ret = SignatureHelper.GetCompressedSize(this.Count) + (sen != -1 ? 1U : 0);
            foreach (ParamElement p in Items)
            {
                ret += p.GetSize();
            }
            return ret;
        }

        public void Write(SignatureWriter wtr)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (i == sen)
                    wtr.Write((byte)0x41);
                Items[i].Write(wtr);
            }
        }
    }

    public class ParamElement : ISignature
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
