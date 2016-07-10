using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Metadata;

namespace NetPE.Disasm.CIL
{
    public class Operand
    {
        internal Operand(object val) { this.val = val; }

        object val;
        public object Value
        {
            get
            {
                return val;
            }
        }

        public override string ToString()
        {
            if (val is MetadataToken)
                return "[0x" + (val as MetadataToken).Value.ToString("x8").ToUpper() + "]";
            else if (val is Instruction)
                return (val as Instruction).GetOffsetString();
            else if (val is Instruction[])
            {
                StringBuilder ret = new StringBuilder();
                foreach (Instruction i in val as Instruction[])
                {
                    if (ret.Length > 0)
                        ret.Append(", ");
                    if (i != null)
                        ret.Append(i.GetOffsetString());
                    else
                    {
                        // sometime instruction is null b.c the switch point to an invalid offset (maybe cause by obfuscators)
                        ret.Append("?");
                        //TODO: Should write the originate offset
                    }
                }
                //return ret.ToString().Substring(0, ret.Length - 2);
                return ret.ToString();
            }
            else if (val is float || val is double)
                return string.Format("{0}", val);
            else
                return string.Format("0x{0:x}", val);
        }
    }
}
