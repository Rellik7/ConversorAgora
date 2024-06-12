﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConversorAgora.Utils
{
    public interface IFileManager
    {
        StreamReader StreamReader(Stream content);
        StreamWriter StreamWriter(string path);
    }
}
