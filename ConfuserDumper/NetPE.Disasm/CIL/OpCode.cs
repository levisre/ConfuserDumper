using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Disasm.CIL
{
    public enum FlowControl
    {
        Branch,
        Break,
        Call,
        Cond_Branch,
        Meta,
        Next,
        Phi,
        Return,
        Throw
    }
    public enum OpCodeType
    {
        Annotation,
        Macro,
        Nternal,
        Objmodel,
        Prefix,
        Primitive
    }
    public enum OperandType
    {
        InlineBrTarget,
        InlineField,
        InlineI,
        InlineI8,
        InlineMethod,
        InlineNone,
        InlinePhi,
        InlineR,
        InlineSig,
        InlineString,
        InlineSwitch,
        InlineTok,
        InlineType,
        InlineVar,
        InlineParam,
        ShortInlineBrTarget,
        ShortInlineI,
        ShortInlineR,
        ShortInlineVar,
        ShortInlineParam
    }
    public enum StackBehaviour
    {
        Pop0,
        Pop1,
        Pop1_pop1,
        Popi,
        Popi_pop1,
        Popi_popi,
        Popi_popi8,
        Popi_popi_popi,
        Popi_popr4,
        Popi_popr8,
        Popref,
        Popref_pop1,
        Popref_popi,
        Popref_popi_popi,
        Popref_popi_popi8,
        Popref_popi_popr4,
        Popref_popi_popr8,
        Popref_popi_popref,
        PopAll,
        Push0,
        Push1,
        Push1_push1,
        Pushi,
        Pushi8,
        Pushr4,
        Pushr8,
        Pushref,
        Varpop,
        Varpush
    }

    public class OpCode
    {
        internal OpCode(string m, ushort c, FlowControl ctrl, OpCodeType ct, OperandType dt, StackBehaviour popb, StackBehaviour pushb)
        {
            this.m = m;
            this.c = c;
            this.ctrl = ctrl;
            this.ct = ct;
            this.dt = dt;
            this.popb = popb;
            this.pushb = pushb;

            OpCodes.Opcodes.Add(c, this);
        }


        string m;
        public string Mnemonic
        {
            get { return m; }
        }

        ushort c;
        public ushort Code { get { return c; } }

        FlowControl ctrl;
        public FlowControl FlowControl { get { return ctrl; } }

        OpCodeType ct;
        public OpCodeType OpCodeType { get { return ct; } }

        OperandType dt;
        public OperandType OperandType { get { return dt; } }

        StackBehaviour popb;
        public StackBehaviour StackBehaviourPop { get { return popb; } }

        StackBehaviour pushb;
        public StackBehaviour StackBehaviourPush { get { return pushb; } }

        public override string ToString()
        {
            return m;
        }
    }
}
