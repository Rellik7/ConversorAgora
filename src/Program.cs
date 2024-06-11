using ConversorAgora.BO;

try
{
    string sourceURL = args.Length > 0 ? args.First() : @"C:\Git\input-01.txt";
    string targetPath = args.Length > 0 ? args.Last() : @"C\Git\output-01.txt";

    Conversor conversor = new("MinhaCDN", sourceURL, targetPath);

    if (!conversor.EhValido) throw new Exception(conversor.Erros.Sumario);

    conversor.ConverterLog();
    Console.WriteLine("Log convertido pressione qualquer tecla para continuar...");
    Console.ReadKey();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
