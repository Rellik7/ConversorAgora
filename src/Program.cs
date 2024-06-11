using ConversorAgora.BO;

Conversor conversor = new("MinhaCDN", @"C:\Git\input-01.txt", @"C:\Git\output-01.txt");

conversor.ConverterLog();
Console.WriteLine("Log convertido pressione qualquer tecla para continuar...");
Console.ReadKey();