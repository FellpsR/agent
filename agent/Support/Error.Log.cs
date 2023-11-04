using System;
using System.IO;

public class ErrorLogger
{
    private static readonly string logFilePath = "./Support/error_log.txt";

    public static void LogError(string errorMessage)
    {
        try
        {
            using (StreamWriter sw = File.AppendText(logFilePath))
            {
                sw.WriteLine($"{DateTime.Now}: {errorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao gravar no arquivo de log: {ex.Message}");
        }
    }
}
