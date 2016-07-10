using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Disasm.x86.libdasm;
using System.Collections.ObjectModel;

namespace NetPE.Disasm.x86
{
    public class InstructionCollection : Collection<Instruction>
    {
        public uint ComputeOffset(Instruction inst)
        {
            uint ret = 0;
            IEnumerator<Instruction> e = this.GetEnumerator();
            while (e.MoveNext() && e.Current != inst)
            {
                ret += e.Current.InsturctionInternal.length;
            }
            return ret;
        }
    }
}
