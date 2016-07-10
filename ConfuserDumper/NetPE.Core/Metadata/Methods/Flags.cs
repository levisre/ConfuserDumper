using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Methods
{
    [Flags]
    public enum HeaderFlags
    {
        FatFormat = 0x03,
        TinyFormat = 0x02,
        MoreSects = 0x08,
        InitLocals = 0x10
    }

    [Flags]
    public enum SectionFlags
    {
        EHTable = 0x01,
        OptILTable = 0x02,
        FatFormat = 0x40,
        MoreSects = 0x80
    }

    [Flags]
    public enum ExceptionClauseFlags
    {
        Exception = 0x0,
        Filter = 0x1,
        Finally = 0x2,
        Fault = 0x4
    }
}
