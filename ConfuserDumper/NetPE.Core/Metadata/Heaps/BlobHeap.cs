using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core.Metadata.Heaps
{
    public class BlobHeap : Heap<BlobToken, byte[]>
    {
        public BlobHeap(MetadataStream str) : base(str) { }

        public override BlobToken NewChildCore(uint len)
        {
            BlobToken ret = new BlobToken(this);
            ret.Token = new MetadataToken(MetadataTokenType.Unknown, (uint)this.Stream.Data.Length);

            uint datLen = len;
            if (len < 0x80)
                datLen += 1;
            else if (len < 0x4000)
                datLen += 2;
            else
                datLen += 4;
            this.Stream.SetLength(this.Stream.Data.Length + datLen);

            MetadataWriter wtr = new MetadataWriter(this.Stream);
            this.Stream.Position = ret.Token.Index;
            wtr.WriteCompressedUInt(len);
            return ret;
        }

        public override uint GetValueLen(BlobToken tkn)
        {
            MetadataStream strs = this.Stream;
            strs.Position = tkn.Token.Index;
            MetadataReader rdr = new MetadataReader(strs);
            uint len = rdr.ReadCompressedUInt();
            if (len < 0x80)
                len += 1;
            else if (len < 0x4000)
                len += 2;
            else
                len += 4;

            return len;
        }

        public override byte[] GetValue(BlobToken tkn)
        {
            MetadataStream strs = this.Stream;
            MetadataReader rdr = new MetadataReader(strs);
            rdr.BaseStream.Position = tkn.Token.Index;

            int c = rdr.ReadCompressedInt();
            return rdr.ReadBytes(c);
        }

        public override void SetValue(BlobToken tkn, byte[] val)
        {
            MetadataStream strs = this.Stream;
            strs.Position = tkn.Token.Index;
            MetadataReader rdr = new MetadataReader(strs);
            uint len = rdr.ReadCompressedUInt();

            MetadataWriter wtr = new MetadataWriter(strs);
            if (len == val.Length)
            {
                strs.Position = tkn.Token.Index;
                wtr.WriteCompressedUInt(len);
                wtr.Write(val);
            }
            else
            {
                ResizeChild(tkn, (uint)val.Length);
                strs.Position = tkn.Token.Index;
                wtr.WriteCompressedInt(val.Length);
                wtr.Write(val);
            }
        }
    }
}