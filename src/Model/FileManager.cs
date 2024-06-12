using ConversorAgora.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConversorAgora.Model
{
    public class FileManager : IFileManager
    {
        public StreamReader StreamReader(string path)
        {
            return new System.IO.StreamReader(path);
        }

        public StreamWriter StreamWriter(string path)
        {
            return new System.IO.StreamWriter(path);
        }
    }
}
