using System;
using System.IO;

namespace INTEGRACAOsql.Filemanager;
public class FileManager
{
    public static void Delete(string path)
    {

        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
                Console.WriteLine("Arquivo excluído com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao excluir o arquivo: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("O arquivo especificado não existe.");
        }
    }
}
