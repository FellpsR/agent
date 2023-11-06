using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
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
                    // Verifica as permissões do diretório ou arquivo.
                    var accessControl = System.IO.File.Exists(path) ? (FileSystemSecurity)new FileSecurity(path, AccessControlSections.Access) : new DirectorySecurity(path, AccessControlSections.Access);
                    var rules = accessControl.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));

                    // Verifica se o usuário atual tem permissão para acessar o diretório ou arquivo.
                    var currentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    var hasPermission = false;
                    foreach (FileSystemAccessRule rule in rules)
                    {
                        if (rule.IdentityReference.Value.Equals(currentUser, StringComparison.CurrentCultureIgnoreCase) && rule.AccessControlType == AccessControlType.Allow)
                        {
                            hasPermission = true;
                            break;
                        }
                    }

                    if (!hasPermission)
                    {
                        return Forbid();
                    }

                    // Se o caminho é um arquivo, ele é aberto aqui.
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                    return Ok(new { message = "path aberto com sucesso." });
                }
                catch (SystemException ex)
                {
                    var msg = "Erro ao abrir arquivo. " + ex.Message;
                    Console.WriteLine(msg);
                    return BadRequest(new { message = msg });
                }
            }
            else
            {
                // Se o caminho não existe, retorna um não encontrado.
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            // Lidar com exceções, se necessário.
            return StatusCode(500, "Teste de msg de retorno! " + ex.Message);
        }
    }
}
