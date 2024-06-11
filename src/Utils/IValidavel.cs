using System.Collections;
using System.Text;

namespace ConversorAgora.Utils
{
    public class Erros : IEnumerable<Erro>
    {
        private readonly ICollection<Erro> erros = new List<Erro>();

        public void RegistrarErro(string mensagem) => erros.Add(new Erro(mensagem));

        public IEnumerator<Erro> GetEnumerator()
        {
            return erros.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return erros.GetEnumerator();
        }

        public string Sumario
        {
            get
            {
                StringBuilder sb = new();
                foreach (Erro item in erros)
                    sb.AppendLine(item.Mensagem);
                return sb.ToString();
            }
        }
    }

    public record Erro(string Mensagem);

    internal interface IValidavel
    {
        // bool Validar();
        bool EhValido { get; }
        Erros Erros { get; }
    }
}
