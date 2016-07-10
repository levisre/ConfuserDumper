using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetPE.Core
{
    public class PeFileFactory
    {
        public static PeFile Read(string path, PeFileType type)
        {
            FileStream str = new FileStream(path, FileMode.Open, FileAccess.Read);
            PeFile ret = Read(str, type);
            str.Dispose();
            return ret;
        }
        public static PeFile Read(byte[] dat, PeFileType type)
        {
            MemoryStream str = new MemoryStream(dat);
            PeFile ret = Read(str, type);
            str.Dispose();
            return ret;
        }
        public static PeFile Read(Stream str, PeFileType type)
        {
            PeFile f = new PeFile(type);
            PeReader rdr = new PeReader(str);
            f.Read(rdr);
            return f;
        }

        public static void Save(string path, PeFile pf)
        {
            FileStream str = new FileStream(path, FileMode.Create);
            Save(str, pf);
            str.Close();
            str.Dispose();
        }
        public static void Save(out byte[] dat, PeFile pf)
        {
            MemoryStream str = new MemoryStream();
            Save(str, pf);
            dat = str.ToArray();
            str.Close();
            str.Dispose();
        }
        public static void Save(Stream str, PeFile pf)
        {
            pf.Write(new PeWriter(str));
        }
    }
}
