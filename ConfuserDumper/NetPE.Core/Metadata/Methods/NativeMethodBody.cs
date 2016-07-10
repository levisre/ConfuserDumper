using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetPE.Core.Metadata.Tables;

namespace NetPE.Core.Metadata.Methods
{
    public class NativeMethodBody : MethodBody
    {
        public NativeMethodBody(MetadataRow par) : base(par) { }

        byte[] c;
        public byte[] Codes { get { return c; } set { c = value; } }

        public override uint Size
        {
            get { return (uint)c.Length; }
        }

        public override void Load(BinaryReader rdr)
        {
            c = NativeHelper.GetNativeCodes(rdr);
        }

        public override void Save(BinaryWriter wtr)
        {
            wtr.Write(c);
        }
    }
}
