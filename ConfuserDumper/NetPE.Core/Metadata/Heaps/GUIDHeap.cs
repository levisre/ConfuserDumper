using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core.Metadata.Heaps
{
    public class GUIDHeap : Heap<GUIDToken,Guid>
    {
        public GUIDHeap(MetadataStream str) : base(str) { }

        public override GUIDToken NewChildCore(uint len)
        {
            GUIDToken ret = new GUIDToken(this);
            ret.Token = new MetadataToken(MetadataTokenType.Unknown, (uint)this.Stream.Data.Length);
            this.Stream.SetLength(this.Stream.Data.Length + 16);
            return ret;
        }

        public override uint GetValueLen(GUIDToken tkn)
        {
            return 16;
        }

        public override Guid GetValue(GUIDToken tkn)
        {
            MetadataStream strs = this.Stream;
            if (tkn.Token.Index == 0) return new Guid();
            MetadataReader rdr = new MetadataReader(strs);
            rdr.BaseStream.Position = (tkn.Token.Index - 1) * 16;

            return new Guid(rdr.ReadBytes(16));
        }

        public override void SetValue(GUIDToken tkn, Guid val)
        {
            if (tkn.Token.Index == 0) return;
            MetadataStream strs = this.Stream;
            MetadataWriter wtr = new MetadataWriter(strs);
            strs.Position = (tkn.Token.Index - 1) * 16;
            wtr.Write(val.ToByteArray());
        }
    }
}
