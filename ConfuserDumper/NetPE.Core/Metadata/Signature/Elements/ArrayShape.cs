using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature
{
    public class ArrayShapeElement : ISignature
    {
        int r;
        public int Rank { get { return r; } set { r = value; } }
        int[] s;
        public int[] Sizes { get { return s; } set { s = value; } }
        int[] lb;
        public int[] LoBounds { get { return lb; } set { lb = value; } }

        public void Read(SignatureReader rdr)
        {
            r = rdr.ReadCompressedInt();
            s = new int[rdr.ReadCompressedInt()];
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = rdr.ReadCompressedInt();
            }
            lb = new int[rdr.ReadCompressedInt()];
            for (int i = 0; i < s.Length; i++)
            {
                lb[i] = rdr.ReadCompressedInt();
            }
        }

        public uint GetSize()
        {
            uint ret = 0;
            ret += SignatureHelper.GetCompressedSize(r);
            ret += SignatureHelper.GetCompressedSize((uint)s.Length);
            foreach (uint i in s)
            {
                ret += SignatureHelper.GetCompressedSize(i);
            }
            ret += SignatureHelper.GetCompressedSize((uint)lb.Length);
            foreach (uint i in lb)
            {
                ret += SignatureHelper.GetCompressedSize(i);
            }
            return ret;
        }

        public void Write(SignatureWriter wtr)
        {
            wtr.WriteCompressedInt(r);
            wtr.WriteCompressedInt(s.Length);
            foreach (int i in s)
            {
                wtr.WriteCompressedInt(i);
            }
            wtr.WriteCompressedInt(lb.Length);
            foreach (int i in lb)
            {
                wtr.WriteCompressedInt(i);
            }
        }
    }
}
