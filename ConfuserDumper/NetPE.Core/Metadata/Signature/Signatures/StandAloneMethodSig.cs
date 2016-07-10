using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature
{
    public class StandAloneMethodSig : ISignature
    {
        MethodFlags f;
        public MethodFlags Flags { get { return f; } set { f = value; } }
        int gc;
        public int GenericParamCount { get { if ((f & MethodFlags.Generic) != MethodFlags.Generic)throw new InvalidOperationException(); return gc; } set { if ((f & MethodFlags.Generic) != MethodFlags.Generic)throw new InvalidOperationException(); gc = value; } }
        RetTypeElement rt;
        public RetTypeElement RetType { get { return rt; } set { rt = value; } }
        ParamCollection ps;
        public ParamCollection Params { get { return ps; } }

        public void Read(SignatureReader rdr)
        {
            f = (MethodFlags)rdr.ReadByte();
            if ((f & MethodFlags.Generic) == MethodFlags.Generic)
                gc = rdr.ReadCompressedInt();
            int c = rdr.ReadCompressedInt();
            rt = new RetTypeElement();
            rt.Read(rdr);
            ps = new ParamCollection();
            ps.Read(rdr, c);
        }

        public uint GetSize()
        {
            return 1 + ((f & MethodFlags.Generic) == MethodFlags.Generic ? SignatureHelper.GetCompressedSize(gc) : 0) + rt.GetSize() + ps.GetSize();
        }

        public void Write(SignatureWriter wtr)
        {
            wtr.Write((byte)f);
            if ((f & MethodFlags.Generic) == MethodFlags.Generic)
                wtr.WriteCompressedInt(gc);
            wtr.WriteCompressedInt(ps.Count);
            rt.Write(wtr);
            ps.Write(wtr);
        }
    }
}
