using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Disasm.x86.libdasm
{
    static partial class Macros
    {
        public static byte FETCH8(byte[] dat, uint idx)
        {
            return dat[idx];
        }
        public static ushort FETCH16(byte[] dat, uint idx)
        {
            return BitConverter.ToUInt16(dat, (int)idx);
        }
        public static uint FETCH32(byte[] dat, uint idx)
        {
            return BitConverter.ToUInt32(dat, (int)idx);
        }
    }
    class libdasm
    {
        private static Mode MODE_CHECK_ADDR(Mode mode, uint flags)
        {
            if (((mode == Mode.MODE_32) && (Macros.MASK_PREFIX_ADDR(flags) == 0)) ||
                ((mode == Mode.MODE_16) && (Macros.MASK_PREFIX_ADDR(flags) == 1)))
                return Mode.MODE_32;
            else
                return Mode.MODE_16;
        }
        private static Mode MODE_CHECK_OPERAND(Mode mode, uint flags)
        {
            if (((mode == Mode.MODE_32) && (Macros.MASK_PREFIX_OPERAND(flags) == 0)) ||
                  ((mode == Mode.MODE_16) && (Macros.MASK_PREFIX_OPERAND(flags) == 1)))
                return Mode.MODE_32;
            else
                return Mode.MODE_16;
        }
        private static void get_real_instruction2(byte addr, ref uint flags)
        {
            switch (addr)
            {
                case 0x00:
                    flags &= 0xffffff00;
                    flags |= Consts.EXT_G6;
                    break;
                case 0x01:
                    flags &= 0xffffff00;
                    flags |= Consts.EXT_G7;
                    break;
                case 0x71:
                    flags &= 0xffffff00;
                    flags |= Consts.EXT_GC;
                    break;
                case 0x72:
                    flags &= 0xffffff00;
                    flags |= Consts.EXT_GD;
                    break;
                case 0x73:
                    flags &= 0xffffff00;
                    flags |= Consts.EXT_GE;
                    break;
                case 0xae:
                    flags &= 0xffffff00;
                    flags |= Consts.EXT_GF;
                    break;
                case 0xba:
                    flags &= 0xffffff00;
                    flags |= Consts.EXT_G8;
                    break;
                case 0xc7:
                    flags &= 0xffffff00;
                    flags |= Consts.EXT_G9;
                    break;
            }
        }
        private static void get_real_instruction(byte[] dat, uint datIdx, ref uint index, ref uint flags)
        {
            switch (dat[datIdx])
            {
                case 0x0f:
                    index += 1;
                    flags |= Consts.EXT_T2;
                    break;
                case 0x2e:
                    index += 1;
                    flags &= 0xff00ffff;
                    flags |= Consts.PREFIX_CS_OVERRIDE;
                    get_real_instruction(dat, datIdx + 1, ref index, ref flags);
                    break;
                case 0x36:
                    index += 1;
                    flags &= 0xff00ffff;
                    flags |= Consts.PREFIX_SS_OVERRIDE;
                    get_real_instruction(dat, datIdx + 1, ref index, ref flags);
                    break;
                case 0x3e:
                    index += 1;
                    flags &= 0xff00ffff;
                    flags |= Consts.PREFIX_DS_OVERRIDE;
                    get_real_instruction(dat, datIdx + 1, ref index, ref flags);
                    break;
                case 0x26:
                    index += 1;
                    flags &= 0xff00ffff;
                    flags |= Consts.PREFIX_ES_OVERRIDE;
                    get_real_instruction(dat, datIdx + 1, ref index, ref flags);
                    break;
                case 0x64:
                    index += 1;
                    flags &= 0xff00ffff;
                    flags |= Consts.PREFIX_FS_OVERRIDE;
                    get_real_instruction(dat, datIdx + 1, ref index, ref flags);
                    break;
                case 0x65:
                    index += 1;
                    flags &= 0xff00ffff;
                    flags |= Consts.PREFIX_GS_OVERRIDE;
                    get_real_instruction(dat, datIdx + 1, ref index, ref flags);
                    break;
                case 0x66:
                    index += 1;
                    flags |= Consts.PREFIX_OPERAND_SIZE_OVERRIDE;
                    get_real_instruction(dat, datIdx + 1, ref index, ref flags);
                    break;
                case 0x67:
                    index += 1;
                    flags |= Consts.PREFIX_ADDR_SIZE_OVERRIDE;
                    get_real_instruction(dat, datIdx + 1, ref index, ref flags);
                    break;
                case 0x80:
                    flags |= Consts.EXT_G1_1;
                    break;
                case 0x81:
                    flags |= Consts.EXT_G1_2;
                    break;
                case 0x82:
                    flags |= Consts.EXT_G1_1;
                    break;
                case 0x83:
                    flags |= Consts.EXT_G1_3;
                    break;
                case 0xc0:
                    flags |= Consts.EXT_G2_1;
                    break;
                case 0xc1:
                    flags |= Consts.EXT_G2_2;
                    break;
                case 0xd0:
                    flags |= Consts.EXT_G2_3;
                    break;
                case 0xd1:
                    flags |= Consts.EXT_G2_4;
                    break;
                case 0xd2:
                    flags |= Consts.EXT_G2_5;
                    break;
                case 0xd3:
                    flags |= Consts.EXT_G2_6;
                    break;
                case 0xd8:
                case 0xd9:
                case 0xda:
                case 0xdb:
                case 0xdc:
                case 0xdd:
                case 0xde:
                case 0xdf:
                    index += 1;
                    flags |= Consts.EXT_CP;
                    break;
                case 0xf0:
                    index += 1;
                    flags &= 0x00ffffff;
                    flags |= Consts.PREFIX_LOCK;
                    get_real_instruction(dat, datIdx + 1, ref index, ref flags);
                    break;
                case 0xf2:
                    index += 1;
                    flags &= 0x00ffffff;
                    flags |= Consts.PREFIX_REPNE;
                    get_real_instruction(dat, datIdx + 1, ref index, ref flags);
                    break;
                case 0xf3:
                    index += 1;
                    flags &= 0x00ffffff;
                    flags |= Consts.PREFIX_REP;
                    get_real_instruction(dat, datIdx + 1, ref index, ref flags);
                    break;
                case 0xf6:
                    flags |= Consts.EXT_G3_1;
                    break;
                case 0xf7:
                    flags |= Consts.EXT_G3_2;
                    break;
                case 0xfe:
                    flags |= Consts.EXT_G4;
                    break;
                case 0xff:
                    flags |= Consts.EXT_G5;
                    break;
                default:
                    break;
            }
        }

        private static bool get_operand(INST inst, uint oflags, INSTRUCTION instruction, out OPERAND op, byte[] data, uint idx, Mode mode, uint iflags)
        {
            uint index = 0;
            bool sib = false;
            uint scale = 0;
            uint reg = Consts.REG_NOP;
            uint basereg = Consts.REG_NOP;
            uint indexreg = Consts.REG_NOP;
            uint dispbytes = 0;
            Mode pmode;

            op = new OPERAND();

            if (oflags == Consts.FLAGS_NONE)
            {
                op.type = OperandType.OPERAND_TYPE_NONE;
                return true;
            }

            op.flags = oflags;


            op.reg = Consts.REG_NOP;
            op.basereg = Consts.REG_NOP;
            op.indexreg = Consts.REG_NOP;


            op.dispoffset = 0;
            op.immoffset = 0;


            if (inst.modrm)
            {
                pmode = MODE_CHECK_ADDR(mode, iflags);


                if (instruction.length == 0)
                {
                    instruction.modrm = (byte)idx;
                    instruction.length += 1;
                }

                reg = Macros.MASK_MODRM_REG(data[idx]);



                if (Macros.MASK_MODRM_MOD(data[idx]) == 0)
                {
                    if ((pmode == Mode.MODE_32) && (Macros.MASK_MODRM_RM(data[idx]) == Consts.REG_EBP))
                        dispbytes = 4;
                    if ((pmode == Mode.MODE_16) && (Macros.MASK_MODRM_RM(data[idx]) == Consts.REG_ESI))
                        dispbytes = 2;
                }
                else if (Macros.MASK_MODRM_MOD(data[idx]) == 1)
                {
                    dispbytes = 1;

                }
                else if (Macros.MASK_MODRM_MOD(data[idx]) == 2)
                {
                    dispbytes = (pmode == Mode.MODE_32) ? 4U : 2U;
                }



                if (pmode == Mode.MODE_32)
                {
                    if ((Macros.MASK_MODRM_RM(data[idx]) == Consts.REG_ESP) && (Macros.MASK_MODRM_MOD(data[idx]) != 3))
                    {
                        sib = true;
                        instruction.sib = data[idx + 1];


                        if (instruction.length == 1)
                        {
                            instruction.sib = data[idx + 1];
                            instruction.length += 1;
                        }
                        basereg = Macros.MASK_SIB_BASE(data[idx + 1]);
                        indexreg = Macros.MASK_SIB_INDEX(data[idx + 1]);
                        scale = Macros.MASK_SIB_SCALE(data[idx + 1]) * 2;

                        if (scale == 6)
                            scale += 2;


                        if ((basereg == Consts.REG_EBP) && Macros.MASK_MODRM_MOD(data[idx]) == 0)
                        {
                            basereg = Consts.REG_NOP;
                            dispbytes = 4;
                        }
                        if (indexreg == Consts.REG_ESP)
                            indexreg = Consts.REG_NOP;
                    }
                    else
                    {
                        if (Macros.MASK_MODRM_MOD(data[idx]) == 0 && (Macros.MASK_MODRM_RM(data[idx]) == Consts.REG_EBP))
                            basereg = Consts.REG_NOP;
                        else
                            basereg = Macros.MASK_MODRM_RM(data[idx]);
                    }

                }
                else
                {
                    switch (Macros.MASK_MODRM_RM(data[idx]))
                    {
                        case 0:
                            basereg = Consts.REG_EBX;
                            indexreg = Consts.REG_ESI;
                            break;
                        case 1:
                            basereg = Consts.REG_EBX;
                            indexreg = Consts.REG_EDI;
                            break;
                        case 2:
                            basereg = Consts.REG_EBP;
                            indexreg = Consts.REG_ESI;
                            break;
                        case 3:
                            basereg = Consts.REG_EBP;
                            indexreg = Consts.REG_EDI;
                            break;
                        case 4:
                            basereg = Consts.REG_ESI;
                            indexreg = Consts.REG_NOP;
                            break;
                        case 5:
                            basereg = Consts.REG_EDI;
                            indexreg = Consts.REG_NOP;
                            break;
                        case 6:
                            if (Macros.MASK_MODRM_MOD(data[idx]) == 0)
                                basereg = Consts.REG_NOP;
                            else
                                basereg = Consts.REG_EBP;
                            indexreg = Consts.REG_NOP;
                            break;
                        case 7:
                            basereg = Consts.REG_EBX;
                            indexreg = Consts.REG_NOP;
                            break;
                    }
                    if (Macros.MASK_MODRM_MOD(data[idx]) == 3)
                    {
                        basereg = Macros.MASK_MODRM_RM(data[idx]);
                        indexreg = Consts.REG_NOP;
                    }
                }
            }


            switch (Macros.MASK_AM(oflags))
            {


                case Consts.AM_REG:
                    op.type = OperandType.OPERAND_TYPE_REGISTER;
                    op.reg = Macros.MASK_REG(oflags);
                    break;


                case Consts.AM_IND:
                    op.type = OperandType.OPERAND_TYPE_MEMORY;
                    op.basereg = Macros.MASK_REG(oflags);
                    break;


                case Consts.AM_M:
                    if (Macros.MASK_MODRM_MOD(data[idx]) == 3)
                        return false;
                    goto skip_rest;
                case Consts.AM_R:
                    if (Macros.MASK_MODRM_MOD(data[idx]) != 3)
                        return false;
                    goto skip_rest;
                case Consts.AM_Q:
                case Consts.AM_W:
                case Consts.AM_E:
                skip_rest:
                    op.type = OperandType.OPERAND_TYPE_MEMORY;
                op.dispbytes = dispbytes;
                instruction.dispbytes = dispbytes;
                op.basereg = basereg;
                op.indexreg = indexreg;
                op.scale = scale;

                index = (sib) ? 1U : 0U;
                if (dispbytes != 0)
                    op.dispoffset = index + 1 + idx;
                switch (dispbytes)
                {
                    case 0:
                        break;
                    case 1:
                        op.displacement = Macros.FETCH8(data, idx + 1 + index);

                        if (op.displacement >= 0x80)
                            op.displacement |= 0xffffff00;
                        break;
                    case 2:
                        op.displacement = Macros.FETCH16(data, idx + 1 + index);
                        break;
                    case 4:
                        op.displacement = Macros.FETCH32(data, idx + 1 + index);
                        break;
                }


                if ((basereg != Consts.REG_NOP) && (Macros.MASK_MODRM_MOD(data[idx]) == 3))
                {
                    op.type = OperandType.OPERAND_TYPE_REGISTER;
                    op.reg = basereg;
                }
                break;


                case Consts.AM_I1:
                op.type = OperandType.OPERAND_TYPE_IMMEDIATE;
                op.immbytes = 1;
                op.immediate = 1;
                break;

                case Consts.AM_J:
                op.type = OperandType.OPERAND_TYPE_IMMEDIATE;

                oflags |= Consts.F_s;
                goto am_i;
                case Consts.AM_I:
            am_i:
                op.type = OperandType.OPERAND_TYPE_IMMEDIATE;
            index = (inst.modrm) ? 1U : 0U;
            index += (sib) ? 1U : 0U;
            index += instruction.immbytes;
            index += instruction.dispbytes;
            op.immoffset = index + idx;


            mode = MODE_CHECK_OPERAND(mode, iflags);

            switch (Macros.MASK_OT(oflags))
            {
                case Consts.OT_b:
                    op.immbytes = 1;
                    op.immediate = Macros.FETCH8(data, idx + index);
                    if ((op.immediate >= 0x80) && (Macros.MASK_FLAGS(oflags) == Consts.F_s))
                        op.immediate |= 0xffffff00;
                    break;
                case Consts.OT_v:
                    op.immbytes = (mode == Mode.MODE_32) ? 4U : 2U;
                    op.immediate = (mode == Mode.MODE_32) ? Macros.FETCH32(data, idx + index) : Macros.FETCH16(data, idx + index);
                    break;
                case Consts.OT_w:
                    op.immbytes = 2;
                    op.immediate = Macros.FETCH16(data, idx + index);
                    break;
            }
            instruction.immbytes += op.immbytes;
            break;


                case Consts.AM_A:
            op.type = OperandType.OPERAND_TYPE_IMMEDIATE;

            mode = MODE_CHECK_OPERAND(mode, iflags);

            op.dispbytes = (mode == Mode.MODE_32) ? 6U : 4U;
            op.displacement = (mode == Mode.MODE_32) ? Macros.FETCH32(data, idx) : Macros.FETCH16(data, idx);
            op.section = Macros.FETCH16(data, idx + op.dispbytes - 2);

            instruction.dispbytes = op.dispbytes;
            instruction.sectionbytes = 2;
            break;


                case Consts.AM_O:
            op.type = OperandType.OPERAND_TYPE_MEMORY;
            switch (Macros.MASK_OT(oflags))
            {
                case Consts.OT_b:
                    op.dispbytes = 1;
                    op.displacement = Macros.FETCH8(data, idx);
                    break;
                case Consts.OT_v:
                    op.dispbytes = (mode == Mode.MODE_32) ? 4U : 2U;
                    op.displacement = (mode == Mode.MODE_32) ? Macros.FETCH32(data, idx) : Macros.FETCH16(data, idx);
                    break;
            }
            instruction.dispbytes = op.dispbytes;
            op.dispoffset = idx;
            break;


                case Consts.AM_G:
            op.type = OperandType.OPERAND_TYPE_REGISTER;
            op.reg = reg;
            break;


                case Consts.AM_C:

                case Consts.AM_D:

                case Consts.AM_S:

                case Consts.AM_T:

                case Consts.AM_P:

                case Consts.AM_V:
            op.type = OperandType.OPERAND_TYPE_REGISTER;
            op.reg = Macros.MASK_MODRM_REG(instruction.modrm);
            break;
            }
            return true;
        }
        public static uint get_instruction(out INSTRUCTION inst, byte[] dat, uint idx, Mode mode)
        {
            INST ptr = null;
            uint index = 0;
            uint flags = 0;

            inst = new INSTRUCTION();


            get_real_instruction(dat, idx, ref index, ref flags);




            if (Macros.MASK_EXT(flags) == 0)
            {
                inst.opcode = dat[idx + index];
                ptr = opcode_tables.inst_table1[inst.opcode];


            }
            else if (Macros.MASK_EXT(flags) == Consts.EXT_CP)
            {
                if (dat[idx + index] < 0xc0)
                {

                    index--;
                    inst.fpuindex = dat[idx + index] - 0xd8;
                    inst.opcode = dat[idx + index + 1];
                    ptr = opcode_tables.inst_table4[inst.fpuindex][Macros.MASK_MODRM_REG(inst.opcode)];
                }
                else
                {
                    inst.fpuindex = dat[idx + index - 1] - 0xd8;
                    inst.opcode = dat[idx + index];
                    ptr = opcode_tables.inst_table4[inst.fpuindex][inst.opcode - 0xb8];
                }

            }
            else if (Macros.MASK_EXT(flags) == Consts.EXT_T2)
            {
                inst.opcode = dat[idx + index];


                get_real_instruction2(dat[idx + index], ref flags);


                ptr = opcode_tables.inst_table2[inst.opcode];


                if (Macros.MASK_TYPE_FLAGS((uint)ptr.type) == Consts.TYPE_3)
                {

                    if (Macros.MASK_PREFIX_OPERAND(flags) == 1)
                    {
                        ptr = opcode_tables.inst_table3_66[inst.opcode];


                    }
                    else if (Macros.MASK_PREFIX_G1(flags) == 2)
                    {
                        ptr = opcode_tables.inst_table3_f2[inst.opcode];


                    }
                    else if (Macros.MASK_PREFIX_G1(flags) == 3)
                    {
                        ptr = opcode_tables.inst_table3_f3[inst.opcode];

                    }
                }
            }

            if (Macros.MASK_EXT(flags) != 0 && (Macros.MASK_EXT(flags) < Consts.EXT_T2))
            {
                inst.opcode = dat[idx + index];
                inst.extindex = Macros.MASK_MODRM_REG(dat[idx + index + 1]);

                switch (Macros.MASK_EXT(flags))
                {
                    case Consts.EXT_GC:

                        if (Macros.MASK_PREFIX_OPERAND(flags) == 1)
                            ptr = opcode_tables.inst_table_ext12_66[inst.extindex];
                        else
                            ptr = opcode_tables.inst_table_ext12[inst.extindex];
                        break;
                    case Consts.EXT_GD:

                        if (Macros.MASK_PREFIX_OPERAND(flags) == 1)
                            ptr = opcode_tables.inst_table_ext13_66[inst.extindex];
                        else
                            ptr = opcode_tables.inst_table_ext13[inst.extindex];
                        break;
                    case Consts.EXT_GE:

                        if (Macros.MASK_PREFIX_OPERAND(flags) == 1)
                            ptr = opcode_tables.inst_table_ext14_66[inst.extindex];
                        else
                            ptr = opcode_tables.inst_table_ext14[inst.extindex];
                        break;


                    case Consts.EXT_G7:
                        if (Macros.MASK_MODRM_MOD(dat[idx + index + 1]) == 3)
                        {
                            if (inst.extindex != 1)
                                return 0;
                            if (Macros.MASK_MODRM_RM(dat[idx + index + 1]) == 0)
                            {
                                ptr = opcode_tables.inst_monitor;


                                index++;
                            }
                            else if (Macros.MASK_MODRM_RM(dat[idx + index + 1]) == 1)
                            {
                                ptr = opcode_tables.inst_mwait;
                                index++;
                            }
                            else
                                return 0;

                        }
                        else
                        {
                            ptr = opcode_tables.inst_table_ext7[inst.extindex];
                        }
                        break;
                    default:
                        ptr = opcode_tables.inst_table_ext[(Macros.MASK_EXT(flags)) - 1][inst.extindex];
                        break;
                }
            }

            index++;


            if (ptr == null)
                return 0;
            if (ptr.mnemonic == null)
                return 0;


            if (ptr.modrm)
                inst.modrm_offset = index;


            inst.type = (InstructionType)Macros.MASK_TYPE_VALUE((uint)ptr.type);


            inst.eflags_affected = ptr.eflags_affected;
            inst.eflags_used = ptr.eflags_used;


            inst.ptr = ptr;



            if (!get_operand(ptr, ptr.flags1, inst, out inst.op1, dat, index + idx, mode, flags))
                return 0;
            if (!get_operand(ptr, ptr.flags2, inst, out inst.op2, dat, index + idx, mode, flags))
                return 0;
            if (!get_operand(ptr, ptr.flags3, inst, out inst.op3, dat, index + idx, mode, flags))
                return 0;


            inst.iop_read = ptr.iop_read;
            inst.iop_written = ptr.iop_written;


            inst.length += index + inst.immbytes + inst.dispbytes;


            inst.mode = mode;


            inst.flags = flags;

            return inst.length;
        }

        public static string get_operand_string(INSTRUCTION inst, OPERAND op, Format format, uint offset)
        {
            StringBuilder ret = new StringBuilder();

            Mode mode;
            uint regtype = 0;
            uint tmp = 0;

            if (op.type == OperandType.OPERAND_TYPE_REGISTER)
            {

                mode = MODE_CHECK_OPERAND(inst.mode, inst.flags);

                if (format == Format.FORMAT_ATT)
                    ret.Append("%%");


                switch (Macros.MASK_AM(op.flags))
                {
                    case Consts.AM_REG:
                        if (Macros.MASK_FLAGS(op.flags) == Consts.F_r)
                            regtype = Consts.REG_SEGMENT;
                        else if (Macros.MASK_FLAGS(op.flags) == Consts.F_f)
                            regtype = Consts.REG_FPU;
                        else
                            regtype = Consts.REG_GEN_DWORD;
                        break;
                    case Consts.AM_E:
                    case Consts.AM_G:
                    case Consts.AM_R:
                        regtype = Consts.REG_GEN_DWORD;
                        break;

                    case Consts.AM_C:
                        regtype = Consts.REG_CONTROL;
                        break;

                    case Consts.AM_D:
                        regtype = Consts.REG_DEBUG;
                        break;

                    case Consts.AM_S:
                        regtype = Consts.REG_SEGMENT;
                        break;

                    case Consts.AM_T:
                        regtype = Consts.REG_TEST;
                        break;

                    case Consts.AM_P:
                    case Consts.AM_Q:
                        regtype = Consts.REG_MMX;
                        break;

                    case Consts.AM_V:
                    case Consts.AM_W:
                        regtype = Consts.REG_XMM;
                        break;
                }
                if (regtype == Consts.REG_GEN_DWORD)
                {
                    switch (Macros.MASK_OT(op.flags))
                    {
                        case Consts.OT_b:
                            ret.Append(Consts.reg_table[Consts.REG_GEN_BYTE, op.reg]);
                            break;
                        case Consts.OT_v:
                            ret.Append((mode == Mode.MODE_32) ? Consts.reg_table[Consts.REG_GEN_DWORD, op.reg] : Consts.reg_table[Consts.REG_GEN_WORD, op.reg]);
                            break;
                        case Consts.OT_w:
                            ret.Append(Consts.reg_table[Consts.REG_GEN_WORD, op.reg]);
                            break;
                        case Consts.OT_d:
                            ret.Append(Consts.reg_table[Consts.REG_GEN_DWORD, op.reg]);
                            break;
                    }
                }
                else
                    ret.Append(Consts.reg_table[regtype, op.reg]);

            }
            else if (op.type == OperandType.OPERAND_TYPE_MEMORY)
            {

                mode = MODE_CHECK_ADDR(inst.mode, inst.flags);


                if (Macros.MASK_PREFIX_G2(inst.flags) != 0)
                    ret.AppendFormat("{0}{1}:", (format == Format.FORMAT_ATT) ? "%" : "", Consts.reg_table[Consts.REG_SEGMENT, (Macros.MASK_PREFIX_G2(inst.flags)) - 1]);

                if (format == Format.FORMAT_ATT)
                {


                    if (Macros.MASK_PERMS(op.flags) == Consts.P_x)
                        ret.Append("*");


                    if (op.dispbytes != 0)
                        ret.AppendFormat("0x{0:X}", op.displacement);


                    if ((op.basereg == Consts.REG_NOP) && (op.indexreg == Consts.REG_NOP))
                        return ret.ToString();
                }

                ret.Append((format == Format.FORMAT_ATT) ? "(" : "[");


                if (op.basereg != Consts.REG_NOP)
                {
                    ret.AppendFormat("{0}{1}", (format == Format.FORMAT_ATT) ? "%" : "", (mode == Mode.MODE_32) ? Consts.reg_table[Consts.REG_GEN_DWORD, op.basereg] : Consts.reg_table[Consts.REG_GEN_WORD, op.basereg]);
                }

                if (op.indexreg != Consts.REG_NOP)
                {
                    if (op.basereg != Consts.REG_NOP)
                        ret.AppendFormat("{0}{1}", (format == Format.FORMAT_ATT) ? ",%" : "+", (mode == Mode.MODE_32) ? Consts.reg_table[Consts.REG_GEN_DWORD, op.indexreg] : Consts.reg_table[Consts.REG_GEN_WORD, op.indexreg]);
                    else
                        ret.AppendFormat("{0}{1}", (format == Format.FORMAT_ATT) ? "%" : "", (mode == Mode.MODE_32) ? Consts.reg_table[Consts.REG_GEN_DWORD, op.indexreg] : Consts.reg_table[Consts.REG_GEN_WORD, op.indexreg]);
                    switch (op.scale)
                    {
                        case 2:
                            ret.Append((format == Format.FORMAT_ATT) ? ",2" : "*2");
                            break;
                        case 4:
                            ret.Append((format == Format.FORMAT_ATT) ? ",4" : "*4");
                            break;
                        case 8:
                            ret.Append((format == Format.FORMAT_ATT) ? ",8" : "*8");
                            break;
                    }
                }

                if (inst.dispbytes != 0 && (format != Format.FORMAT_ATT))
                {
                    if ((op.basereg != Consts.REG_NOP) || (op.indexreg != Consts.REG_NOP))
                    {

                        if ((op.displacement & (1 << (int)(op.dispbytes * 8 - 1))) != 0)
                        {
                            tmp = op.displacement;
                            switch (op.dispbytes)
                            {
                                case 1:
                                    tmp = ~tmp & 0xff;
                                    break;
                                case 2:
                                    tmp = ~tmp & 0xffff;
                                    break;
                                case 4:
                                    tmp = ~tmp;
                                    break;
                            }
                            ret.AppendFormat("-0x{0:X}", tmp + 1);

                        }
                        else
                            ret.AppendFormat("+0x{0:X}", op.displacement);

                    }
                    else
                    {
                        ret.AppendFormat("0x{0:X}", op.displacement);
                    }
                }

                ret.Append((format == Format.FORMAT_ATT) ? ")" : "]");

            }
            else if (op.type == OperandType.OPERAND_TYPE_IMMEDIATE)
            {

                switch (Macros.MASK_AM(op.flags))
                {
                    case Consts.AM_J:
                        ret.AppendFormat("0x{0:X}", op.immediate + inst.length + offset);
                        break;
                    case Consts.AM_I1:
                    case Consts.AM_I:
                        if (format == Format.FORMAT_ATT)
                            ret.Append("$");
                        ret.AppendFormat("0x{0:X}", op.immediate);
                        break;

                    case Consts.AM_A:
                        ret.AppendFormat("{0}0x{1:X}:{2}0x{2:X}", (format == Format.FORMAT_ATT) ? "$" : "", op.section, (format == Format.FORMAT_ATT) ? "$" : "", op.displacement);
                        break;
                }

            }
            else
                return null;

            return ret.ToString();

        }
        public static string get_mnemonic_string(INSTRUCTION inst, Format format)
        {
            StringBuilder ret = new StringBuilder();

            Mode mode;


            if (Macros.MASK_PREFIX_G2(inst.flags) != 0 && (inst.op1.type != OperandType.OPERAND_TYPE_MEMORY) && (inst.op2.type != OperandType.OPERAND_TYPE_MEMORY))
            {

                if (inst.type == InstructionType.INSTRUCTION_TYPE_JMPC)
                    ret.Append(Consts.reg_table[Consts.REG_BRANCH, (Macros.MASK_PREFIX_G2(inst.flags)) - 1] + " ");

                else
                    ret.Append(Consts.reg_table[Consts.REG_SEGMENT, (Macros.MASK_PREFIX_G2(inst.flags)) - 1] + " ");
            }


            if (Macros.MASK_PREFIX_G1(inst.flags) != 0 && (Macros.MASK_EXT(inst.flags) != Consts.EXT_T2))
                ret.Append(Consts.rep_table[(Macros.MASK_PREFIX_G1(inst.flags)) - 1]);




            if (((inst.type == InstructionType.INSTRUCTION_TYPE_JMPC) && (inst.opcode == 0xe3)) && (Macros.MASK_PREFIX_ADDR(inst.flags) == 1))
                ret.Append("jcxz");
            else
                ret.Append(inst.ptr.mnemonic);



            if (inst.type == InstructionType.INSTRUCTION_TYPE_PUSH)
            {
                if (inst.op1.type == OperandType.OPERAND_TYPE_IMMEDIATE)
                {
                    switch (inst.op1.immbytes)
                    {
                        case 1:
                            ret.Append((format == Format.FORMAT_ATT) ? "b" : " byte");
                            break;
                        case 2:
                            ret.Append((format == Format.FORMAT_ATT) ? "w" : " word");
                            break;
                        case 4:
                            ret.Append((format == Format.FORMAT_ATT) ? "l" : " dword");
                            break;
                    }

                }
                else if (inst.op1.type == OperandType.OPERAND_TYPE_MEMORY)
                {
                    mode = MODE_CHECK_OPERAND(inst.mode, inst.flags);

                    if (mode == Mode.MODE_16)
                    {
                        ret.Append((format == Format.FORMAT_ATT) ? "w" : " word");
                    }
                    else if (mode == Mode.MODE_32)
                    {
                        ret.Append((format == Format.FORMAT_ATT) ? "l" : " dword");
                    }

                }
                return ret.ToString();

            }
            if (inst.type == InstructionType.INSTRUCTION_TYPE_POP)
            {
                if (inst.op1.type == OperandType.OPERAND_TYPE_MEMORY)
                {
                    mode = MODE_CHECK_OPERAND(inst.mode, inst.flags);

                    if (mode == Mode.MODE_16)
                    {
                        ret.Append((format == Format.FORMAT_ATT) ? "w" : " word");
                    }
                    else if (mode == Mode.MODE_32)
                    {
                        ret.Append((format == Format.FORMAT_ATT) ? "l" : " dword");
                    }
                }
                return ret.ToString();
            }


            if (inst.ptr.modrm && (Macros.MASK_MODRM_MOD(inst.modrm) != 3) && (Macros.MASK_AM(inst.op2.flags) == Consts.AM_I))
            {

                switch (Macros.MASK_OT(inst.op1.flags))
                {
                    case Consts.OT_b:
                        ret.Append((format == Format.FORMAT_ATT) ? "b" : " byte");
                        break;
                    case Consts.OT_w:
                        ret.Append((format == Format.FORMAT_ATT) ? "w" : " word");
                        break;
                    case Consts.OT_d:
                        ret.Append((format == Format.FORMAT_ATT) ? "l" : " dword");
                        break;
                    case Consts.OT_v:
                        if (((inst.mode == Mode.MODE_32) && (Macros.MASK_PREFIX_OPERAND(inst.flags) == 0)) || ((inst.mode == Mode.MODE_16) && (Macros.MASK_PREFIX_OPERAND(inst.flags) == 1)))
                            ret.Append((format == Format.FORMAT_ATT) ? "l" : " dword");
                        else
                            ret.Append((format == Format.FORMAT_ATT) ? "w" : " word");
                        break;
                }
            }



            return ret.ToString();
        }
        public static string get_operands_string(INSTRUCTION inst, Format format, uint offset)
        {
            StringBuilder ret = new StringBuilder();

            if (format == Format.FORMAT_ATT)
            {
                if (inst.op3.type != OperandType.OPERAND_TYPE_NONE)
                {
                    ret.Append(get_operand_string(inst, inst.op3, format, offset) + ",");
                }
                if (inst.op2.type != OperandType.OPERAND_TYPE_NONE)
                {
                    ret.Append(get_operand_string(inst, inst.op2, format, offset) + ",");
                }
                if (inst.op1.type != OperandType.OPERAND_TYPE_NONE)
                    ret.Append(get_operand_string(inst, inst.op1, format, offset));
            }
            else if (format == Format.FORMAT_INTEL)
            {
                if (inst.op1.type != OperandType.OPERAND_TYPE_NONE)
                    ret.Append(get_operand_string(inst, inst.op1, format, offset));
                if (inst.op2.type != OperandType.OPERAND_TYPE_NONE)
                {
                    ret.Append("," + get_operand_string(inst, inst.op2, format, offset));
                }
                if (inst.op3.type != OperandType.OPERAND_TYPE_NONE)
                {
                    ret.Append("," + get_operand_string(inst, inst.op3, format, offset));
                }
            }
            else
                return null;

            return ret.ToString();
        }
        public static string get_instruction_string(INSTRUCTION inst, Format format, uint offset)
        {
            StringBuilder ret = new StringBuilder();

            ret.Append(get_mnemonic_string(inst, format));

            ret.Append(" ");

            ret.Append(get_operands_string(inst, format, offset));

            return ret.ToString();
        }

        public static uint get_register_type(OPERAND op)
        {
            if (op.type != OperandType.OPERAND_TYPE_REGISTER)
                return 0;
            switch (Macros.MASK_AM(op.flags))
            {
                case Consts.AM_REG:
                    if (Macros.MASK_FLAGS(op.flags) == Consts.F_r)
                        return Consts.REGISTER_TYPE_SEGMENT;
                    else if (Macros.MASK_FLAGS(op.flags) == Consts.F_f)
                        return Consts.REGISTER_TYPE_FPU;
                    else
                        return Consts.REGISTER_TYPE_GEN;
                case Consts.AM_E:
                case Consts.AM_G:
                case Consts.AM_R:
                    return Consts.REGISTER_TYPE_GEN;
                case Consts.AM_C:
                    return Consts.REGISTER_TYPE_CONTROL;
                case Consts.AM_D:
                    return Consts.REGISTER_TYPE_DEBUG;
                case Consts.AM_S:
                    return Consts.REGISTER_TYPE_SEGMENT;
                case Consts.AM_T:
                    return Consts.REGISTER_TYPE_TEST;
                case Consts.AM_P:
                case Consts.AM_Q:
                    return Consts.REGISTER_TYPE_MMX;
                case Consts.AM_V:
                case Consts.AM_W:
                    return Consts.REGISTER_TYPE_XMM;
                default:
                    break;
            }
            return 0;
        }
        public static OperandType get_operand_type(OPERAND op)
        {
            return op.type;
        }
        public static uint get_operand_register(OPERAND op)
        {
            return op.reg;
        }
        public static uint get_operand_basereg(OPERAND op)
        {
            return op.basereg;
        }
        public static uint get_operand_indexreg(OPERAND op)
        {
            return op.indexreg;
        }
        public static uint get_operand_scale(OPERAND op)
        {
            return op.scale;
        }
        public static bool get_operand_immediate(OPERAND op, ref uint imm)
        {
            if (op.immbytes != 0)
            {
                imm = op.immediate;
                return true;
            }
            else
                return false;
        }
        public static bool get_operand_displacement(OPERAND op, ref uint disp)
        {
            if (op.dispbytes != 0)
            {
                disp = op.displacement;
                return true;
            }
            else
                return false;
        }

        public static bool get_source_operand(INSTRUCTION inst, ref OPERAND op)
        {
            if (inst.op2.type != OperandType.OPERAND_TYPE_NONE)
            {
                op = inst.op2;
                return true;
            }
            else
                return false;
        }
        public static bool get_destination_operand(INSTRUCTION inst, ref OPERAND op)
        {
            if (inst.op1.type != OperandType.OPERAND_TYPE_NONE)
            {
                op = inst.op1;
                return true;
            }
            else
                return false;
        }
    }
}
