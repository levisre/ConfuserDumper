using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using NetPE.Core.Metadata.Signature.Types;

namespace NetPE.Core.Metadata.Signature
{
    public class TypeElementCollection : Collection<TypeElement>, ISignature
    {
        public void Read(SignatureReader rdr)
        {
            int c = rdr.ReadCompressedInt();
            for (int i = 0; i < c; i++)
            {
                this.Add(TypeElement.ReadType(rdr));
            }
        }

        public uint GetSize()
        {
            uint ret = SignatureHelper.GetCompressedSize(this.Count);
            foreach (TypeElement t in Items)
            {
                ret += t.GetSize();
            }
            return ret;
        }

        public void Write(SignatureWriter wtr)
        {
            wtr.WriteCompressedInt(this.Count);
            foreach (TypeElement t in Items)
            {
                t.Write(wtr);
            }
        }
    }

    public abstract class TypeElement : ISignature
    {
        ElementType t;
        public ElementType Element { get { return t; } set { t = value; } }

        public abstract void Read(SignatureReader rdr);

        public static TypeElement ReadType(SignatureReader rdr)
        {
            TypeElement ret;
            switch (rdr.GetElementTypeAhead())
            {
                case ElementType.Void:
                case ElementType.TypedByRef:
                    ret = new BaseType(); break;
                case ElementType.Boolean:
                case ElementType.Char:
                case ElementType.Int8:
                case ElementType.UInt8:
                case ElementType.Int16:
                case ElementType.UInt16:
                case ElementType.Int32:
                case ElementType.UInt32:
                case ElementType.Int64:
                case ElementType.UInt64:
                case ElementType.Single:
                case ElementType.Double:
                case ElementType.IntPtr:
                case ElementType.UIntPtr:
                case ElementType.Object:
                case ElementType.String:
                    ret = new BaseType(); break;
                case ElementType.Array:
                    ret = new ARRAY(); break;
                case ElementType.Class:
                    ret = new CLASS(); break;
                case ElementType.FnPtr:
                    ret = new FNPTR(); break;
                case ElementType.GenericInstance:
                    ret = new GENERICINST(); break;
                case ElementType.MethodGenericVar:
                    ret = new MVAR(); break;
                case ElementType.Pointer:
                    ret = new PTR(); break;
                case ElementType.SzArray:
                    ret = new SZARRAY(); break;
                case ElementType.ValueType:
                    ret = new VALUETYPE(); break;
                case ElementType.GenericVar:
                    ret = new VAR(); break;
                default:
                    throw new InvalidOperationException();
            }
            ret.Read(rdr);
            return ret;
        }

        public abstract uint GetSize();

        public abstract void Write(SignatureWriter wtr);
    }
}
