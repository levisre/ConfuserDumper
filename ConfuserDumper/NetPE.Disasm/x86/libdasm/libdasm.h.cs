using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Disasm.x86.libdasm
{
    static partial class Consts
    {

        public const uint REGISTER_EAX = 0;
        public const uint REGISTER_ECX = 1;
        public const uint REGISTER_EDX = 2;
        public const uint REGISTER_EBX = 3;
        public const uint REGISTER_ESP = 4;
        public const uint REGISTER_EBP = 5;
        public const uint REGISTER_ESI = 6;
        public const uint REGISTER_EDI = 7;
        public const uint REGISTER_NOP = 8;

        public const uint REG_EAX = REGISTER_EAX;
        public const uint REG_AX = REG_EAX;
        public const uint REG_AL = REG_EAX;
        public const uint REG_ES = REG_EAX;
        public const uint REG_ST0 = REG_EAX;
        public const uint REG_ECX = REGISTER_ECX;
        public const uint REG_CX = REG_ECX;
        public const uint REG_CL = REG_ECX;
        public const uint REG_CS = REG_ECX;
        public const uint REG_ST1 = REG_ECX;
        public const uint REG_EDX = REGISTER_EDX;
        public const uint REG_DX = REG_EDX;
        public const uint REG_DL = REG_EDX;
        public const uint REG_SS = REG_EDX;
        public const uint REG_ST2 = REG_EDX;
        public const uint REG_EBX = REGISTER_EBX;
        public const uint REG_BX = REG_EBX;
        public const uint REG_BL = REG_EBX;
        public const uint REG_DS = REG_EBX;
        public const uint REG_ST3 = REG_EBX;
        public const uint REG_ESP = REGISTER_ESP;
        public const uint REG_SP = REG_ESP;
        public const uint REG_AH = REG_ESP;
        public const uint REG_FS = REG_ESP;
        public const uint REG_ST4 = REG_ESP;
        public const uint REG_EBP = REGISTER_EBP;
        public const uint REG_BP = REG_EBP;
        public const uint REG_CH = REG_EBP;
        public const uint REG_GS = REG_EBP;
        public const uint REG_ST5 = REG_EBP;
        public const uint REG_ESI = REGISTER_ESI;
        public const uint REG_SI = REG_ESI;
        public const uint REG_DH = REG_ESI;
        public const uint REG_ST6 = REG_ESI;
        public const uint REG_EDI = REGISTER_EDI;
        public const uint REG_DI = REG_EDI;
        public const uint REG_BH = REG_EDI;
        public const uint REG_ST7 = REG_EDI;
        public const uint REG_NOP = REGISTER_NOP;

        public const uint IOP_EAX = 1;
        public const uint IOP_ECX = (1 << (int)REG_ECX);
        public const uint IOP_EDX = (1 << (int)REG_EDX);
        public const uint IOP_EBX = (1 << (int)REG_EBX);
        public const uint IOP_ESP = (1 << (int)REG_ESP);
        public const uint IOP_EBP = (1 << (int)REG_EBP);
        public const uint IOP_ESI = (1 << (int)REG_ESI);
        public const uint IOP_EDI = (1 << (int)REG_EDI);
        public const uint IOP_ALL = IOP_EAX | IOP_ECX | IOP_EDX | IOP_ESP | IOP_EBP | IOP_ESI | IOP_EDI;

        public const uint REGISTER_TYPE_GEN = 1;
        public const uint REGISTER_TYPE_SEGMENT = 2;
        public const uint REGISTER_TYPE_DEBUG = 3;
        public const uint REGISTER_TYPE_CONTROL = 4;
        public const uint REGISTER_TYPE_TEST = 5;
        public const uint REGISTER_TYPE_XMM = 6;
        public const uint REGISTER_TYPE_MMX = 7;
        public const uint REGISTER_TYPE_FPU = 8;

        public const uint EFL_CF = (1 << 0);
        public const uint EFL_PF = (1 << 2);
        public const uint EFL_AF = (1 << 4);
        public const uint EFL_ZF = (1 << 6);
        public const uint EFL_SF = (1 << 7);
        public const uint EFL_TF = (1 << 8);
        public const uint EFL_IF = (1 << 9);
        public const uint EFL_DF = (1 << 10);
        public const uint EFL_OF = (1 << 11);
        public const uint EFL_MATH = EFL_OF | EFL_SF | EFL_ZF | EFL_AF | EFL_PF | EFL_CF;
        public const uint EFL_BITWISE = EFL_OF | EFL_CF | EFL_SF | EFL_ZF | EFL_PF;
        public const uint EFL_ALL_COMMON = EFL_CF | EFL_OF | EFL_SF | EFL_ZF | EFL_AF | EFL_PF;

        public const uint PREFIX_LOCK = 0x01000000;
        public const uint PREFIX_REPNE = 0x02000000;
        public const uint PREFIX_REP = 0x03000000;
        public const uint PREFIX_REPE = 0x03000000;

        public const uint PREFIX_ES_OVERRIDE = 0x00010000;
        public const uint PREFIX_CS_OVERRIDE = 0x00020000;
        public const uint PREFIX_SS_OVERRIDE = 0x00030000;
        public const uint PREFIX_DS_OVERRIDE = 0x00040000;
        public const uint PREFIX_FS_OVERRIDE = 0x00050000;
        public const uint PREFIX_GS_OVERRIDE = 0x00060000;

        public const uint PREFIX_OPERAND_SIZE_OVERRIDE = 0x00000100;
        public const uint PREFIX_ADDR_SIZE_OVERRIDE = 0x00001000;

        public const uint EXT_G1_1 = 0x00000001;
        public const uint EXT_G1_2 = 0x00000002;
        public const uint EXT_G1_3 = 0x00000003;
        public const uint EXT_G2_1 = 0x00000004;
        public const uint EXT_G2_2 = 0x00000005;
        public const uint EXT_G2_3 = 0x00000006;
        public const uint EXT_G2_4 = 0x00000007;
        public const uint EXT_G2_5 = 0x00000008;
        public const uint EXT_G2_6 = 0x00000009;
        public const uint EXT_G3_1 = 0x0000000a;
        public const uint EXT_G3_2 = 0x0000000b;
        public const uint EXT_G4 = 0x0000000c;
        public const uint EXT_G5 = 0x0000000d;
        public const uint EXT_G6 = 0x0000000e;
        public const uint EXT_G7 = 0x0000000f;
        public const uint EXT_G8 = 0x00000010;
        public const uint EXT_G9 = 0x00000011;
        public const uint EXT_GA = 0x00000012;
        public const uint EXT_GB = 0x00000013;
        public const uint EXT_GC = 0x00000014;
        public const uint EXT_GD = 0x00000015;
        public const uint EXT_GE = 0x00000016;
        public const uint EXT_GF = 0x00000017;
        public const uint EXT_G0 = 0x00000018;

        public const uint EXT_T2 = 0x00000020;
        public const uint EXT_CP = 0x00000030;

        public const uint TYPE_3 = 0x80000000;

        public const uint FLAGS_NONE = 0;

        public const uint AM_A = 0x00010000;
        public const uint AM_C = 0x00020000;
        public const uint AM_D = 0x00030000;
        public const uint AM_E = 0x00040000;
        public const uint AM_G = 0x00050000;
        public const uint AM_I = 0x00060000;
        public const uint AM_J = 0x00070000;
        public const uint AM_M = 0x00080000;
        public const uint AM_O = 0x00090000;
        public const uint AM_P = 0x000a0000;
        public const uint AM_Q = 0x000b0000;
        public const uint AM_R = 0x000c0000;
        public const uint AM_S = 0x000d0000;
        public const uint AM_T = 0x000e0000;
        public const uint AM_V = 0x000f0000;
        public const uint AM_W = 0x00100000;
        public const uint AM_I1 = 0x00200000;
        public const uint AM_REG = 0x00210000;
        public const uint AM_IND = 0x00220000;

        public const uint OT_a = 0x01000000;
        public const uint OT_b = 0x02000000;
        public const uint OT_c = 0x03000000;
        public const uint OT_d = 0x04000000;
        public const uint OT_q = 0x05000000;
        public const uint OT_dq = 0x06000000;
        public const uint OT_v = 0x07000000;
        public const uint OT_w = 0x08000000;
        public const uint OT_p = 0x09000000;
        public const uint OT_pi = 0x0a000000;
        public const uint OT_pd = 0x0b000000;
        public const uint OT_ps = 0x0c000000;
        public const uint OT_s = 0x0d000000;
        public const uint OT_sd = 0x0e000000;
        public const uint OT_ss = 0x0f000000;
        public const uint OT_si = 0x10000000;
        public const uint OT_t = 0x11000000;

        public const uint P_r = 0x00004000;
        public const uint P_w = 0x00002000;
        public const uint P_x = 0x00001000;

        public const uint F_s = 0x00000100;
        public const uint F_r = 0x00000200;
        public const uint F_f = 0x00000400;
    }

    static partial class Macros
    {
        public static bool IS_IOP_REG(uint x, uint y) { return ((x >> (int)y) & 1) == 1; }
        public static bool IS_IOP_EAX(uint x) { return ((x) & 1) == 1; }
        public static bool IS_IOP_ECX(uint x) { return ((x >> (int)Consts.REG_ECX) & 1) == 1; }
        public static bool IS_IOP_EDX(uint x) { return ((x >> (int)Consts.REG_ECX) & 1) == 1; }
        public static bool IS_IOP_EBX(uint x) { return ((x >> (int)Consts.REG_ECX) & 1) == 1; }
        public static bool IS_IOP_EBP(uint x) { return ((x >> (int)Consts.REG_ECX) & 1) == 1; }
        public static bool IS_IOP_ESI(uint x) { return ((x >> (int)Consts.REG_ECX) & 1) == 1; }
        public static bool IS_IOP_EDI(uint x) { return ((x >> (int)Consts.REG_ECX) & 1) == 1; }
        public static uint MASK_PREFIX_G1(uint x) { return ((x) & 0xff000000) >> 24; }
        public static uint MASK_PREFIX_G2(uint x) { return ((x) & 0x00ff0000) >> 16; }
        public static uint MASK_PREFIX_G3(uint x) { return ((x) & 0x0000ff00) >> 8; }
        public static uint MASK_PREFIX_OPERAND(uint x) { return ((x) & 0x00000f00) >> 8; }
        public static uint MASK_PREFIX_ADDR(uint x) { return ((x) & 0x0000f000) >> 12; }
        public static uint MASK_EXT(uint x) { return ((x) & 0x000000ff); }
        public static uint MASK_TYPE_FLAGS(uint x) { return ((x) & 0xff000000); }
        public static uint MASK_TYPE_VALUE(uint x) { return ((x) & 0x00ffffff); }
        public static uint MASK_AM(uint x) { return ((x) & 0x00ff0000); }
        public static uint MASK_OT(uint x) { return ((x) & 0xff000000); }
        public static uint MASK_PERMS(uint x) { return ((x) & 0x0000f000); }
        public static uint MASK_FLAGS(uint x) { return ((x) & 0x00000f00); }
        public static uint MASK_REG(uint x) { return ((x) & 0x0000000f); }
        public static uint MASK_MODRM_MOD(uint x) { return (((x) & 0xc0) >> 6); }
        public static uint MASK_MODRM_REG(uint x) { return (((x) & 0x38) >> 3); }
        public static uint MASK_MODRM_RM(uint x) { return ((x) & 0x7); }
        public static uint MASK_SIB_SCALE(uint x) { return MASK_MODRM_MOD(x); }
        public static uint MASK_SIB_INDEX(uint x) { return MASK_MODRM_REG(x); }
        public static uint MASK_SIB_BASE(uint x) { return MASK_MODRM_RM(x); }
    }

    enum Mode
    {
        MODE_32,
        MODE_16
    }
    enum Format
    {
        FORMAT_ATT,
        FORMAT_INTEL,
    }
    enum InstructionType : uint
    {
        INSTRUCTION_TYPE_ASC,
        INSTRUCTION_TYPE_DCL,
        INSTRUCTION_TYPE_MOV,
        INSTRUCTION_TYPE_MOVSR,
        INSTRUCTION_TYPE_ADD,
        INSTRUCTION_TYPE_XADD,
        INSTRUCTION_TYPE_ADC,
        INSTRUCTION_TYPE_SUB,
        INSTRUCTION_TYPE_SBB,
        INSTRUCTION_TYPE_INC,
        INSTRUCTION_TYPE_DEC,
        INSTRUCTION_TYPE_DIV,
        INSTRUCTION_TYPE_IDIV,
        INSTRUCTION_TYPE_NOT,
        INSTRUCTION_TYPE_NEG,
        INSTRUCTION_TYPE_STOS,
        INSTRUCTION_TYPE_LODS,
        INSTRUCTION_TYPE_SCAS,
        INSTRUCTION_TYPE_MOVS,
        INSTRUCTION_TYPE_MOVSX,
        INSTRUCTION_TYPE_MOVZX,
        INSTRUCTION_TYPE_CMPS,
        INSTRUCTION_TYPE_SHX,
        INSTRUCTION_TYPE_ROX,
        INSTRUCTION_TYPE_MUL,
        INSTRUCTION_TYPE_IMUL,
        INSTRUCTION_TYPE_EIMUL,
        INSTRUCTION_TYPE_XOR,
        INSTRUCTION_TYPE_LEA,
        INSTRUCTION_TYPE_XCHG,
        INSTRUCTION_TYPE_CMP,
        INSTRUCTION_TYPE_TEST,
        INSTRUCTION_TYPE_PUSH,
        INSTRUCTION_TYPE_AND,
        INSTRUCTION_TYPE_OR,
        INSTRUCTION_TYPE_POP,
        INSTRUCTION_TYPE_JMP,
        INSTRUCTION_TYPE_JMPC,
        INSTRUCTION_TYPE_JECXZ,
        INSTRUCTION_TYPE_SETC,
        INSTRUCTION_TYPE_MOVC,
        INSTRUCTION_TYPE_LOOP,
        INSTRUCTION_TYPE_CALL,
        INSTRUCTION_TYPE_RET,
        INSTRUCTION_TYPE_ENTER,
        INSTRUCTION_TYPE_INT,
        INSTRUCTION_TYPE_BT,
        INSTRUCTION_TYPE_BTS,
        INSTRUCTION_TYPE_BTR,
        INSTRUCTION_TYPE_BTC,
        INSTRUCTION_TYPE_BSF,
        INSTRUCTION_TYPE_BSR,
        INSTRUCTION_TYPE_BSWAP,
        INSTRUCTION_TYPE_SGDT,
        INSTRUCTION_TYPE_SIDT,
        INSTRUCTION_TYPE_SLDT,
        INSTRUCTION_TYPE_LFP,
        INSTRUCTION_TYPE_CLD,
        INSTRUCTION_TYPE_STD,
        INSTRUCTION_TYPE_XLAT,

        INSTRUCTION_TYPE_FCMOVC,
        INSTRUCTION_TYPE_FADD,
        INSTRUCTION_TYPE_FADDP,
        INSTRUCTION_TYPE_FIADD,
        INSTRUCTION_TYPE_FSUB,
        INSTRUCTION_TYPE_FSUBP,
        INSTRUCTION_TYPE_FISUB,
        INSTRUCTION_TYPE_FSUBR,
        INSTRUCTION_TYPE_FSUBRP,
        INSTRUCTION_TYPE_FISUBR,
        INSTRUCTION_TYPE_FMUL,
        INSTRUCTION_TYPE_FMULP,
        INSTRUCTION_TYPE_FIMUL,
        INSTRUCTION_TYPE_FDIV,
        INSTRUCTION_TYPE_FDIVP,
        INSTRUCTION_TYPE_FDIVR,
        INSTRUCTION_TYPE_FDIVRP,
        INSTRUCTION_TYPE_FIDIV,
        INSTRUCTION_TYPE_FIDIVR,
        INSTRUCTION_TYPE_FCOM,
        INSTRUCTION_TYPE_FCOMP,
        INSTRUCTION_TYPE_FCOMPP,
        INSTRUCTION_TYPE_FCOMI,
        INSTRUCTION_TYPE_FCOMIP,
        INSTRUCTION_TYPE_FUCOM,
        INSTRUCTION_TYPE_FUCOMP,
        INSTRUCTION_TYPE_FUCOMPP,
        INSTRUCTION_TYPE_FUCOMI,
        INSTRUCTION_TYPE_FUCOMIP,
        INSTRUCTION_TYPE_FST,
        INSTRUCTION_TYPE_FSTP,
        INSTRUCTION_TYPE_FIST,
        INSTRUCTION_TYPE_FISTP,
        INSTRUCTION_TYPE_FISTTP,
        INSTRUCTION_TYPE_FLD,
        INSTRUCTION_TYPE_FILD,
        INSTRUCTION_TYPE_FICOM,
        INSTRUCTION_TYPE_FICOMP,
        INSTRUCTION_TYPE_FFREE,
        INSTRUCTION_TYPE_FFREEP,
        INSTRUCTION_TYPE_FXCH,
        INSTRUCTION_TYPE_SYSENTER,
        INSTRUCTION_TYPE_FPU_CTRL,
        INSTRUCTION_TYPE_FPU,

        INSTRUCTION_TYPE_MMX,

        INSTRUCTION_TYPE_SSE,

        INSTRUCTION_TYPE_OTHER,
        INSTRUCTION_TYPE_PRIV
    }
    enum OperandType
    {
        OPERAND_TYPE_NONE, 
        OPERAND_TYPE_MEMORY, 
        OPERAND_TYPE_REGISTER, 
        OPERAND_TYPE_IMMEDIATE, 
    }

    class INST
    {
        public INST(InstructionType type, string mnemonic, uint flags1, uint flags2, uint flags3, uint modrm, uint eflags_affected, uint eflags_used, uint iop_written, uint iop_read)
        {
            this.type = type;
            this.mnemonic = mnemonic;
            this.flags1 = flags1;
            this.flags2 = flags2;
            this.flags3 = flags3;
            this.modrm = modrm != 0;
            this.eflags_affected = eflags_affected;
            this.eflags_used = eflags_used;
            this.iop_written = iop_written;
            this.iop_read = iop_read;
        }

        public InstructionType type;
        public string mnemonic;
        public uint flags1;
        public uint flags2;
        public uint flags3;
        public bool modrm;
        public uint eflags_affected;
        public uint eflags_used;
        public uint iop_written;
        public uint iop_read;
    }
    class OPERAND
    {
        public OperandType type; 
        public uint reg; 
        public uint basereg; 
        public uint indexreg; 
        public uint scale; 
        public uint dispbytes; 
        public uint dispoffset; 
        public uint immbytes; 
        public uint immoffset; 
        public uint sectionbytes; 
        public ushort section; 
        public uint displacement; 
        public uint immediate; 
        public uint flags; 
    }

    class INSTRUCTION
    {
        public uint length; 
        public InstructionType type; 
        public Mode mode; 
        public byte opcode; 
        public byte modrm; 
        public byte sib; 
        public uint modrm_offset; 
        public uint extindex; 
        public int fpuindex; 
        public uint dispbytes; 
        public uint immbytes; 
        public int sectionbytes; 
        public OPERAND op1; 
        public OPERAND op2; 
        public OPERAND op3; 
        public INST ptr; 
        public uint flags;
        public uint eflags_affected;
        public uint eflags_used;
        public uint iop_written;
        public uint iop_read; 
    }
}
