using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core.Metadata.Heaps
{
    public class USHeap : Heap<USToken, string>
    {
        public USHeap(MetadataStream str) : base(str) { }

        public override USToken NewChildCore(uint len)
        {
            USToken ret = new USToken(this);
            ret.Token = new MetadataToken(MetadataTokenType.String, (uint)this.Stream.Data.Length);

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

        public override uint GetValueLen(USToken tkn)
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

        public override string GetValue(USToken tkn)
        {
            MetadataStream strs = this.Stream;
            MetadataReader rdr = new MetadataReader(strs);
            rdr.BaseStream.Position = tkn.Token.Index;
 
            //int c = rdr.ReadCompressedInt();
            //return Encoding.Unicode.GetString(rdr.ReadBytes(c));

            /*
            #US Array of unicode strings. The name stands for User Strings, 
             * and these strings are referenced directly by code instructions (ldstr). 
             * This stream starts with a null byte exactly like the #Blob one. 
             * Each entry of this stream begins with a 7bit encoded integer which tells us the size 
             * of the following string (the size is in bytes, not characters). 
             * Moreover, there's an additional byte at the end of the string (so that every string size is odd and not even). 
             * This last byte tells the framework if any of the characters in the string 
             * has its high byte set or if the low byte is any of these particular values: 
             * 0x01–0x08, 0x0E–0x1F, 0x27, 0x2D.
            */

            //TODO: Check if SetValue need to modify?

            // size is 7bit
            var length = (int)(rdr.ReadCompressedUInt() & ~1);
            if (length < 1)
                return string.Empty;

            var data = rdr.ReadBytes(length);
            var chars = new char[length / 2];
            for (int i = 0, j = 0; i < length; i += 2)
                chars[j++] = (char)(data[i] | (data[i + 1] << 8));
            return new string(chars); 
        }

        public override void SetValue(USToken tkn, string val)
        {
            MetadataStream strs = this.Stream;
            strs.Position = tkn.Token.Index;
            MetadataReader rdr = new MetadataReader(strs);
            uint len = rdr.ReadCompressedUInt();

            MetadataWriter wtr = new MetadataWriter(strs);
            byte[] b = Encoding.Unicode.GetBytes(val);
            if (len == b.Length)
            {
                strs.Position = tkn.Token.Index;
                wtr.WriteCompressedUInt(len);
                wtr.Write(b);
            }
            else
            {
                ResizeChild(tkn, (uint)b.Length);
                strs.Position = tkn.Token.Index;
                wtr.WriteCompressedInt(b.Length);
                wtr.Write(b);
            }
        }
    }
}
