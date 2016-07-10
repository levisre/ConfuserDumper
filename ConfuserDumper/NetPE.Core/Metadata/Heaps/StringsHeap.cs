using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core.Metadata.Heaps
{
    public class StringsHeap : Heap<StringToken, string>
    {
        public StringsHeap(MetadataStream str) : base(str) { }

        public override StringToken NewChildCore(uint len)
        {
            StringToken ret = new StringToken(this);
            ret.Token = new MetadataToken(MetadataTokenType.Unknown, (uint)this.Stream.Data.Length);

            this.Stream.SetLength(this.Stream.Data.Length + len + 1);

            MetadataWriter wtr = new MetadataWriter(this.Stream);
            this.Stream.Position = ret.Token.Index;
            for (int i = 0; i < len; i++) wtr.Write((byte)0xff);
            wtr.Write((byte)0);
            return ret;
        }

        public override uint GetValueLen(StringToken tkn)
        {
            MetadataStream strs = this.Stream;
            uint idx = tkn.Token.Index;
            uint c = 0;
            do c++; while (strs.Data[idx + c] != 0);

            return c + 1;
        }

        public override string GetValue(StringToken tkn)
        {
            MetadataStream strs = this.Stream;
            int idx = (int)tkn.Token.Index;
            if (idx == 0)
                return string.Empty;
            int c = 0;
            do c++; while (strs.Data[idx + c] != 0);
            return Encoding.UTF8.GetString(strs.Data, idx, c);
        }

        public override void SetValue(StringToken tkn, string val)
        {
            MetadataStream strs = this.Stream;
            uint idx = tkn.Token.Index;
            uint c = 0;
            while (strs.Data[idx + c] != 0) c++; c++;

            byte[] dat = Encoding.UTF8.GetBytes(val);
            MetadataWriter wtr = new MetadataWriter(strs);
            if (c == dat.Length + 1)
            {
                strs.Position = tkn.Token.Index;
                wtr.Write(dat);
            }
            else
            {
                ResizeChild(tkn, (uint)dat.Length + 1);
                strs.Position = tkn.Token.Index;
                wtr.Write(dat);
                wtr.Write((byte)0);
            }
        }
    }
}
