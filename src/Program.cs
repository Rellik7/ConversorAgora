using ConversorAgora.BO;
using ConversorAgora.Model;

try
{
    if (!ValidaArgumentos())
    {
        Console.WriteLine("Argumentos inválidos");
        return;
    }

    string sourceURL = args.First();
    string targetPath = args.Last();
    FileManager fileManager = new();
    HttpClient httpClient = new();

    Conversor conversor = new("MINHA CDN", sourceURL, targetPath, fileManager, httpClient);

    if (!conversor.EhValido) throw new Exception(conversor.Erros.Sumario);

    conversor.ConverterLog();
    Console.WriteLine("Log convertido pressione qualquer tecla para continuar...");
    Console.ReadKey();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

bool ValidaArgumentos()
{
    return args.Length == 2 && !string.IsNullOrEmpty(args.First()) && !string.IsNullOrEmpty(args.Last());
}