using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConversorAgora.Utils
{
    public abstract class Valida : IValidavel
    {
        private readonly Erros erros = new();
        public bool EhValido => erros.Count() == 0;
        public Erros Erros => erros;
        protected abstract void Validar();
    }
}
