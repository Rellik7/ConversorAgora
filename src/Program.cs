using ConversorAgora.BO;
using ConversorAgora.Model;

try
{
    string sourceURL = args.Length > 0 ? args.First() : @"C:\Git\input-01.txt";
    string targetPath = args.Length > 0 ? args.Last() : @"C:\Git\output-01.txt";
    FileManager fileManager = new();

    Conversor conversor = new("MINHA CDN", sourceURL, targetPath, fileManager);

    if (!conversor.EhValido) throw new Exception(conversor.Erros.Sumario);

    conversor.ConverterLog();
    Console.WriteLine("Log convertido pressione qualquer tecla para continuar...");
    Console.ReadKey();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
