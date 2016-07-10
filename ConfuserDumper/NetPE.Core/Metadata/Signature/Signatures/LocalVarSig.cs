using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace NetPE.Core.Metadata.Signature
{
    public class LocalVarCollection : Collection<LocalVar>, ISignature
    {
        public void Read(SignatureReader rdr)
        {
            int c = rdr.ReadCompressedInt();
            for (int i = 0; i < c; i++)
            {
                LocalVar v = new LocalVar();
                v.Read(rdr);
                Items.Add(v);
            }
        }

        public uint GetSize()
        {
            uint ret = SignatureHelper.GetCompressedSize(this.Count);
            foreach (LocalVar v in Items)
            {
                ret += v.GetSize();
            }
            return ret;
        }

        public void Write(SignatureWriter wtr)
        {
            wtr.WriteCompressedInt(this.Count);
            foreach (LocalVar v in Items)
            {
                v.Write(wtr);
            }
        }
    }
    public class LocalVar : ISignature
    {
        LocalVarModCollection mod;
        public LocalVarModCollection Modifier { get { return mod; } }
        bool br;
        public bool ByRef { get { return br; } set { br = value; } }
        TypeElement e;
        public TypeElement Type { get { return e; } set { e = value; } }


        public void Read(SignatureReader rdr)
        {
            if (rdr.GetElementTypeAhead() != ElementType.TypedByRef)
            {
                mod = new LocalVarModCollection();
                mod.Read(rdr);
                if (rdr.GetElementTypeAhead() == ElementType.ByRef)
                    br = true;
            }
            e = TypeElement.ReadType(rdr);
        }

        public uint GetSize()
        {
            return e.Element == ElementType.TypedByRef ? 1 : (mod.GetSize() + (br ? 1U : 0) + e.GetSize());
        }

        public void Write(SignatureWriter wtr)
        {
            if (e.Element != ElementType.TypedByRef)
            {
                mod.Write(wtr);
                if (br) wtr.Write(ElementType.ByRef);
            }
            e.Write(wtr);
        }
    }
    public class LocalVarModCollection : Collection<LocalVarMod>, ISignature
    {
        public void Read(SignatureReader rdr)
        {
            while (rdr.IsCustomModifierAhead() || (byte)rdr.GetElementTypeAhead() == 0x45) //0x45=ELEMENT_TYPE_PINNED
            {
                LocalVarMod m = new LocalVarMod();
                if (rdr.IsCustomModifierAhead())
                {
                    m.Modifiers = new CustomModifierCollection();
                    m.Modifiers.Read(rdr);
                }
                if ((byte)rdr.GetElementTypeAhead() == 0x45)
                {
                    m.Pinned = true;
                }
                Items.Add(m);
            }
        }

        public uint GetSize()
        {
            uint ret = 0;
            foreach (LocalVarMod m in Items)
            {
                if (m.Modifiers != null)
                    ret += m.Modifiers.GetSize();
                if (m.Pinned)
                    ret++;
            }
            return ret;
        }

        public void Write(SignatureWriter wtr)
        {
            foreach (LocalVarMod m in Items)
            {
                if (m.Modifiers != null)
                    m.Modifiers.Write(wtr);
                if (m.Pinned)
                    wtr.Write((byte)0x45);
            }
        }
    }
    public class LocalVarMod
    {
        CustomModifierCollection mod;
        public CustomModifierCollection Modifiers { get { return mod; } internal set { mod = value; } }
        bool pin;
        public bool Pinned { get { return pin; } set { pin = value; } }
    }
    public class LocalVarSig : ISignature
    {
        byte pl;
        public byte Prolog { get { return pl; } set { pl = value; } }
        LocalVarCollection vars;
        public LocalVarCollection LocalVars { get { return vars; } }


        public void Read(SignatureReader rdr)
        {
            pl = rdr.ReadByte();
            vars = new LocalVarCollection();
            vars.Read(rdr);
        }

        public uint GetSize()
        {
            return 1 + vars.GetSize();
        }

        public void Write(SignatureWriter wtr)
        {
            wtr.Write(pl);
            vars.Write(wtr);
        }
    }
}
