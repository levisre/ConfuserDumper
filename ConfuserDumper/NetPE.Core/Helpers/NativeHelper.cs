using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core
{
    internal static class NativeHelper
    {
        public static byte[] GetNativeCodes(BinaryReader rdr)
        {
            List<byte> ret = new List<byte>();
            while (true)
            {
                byte c = rdr.ReadByte();
                if (c == 0xc3)
                {
                    ret.Add(c);
                    break;
                }
                else if (c == 0xc2)
                {
                    ret.Add(c);
                    ret.Add(rdr.ReadByte());
                    ret.Add(rdr.ReadByte());
                    break;
                }
                ret.Add(c);

                //TODO: EOF is reach before the body is complete
                if (rdr.BaseStream.Position >= rdr.BaseStream.Length)
                {
                    //TODO: Just return or throw exception?
                    throw new InvalidOperationException("Invalid native body size (readsize=" + rdr.BaseStream.Length +")");
                }
            }
            return ret.ToArray();
        }
    }
}
