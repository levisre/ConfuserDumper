using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using NetPE.Disasm.CIL;
using NetPE.Core.Metadata;
using NetPE.Core;
using NetPE.Core.Pe;
using NetPE.Core.DataDirectories;
using System.Windows.Forms;
using NetPE.Core.Metadata.Tables;
using NetPE.Core.Metadata.Heaps;
using System.Security.Cryptography;

namespace ConfuserDumper
{
    class MethodsDumper
    {
        public Exception excpetion;
        private List<IntPtr> Dumped;

        public bool DumpMethodsOf(string target)
        {
            excpetion = null;
            Dumped = new List<IntPtr>();
            bool found = false;
            StreamWriter log = null;
            try
            {
                string output;
                string logpath;
                string dumpPath = Path.GetDirectoryName(target) + "\\DumpedAsms\\";
                if (Path.HasExtension(target))
                {
                    output = Path.ChangeExtension(target, "DumpedMethods" + Path.GetExtension(target));
                    logpath = Path.ChangeExtension(target, "log.txt");
                }
                else
                {
                    output = target + ".DumpedMethods";
                    logpath = target + ".log.txt";
                }
                log = File.CreateText(logpath);
                Assembly asm = Assembly.LoadFrom(target);

                PeFile PE = PeFileFactory.Read(target, PeFileType.Image);
                CLRDirectory dir = GetClrDirectory(PE);

                ConstructorInfo ctor = (ConstructorInfo)asm.ManifestModule.ResolveMethod(0x06000001);
                InstructionCollection insts = Disassembler.Disassemble(ctor.GetMethodBody().GetILAsByteArray());
                foreach (Instruction inst in from instr in insts where instr.OpCode == OpCodes.Call select instr)
                {
                    uint token = (inst.Operand.Value as MetadataToken).Value;
                    try
                    {
                        MethodBase method = asm.ManifestModule.ResolveMethod((int)token);
                        if (isDecryptMethod(method))
                        {
                            log.WriteLine("Decryption method found, token : 0x{0:x8}, rva : 0x{1:x8}", token, GetRvaFromToken(token, dir).Value);
                            log.WriteLine("Decryption called from \"<Module>.ctor\" at {0}", inst.GetOffsetString());
                            try
                            {
                                method.Invoke(null, null);
                            }
                            catch { }
                            found = true;
                        }                       
                    }
                    catch { }
                }
                if (found)
                {
                    log.WriteLine("Copying code");
                    CopyData(dir, Marshal.GetHINSTANCE(asm.ManifestModule), PE);
                    PeFileFactory.Save(output, PE);
                    log.WriteLine("Dump done, try deobfuscate with de4dot");
                }
                else
                log.WriteLine("Can't find decryption method, maybe not obfuscated with confuser 1.9");
            }
            catch (Exception ex)
            {
                excpetion = ex;
                log.WriteLine(ex.Message);
            }
            log.Close();
            return found;
        }
        private void CopyData(CLRDirectory datas, IntPtr hinstance, PeFile PE)
        {            
            VirtualStream vs = PE.SectionHeaders.GetVirtualStream();
            foreach (CLRData data in datas.Datas)
            {
                vs.Seek(data.Address, SeekOrigin.Begin);
                Marshal.Copy((IntPtr)(data.Address.Value + hinstance.ToInt32()), data.Data, 0, data.Data.Length);
                vs.Write(data.Data, 0, data.Data.Length);
            }
        }

        private Rva GetRvaFromToken(uint token, CLRDirectory dir)
        {
            try
            {
                MetadataRow metarow = (from row in (dir.Metadata[MetadataStreamType.Tables].Heap as TablesHeap)[TableType.MethodDef].Rows where row.Token == token select row).FirstOrDefault();
                MetadataRoot root = metarow.Container.Heap.Stream.Root;
                return root.Directory.Datas[(Rva)metarow["Rva"]].Address;
            }
            catch
            {
                return new Rva(0);
            }
        }

        private bool isDecryptMethod(MethodBase method)
        {
            if (method.GetParameters().Length != 0 || !(method is MethodInfo) || (method as MethodInfo).ReturnType != typeof(void)) return false;
            InstructionCollection insts = Disassembler.Disassemble(method.GetMethodBody().GetILAsByteArray());
            foreach (Instruction inst in from instr in insts where instr.OpCode == OpCodes.Call select instr)
            {
                uint destToken = (inst.Operand.Value as MetadataToken).Value;
                {
                    try
                    {
                        MethodBase destMethod = method.Module.ResolveMethod((int)destToken);
                        if (destMethod == typeof(Marshal).GetMethod("GetHINSTANCE", BindingFlags.Public | BindingFlags.Static))
                        {
                            return true;
                        }
                    }
                    catch { }
                }
            }
            return false;
        }
        private CLRDirectory GetClrDirectory(PeFile PE)
        {
            DataDirectoryEntry entry = (from t in PE.OptionalHeader.DataDirectories where t.Type == DataDirectoryType.CLR select t).FirstOrDefault();
            entry.Reload();
            CLRDirectory dir = entry.GetComponent() as CLRDirectory;
            dir.Load(new VirtualReader(PE.SectionHeaders.GetVirtualStream()));
            return dir;
        }
    }
}