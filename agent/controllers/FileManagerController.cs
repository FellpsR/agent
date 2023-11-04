using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using FileManager.Model;

[Route("link")]
[ApiController]
public class FileManagerController : ControllerBase
{
    [HttpPost]
    public IActionResult CheckFile([FromBody] FileManagerModel request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Url))
            {
                return BadRequest(new { message = "Caminho informado em branco ou nulo!" });
            }

            string path = request.Url;

            if (System.IO.File.Exists(path) || Directory.Exists(path))
            {
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        var fileInfo = new FileInfo(path);

                        if (fileInfo.IsReadOnly)
                        {
                            return BadRequest(new { message = "Sem permissão de escrita no arquivo." });
                        }

                        try
                        {
                            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                            {
                                // Faz a verificação se o usuário tem permissão para Ler o arquivo
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            return BadRequest(new { message = "Usuário sem permissão." });
                        }

                        using (var process = new Process())
                        {
                            process.StartInfo.FileName = path;
                            process.StartInfo.UseShellExecute = true;
                            process.Start();
                        }
                    }
                    else
                    {
                        var directoryInfo = new DirectoryInfo(path);

                        if ((directoryInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            return BadRequest(new { message = "Sem permissão de escrita no diretório." });
                        }

                        try
                        {
                            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                            {
                                // Faz a verificação se o usuário tem permissão para Ler o diretório
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            return BadRequest(new { message = "Usuário sem permissão." });
                        }

                        Process.Start("explorer.exe", path);
                    }
                    return Ok(new { message = "path aberto com sucesso." });
                }
                catch (FileNotFoundException ex)
                {
                    var msg = "Arquivo não encontrado. " + ex.Message;
                    System.Console.WriteLine(msg);
                    return NotFound(new { message = msg });
                }
                catch (UnauthorizedAccessException ex)
                {
                    var msg = "Acesso não autorizado. " + ex.Message;
                    System.Console.WriteLine(msg);
                    return BadRequest(new { message = msg });
                }
                catch (Exception ex)
                {
                    var msg = "Erro ao abrir arquivo. " + ex.Message;
                    System.Console.WriteLine(msg);
                    return NotFound(new { message = msg });
                }
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex);
            return StatusCode(500, "Erro interno do servidor " + ex.Message);
        }
    }
}