using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lerXML.Application.Services
{
    public class RarService
    {
        private string _7zipPath = @"C:\Program Files\7-Zip\7z.exe";
        public async Task CompactarDiretorios(string[] diretorios, string arquivoSaida)
        {
            try
            {
                if (diretorios == null || diretorios.Length == 0)
                {
                    Console.WriteLine("Nenhum diretório para compactar.");
                    return;
                }

                if (!File.Exists(_7zipPath))
                {
                    throw new FileNotFoundException("O 7-Zip não foi encontrado. Verifique se está instalado.");
                }

                // Verifica se os diretórios realmente existem antes de processar
                List<string> diretoriosValidos = diretorios.Where(Directory.Exists).ToList();
                if (diretoriosValidos.Count == 0)
                {
                    Console.WriteLine("Nenhum dos diretórios especificados existe.");
                    return;
                }

                // Pegamos o diretório pai mais alto possível para manter a estrutura relativa
                string raizComum = Path.GetPathRoot(diretoriosValidos[0]); // Obtém a raiz (ex: "Z:\")
                Console.WriteLine($"📂 Diretório raiz comum: {raizComum}");

                // Criar lista de diretórios temporária para o 7-Zip
                string fileListPath = Path.Combine(Path.GetTempPath(), "dirlist.txt");
                using (StreamWriter sw = new StreamWriter(fileListPath))
                {
                    foreach (var diretorio in diretoriosValidos)
                    {
                        string relativePath = Path.GetRelativePath(raizComum, diretorio);
                        sw.WriteLine($"\"{relativePath}\""); // Adicionamos o caminho relativo
                        Console.WriteLine($"✔ Adicionado: {relativePath}");
                    }
                }

                // Argumentos do 7-Zip:
                string argumentos = $"a -t7z \"{arquivoSaida}\" @{fileListPath} -mx=9 -spf";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = _7zipPath,
                    Arguments = argumentos,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = raizComum // Define a raiz como diretório de trabalho
                };

                using (Process proc = Process.Start(psi))
                {
                    string output = await proc.StandardOutput.ReadToEndAsync();
                    string error = await proc.StandardError.ReadToEndAsync();
                    await proc.WaitForExitAsync();

                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine($"⚠ Erro do 7-Zip: {error}");
                    }
                    else
                    {
                        Console.WriteLine($"✔ Diretórios compactados com sucesso: {arquivoSaida}");
                    }
                }

                // Remove o arquivo temporário após a compactação
                if (File.Exists(fileListPath))
                {
                    File.Delete(fileListPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao compactar diretórios: {ex.Message}");
            }
        }
    }
}
