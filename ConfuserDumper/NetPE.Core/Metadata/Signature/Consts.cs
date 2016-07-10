using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Core.Metadata.Signature
{
    [Flags]
    public enum MethodFlags : byte
    {
        HasThis = 0x20,
        ExplicitThis = 0x40,
        Default = 0x00,
        VarArg = 0x05,
        Generic = 0x10
    }

    public enum ElementType : byte
    {
        End = 0x0,
        Void = 0x1,
        Boolean = 0x2,
        Char = 0x3,
        Int8 = 0x4,
        UInt8 = 0x5,
        Int16 = 0x6,
        UInt16 = 0x7,
        Int32 = 0x8,
        UInt32 = 0x9,
        Int64 = 0xa,
        UInt64 = 0xb,
        Single = 0xc,
        Double = 0xd,
        String = 0xe,

        Pointer = 0xf,
        ByRef = 0x10,

        ValueType = 0x11,
        Class = 0x12,
        GenericVar = 0x13,
        Array = 0x14,
        GenericInstance = 0x15,
        TypedByRef = 0x16,

        IntPtr = 0x18,
        UIntPtr = 0x19,
        FnPtr = 0x1B,
        Object = 0x1C,
        SzArray = 0x1D,
        MethodGenericVar = 0x1e,

        Modifier_Required = 0x1F,
        Modifier_Optional = 0x20,

        Internal = 0x21
    }
}
