using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConversorAgora.Utils
{
    public interface IRequestManager
    {
        HttpClient HttpClient();
    }
}
