using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core
{
    internal static class StampsHelper
    {
        public static DateTime StampFromUInt(uint val)
        {
            return new DateTime(1970, 1, 1).AddSeconds(val);
        }
        public static uint UIntFromStamp(DateTime val)
        {
            return (uint)val.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
